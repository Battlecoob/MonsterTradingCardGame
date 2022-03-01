using Npgsql;
using MonsterTradingCardGame.Models;
using System;
using System.Data;
using System.Collections.Generic;
using System.Threading;

namespace MonsterTradingCardGame.DAL
{
    class CardRepo : ICardRepo
    {
        private Mutex mutex;
        private readonly NpgsqlConnection _connection;

        private const string _createTable =   @"create table if not exists cards
                                                (
                                                    card_id text    not null
                                                        constraint cards_pk
                                                            primary key,
                                                    name    text    not null,
                                                    dmg     integer not null,
                                                    token   text
                                                        constraint cards_users_token_fk
                                                            references users (token)
                                                            on update cascade on delete cascade
                                                            deferrable initially deferred
                                                );";

        private const string _selectCardsByToken        = "SELECT * FROM cards WHERE token = @token";
        private const string _selectCardById            = "SELECT * FROM cards WHERE card_id = @card_id";
        private const string _updateCardOwnerByToken    = "UPDATE cards SET token = @token WHERE card_id = @card_id";
        private const string _selectCardByIdToken       = "SELECT * FROM cards WHERE card_id = @card_id AND token = @token";
        private const string _insertCard                = "INSERT INTO cards(card_id, name, dmg) VALUES (@card_id, @name, @dmg)";


        public CardRepo(NpgsqlConnection connection, Mutex mutex)
        {
            _connection = connection;
            this.mutex = mutex;
            CreateTables();
        }

        public IEnumerable<Card> SelectCardsByToken(string authToken)
        {
            var cards = new List<Card>();

            using (var command = new NpgsqlCommand(_selectCardsByToken, _connection))
            {
                command.Parameters.AddWithValue("token", authToken);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                while (reader.Read())
                {
                    var card = ReadCard(reader);
                    cards.Add(card);
                }
            }
            return cards;
        }

        public void InsertCard(Card card)
        {
            try
            {
                using var command = new NpgsqlCommand(_insertCard, _connection);
                command.Parameters.AddWithValue("card_id", card.Id);
                command.Parameters.AddWithValue("name", card.Name);
                command.Parameters.AddWithValue("dmg", card.Damage);
            
                mutex.WaitOne();
                command.ExecuteNonQuery();
            }
            catch (PostgresException)
            {
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public Card SelectCardById(string cardId)
        {
            Card card = null;

            using (var command = new NpgsqlCommand(_selectCardById, _connection))
            {
                command.Parameters.AddWithValue("card_id", cardId);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();
                
                if (reader.Read())
                    card = ReadCard(reader);
            }
            return card;
        }

        public void UpdateCardOwner(string cardId, string authToken)
        {
            using var command = new NpgsqlCommand(_updateCardOwnerByToken, _connection);
            command.Parameters.AddWithValue("token", authToken);
            command.Parameters.AddWithValue("card_id", cardId);

            mutex.WaitOne();
            command.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        public Card SelectCardByIdAndToken(string cardId, string authToken)
        {
            Card card = null;

            using (var command = new NpgsqlCommand(_selectCardByIdToken, _connection))
            {
                command.Parameters.AddWithValue("card_id", cardId);
                command.Parameters.AddWithValue("token", authToken);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();
                
                if (reader.Read())
                    card = ReadCard(reader);
            }
            return card;
        }

        private void CreateTables()
        {
            using var command = new NpgsqlCommand(_createTable, _connection);

            mutex.WaitOne();
            command.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        private Card ReadCard(IDataRecord record)
        {
            var card = new Card
            {
                Damage = Convert.ToInt32(record["dmg"]),
                Name = Convert.ToString(record["name"]),
                Id = Convert.ToString(record["card_id"])
            };

            return card;
        }
    }
}
