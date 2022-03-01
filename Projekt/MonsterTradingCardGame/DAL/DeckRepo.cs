using Npgsql;
using MonsterTradingCardGame.Models;
using System;
using System.Data;
using System.Collections.Generic;
using System.Threading;

namespace MonsterTradingCardGame.DAL
{
    class DeckRepo : IDeckRepo
    {
        private Mutex mutex;
        private readonly NpgsqlConnection _connection;

        private const string CreateTableCommand = @"create table if not exists decks
                                                    (
                                                        deck_id  serial
                                                            constraint decks_pk
                                                                primary key,
                                                        token    text not null
                                                            constraint decks_users_token_fk
                                                                references users (token)
                                                                on update cascade on delete cascade,
                                                        card1_id text
                                                            constraint decks_cards_card_id_fk
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card2_id text
                                                            constraint decks_cards_card_id_fk_2
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card3_id text
                                                            constraint decks_cards_card_id_fk_3
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card4_id text
                                                            constraint decks_cards_card_id_fk_4
                                                                references cards
                                                                on update cascade on delete cascade
                                                    );

                                                    alter table decks
                                                        owner to postgres;

                                                    create unique index if not exists decks_card1_id_uindex
                                                        on decks (card1_id);

                                                    create unique index if not exists decks_card2_id_uindex
                                                        on decks (card2_id);

                                                    create unique index if not exists decks_card3_id_uindex
                                                        on decks (card3_id);

                                                    create unique index if not exists decks_card4_id_uindex
                                                        on decks (card4_id);

                                                    create unique index if not exists decks_deck_id_uindex
                                                        on decks (deck_id);

                                                    create unique index if not exists decks_user_id_uindex
                                                        on decks (token);
                                                    ";

        private const string _selectDeck = "SELECT * FROM decks WHERE token = @token";
        private const string _updateDeck = "UPDATE decks SET card1_id=@card1_id, card2_id=@card2_id, card3_id=@card3_id, card4_id=@card4_id WHERE token=@token";
        private const string _insertDeck = "INSERT INTO decks(token, card1_id, card2_id, card3_id, card4_id) VALUES (@token, @card1_id, @card2_id, @card3_id, @card4_id) RETURNING deck_id";

        public DeckRepo(NpgsqlConnection connection, Mutex mutex)
        {
            _connection = connection;
            this.mutex = mutex;
            CreateTables();
        }

        public Deck SelectDeck(string authToken)
        {
            Deck deck = null;

            using (var command = new NpgsqlCommand(_selectDeck, _connection))
            {
                command.Parameters.AddWithValue("token", authToken);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();
                
                if (reader.Read())
                    deck = ReadDeck(reader);
            }

            return deck;
        }

        public void UpdateDeck(string authToken, List<string> cardIds)
        {
            using var command = new NpgsqlCommand(_updateDeck, _connection);
            command.Parameters.AddWithValue("card1_id", cardIds[0]);
            command.Parameters.AddWithValue("card2_id", cardIds[1]);
            command.Parameters.AddWithValue("card3_id", cardIds[2]);
            command.Parameters.AddWithValue("card4_id", cardIds[3]);
            command.Parameters.AddWithValue("token", authToken);

            mutex.WaitOne();
            command.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        public int InsertDeck(string authToken, List<string> cardIds)
        {
            using var command = new NpgsqlCommand(_insertDeck, _connection);
            command.Parameters.AddWithValue("token", authToken);
            command.Parameters.AddWithValue("card1_id", cardIds[0]);
            command.Parameters.AddWithValue("card2_id", cardIds[1]);
            command.Parameters.AddWithValue("card3_id", cardIds[2]);
            command.Parameters.AddWithValue("card4_id", cardIds[3]);

            mutex.WaitOne();
            var result = command.ExecuteScalar();
            mutex.ReleaseMutex();

            return Convert.ToInt32(result);
        }

        private void CreateTables()
        {
            using var command = new NpgsqlCommand(CreateTableCommand, _connection);
         
            mutex.WaitOne();
            command.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        private Deck ReadDeck(IDataRecord record)
        {
            var cardIds = new List<string>();

            cardIds.Add(Convert.ToString(record["card1_id"]));
            cardIds.Add(Convert.ToString(record["card2_id"]));
            cardIds.Add(Convert.ToString(record["card3_id"]));
            cardIds.Add(Convert.ToString(record["card4_id"]));

            var deck = new Deck
            {
                CardIds = cardIds,
                Id = Convert.ToInt32(record["deck_id"]),
                Token = Convert.ToString(record["token"])
            };
            return deck;
        }
    }
}
