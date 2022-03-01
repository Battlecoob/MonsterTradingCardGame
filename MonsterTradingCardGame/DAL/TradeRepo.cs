using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Models;
using MonsterTradingCard.Exceptions;
using System.Threading;
using Npgsql;
using MonsterTradingCardGame.Models;
using System.Data;
using MonsterTradingCard.Models.Enums;

namespace MonsterTradingCardGame.DAL
{
    public class TradeRepo : ITradingRepo
    {
        private Mutex mutex { get; }
        private readonly NpgsqlConnection _connection;

        private const string _createTable = @"create table if not exists trading
                                                (
                                                    trading_id  text not null
                                                        constraint trading_pk
                                                            primary key,
                                                    cardtotrade text not null
                                                        constraint trading_cards_card_id_fk
                                                            references cards
                                                            on update cascade on delete cascade,
                                                    mindmg      integer,
                                                    element     public.element,
                                                    cardtype    public.card_type,
                                                    species     public.species,
                                                    usertoken   text not null
                                                        constraint trading_users_token_fk
                                                            references users (token)
                                                            on update cascade on delete cascade
                                                );

                                                alter table trading
                                                    owner to postgres;

                                                create unique index if not exists trading_cardtotrade_uindex
                                                    on trading (cardtotrade);

                                                create unique index if not exists trading_trading_id_uindex
                                                    on trading (trading_id);
                                                ";

        private const string _selectTrading = "SELECT * FROM trading";
        private const string _selectTradingById= "SELECT * FROM trading WHERE trading_id=@trading_id";
        private const string _selectTradingByCardId = "SELECT * FROM trading WHERE cardtotrade=@card_id";
        private const string _deleteTradingByIdAndToken = "DELETE FROM trading WHERE trading_id=@trading_id AND usertoken=@usertoken";
        private const string _insertTrading = "INSERT INTO trading (trading_id, usertoken, cardtotrade, mindmg, element, cardtype, species) VALUES (@trading_id, @usertoken, @cardtotrade, @mindmg, @element, @cardtype, @species)";

        public TradeRepo(NpgsqlConnection connection, Mutex mutex)
        {
            this.mutex = mutex;
            _connection = connection;
            CreateTables();
        }

        public IEnumerable<Trading> SelectOpenTrades()
        {
            var trades = new List<Trading>();

            using (var cmd = new NpgsqlCommand(_selectTrading, _connection))
            {
                mutex.WaitOne();
                using var reader = cmd.ExecuteReader();
                mutex.ReleaseMutex();

                while (reader.Read())
                {
                    var trade = ReadTrade(reader);
                    trades.Add(trade);
                }
            }
            return trades;
        }

        public Trading SelectTradeByCardId(string cardId)
        {
            Trading trade = null;

            using (var cmd = new NpgsqlCommand(_selectTradingByCardId, _connection))
            {
                cmd.Parameters.AddWithValue("card_id", cardId);

                mutex.WaitOne();
                using var reader = cmd.ExecuteReader();
                mutex.ReleaseMutex();
                
                if (reader.Read())
                    trade = ReadTrade(reader);
            }

            return trade;
        }

        public int InsertTrade(Trading trade, string authToken)
        {
            var affectedRows = 0;

            try
            {
                using var cmd = new NpgsqlCommand(_insertTrading, _connection);
                cmd.Parameters.AddWithValue("trading_id", trade.Id);
                cmd.Parameters.AddWithValue("usertoken", authToken);
                cmd.Parameters.AddWithValue("cardtotrade", trade.Card2Trade);

                if (trade.MinDmg.HasValue)
                    cmd.Parameters.AddWithValue("mindmg", trade.MinDmg);
                else
                    cmd.Parameters.AddWithValue("mindmg", DBNull.Value);
                //------------------------------------------------------

                if (trade.Type.HasValue)
                    cmd.Parameters.AddWithValue("cardtype", trade.Type);
                else
                    cmd.Parameters.AddWithValue("cardtype", DBNull.Value);
                //------------------------------------------------------

                if (trade.Element.HasValue)
                    cmd.Parameters.AddWithValue("element", trade.Element);
                else
                    cmd.Parameters.AddWithValue("element", DBNull.Value);
                //------------------------------------------------------

                if (trade.Species.HasValue)
                    cmd.Parameters.AddWithValue("species", trade.Species);
                else
                    cmd.Parameters.AddWithValue("species", DBNull.Value);
                
                mutex.WaitOne();
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {
            }
            finally
            {
                mutex.ReleaseMutex();
            }

            return affectedRows;
        }

        public int DeleteTradeByIdAndToken(string id, string authToken)
        {
            var rowsAffected = 0;

            try
            {
                var cmd = new NpgsqlCommand(_deleteTradingByIdAndToken, _connection);
                cmd.Parameters.AddWithValue("trading_id", id);
                cmd.Parameters.AddWithValue("usertoken", authToken);

                mutex.WaitOne();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {
            }
            finally
            {
                mutex.ReleaseMutex();
            }

            return rowsAffected;
        }

        public Trading SelectTradeAndTokenById(string tradingDealId)
        {
            Trading trade = null;

            using (var cmd = new NpgsqlCommand(_selectTradingById, _connection))
            {
                cmd.Parameters.AddWithValue("trading_id", tradingDealId);
                
                mutex.WaitOne();
                using var reader = cmd.ExecuteReader();
                mutex.ReleaseMutex();
                
                if (reader.Read())
                    trade = ReadTradeWithToken(reader);
            }

            return trade;
        }

        private void CreateTables()
        {
            using var cmd = new NpgsqlCommand(_createTable, _connection);
            
            mutex.WaitOne();
            cmd.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        private Trading ReadTrade(IDataRecord record)
        {
            var trade = new Trading
            {
                Id = Convert.ToString(record["trading_id"]),
                Card2Trade = Convert.ToString(record["cardtotrade"]),
                MinDmg = record["mindmg"] is DBNull ? null : Convert.ToInt32(record["mindmg"]),
                Element = record["element"] is DBNull ? null : (Element)Enum.Parse(typeof(Element), Convert.ToString(record["element"])),
                Species = record["species"] is DBNull ? null : (Species)Enum.Parse(typeof(Species), Convert.ToString(record["species"])),
                Type = record["cardtype"] is DBNull ? null : (CardType)Enum.Parse(typeof(CardType), Convert.ToString(record["cardtype"]))
            };
            return trade;
        }

        private Trading ReadTradeWithToken(IDataRecord record)
        {
            var trade = new Trading
            {
                Id = Convert.ToString(record["trading_id"]),
                Token = Convert.ToString(record["usertoken"]),
                Card2Trade = Convert.ToString(record["cardtotrade"]),
                MinDmg = record["mindmg"] is DBNull ? null : Convert.ToInt32(record["mindmg"]),
                Element = record["element"] is DBNull ? null : (Element)Enum.Parse(typeof(Element), Convert.ToString(record["element"])),
                Species = record["species"] is DBNull ? null : (Species)Enum.Parse(typeof(Species), Convert.ToString(record["species"])),
                Type = record["cardtype"] is DBNull ? null : (CardType)Enum.Parse(typeof(CardType), Convert.ToString(record["cardtype"]))
            };
            return trade;
        }
    }
}
