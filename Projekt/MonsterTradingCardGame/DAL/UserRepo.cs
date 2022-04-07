using MonsterTradingCardGame.Models;
using Npgsql;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.DAL
{
    class UserRepo : IUserRepo
    {

        private Mutex mutex;
        private readonly NpgsqlConnection _connection;

        // '@' for utf-16 representation
        private const string _createTables = @"create table if not exists users 
                                                (
                                                    username varchar                not null,
                                                    password varchar                not null,
                                                    token    varchar                not null,
                                                    user_id  serial 
                                                        constraint users_pk         primary key,
                                                    image    text,
                                                    bio      text,
                                                    name     text,
                                                    coins    integer default 20     not null,
                                                    coinsBought integer default 0   not null,
                                                    wins     integer default 0      not null,
                                                    losses   integer default 0      not null,
                                                    draws    integer default 0      not null,
                                                    elo      integer default 100    not null
                                                );

                                                alter table users
                                                    owner to postgres;

                                                create unique index if not exists users_user_id_uindex
                                                    on users(user_id);

                                                create unique index if not exists users_token_uindex
                                                    on users(token);

                                                create unique index if not exists users_username_uindex
                                                    on users(username);
                                                ";

        private const string _selectCoins = "SELECT coins FROM users WHERE token = @token";
        private const string _updateCoins = "UPDATE users SET coins = coins - 5 WHERE token = @token";
        private const string _updateStatsDraw = "UPDATE users SET draws = draws + 1 WHERE token = @token";
        private const string _selectUser = "SELECT username, password FROM users WHERE token = @token";
        private const string _selectUserDataByUsername = "SELECT name, bio, image FROM users WHERE username = @username";
        private const string _updateStatsWinner = "UPDATE users SET wins = wins + 1, elo = elo + 3 WHERE token = @token";
        private const string _updateStatsLoser = "UPDATE users SET losses = losses + 1, elo = elo - 5 WHERE token = @token";
        private const string _selectUserStats = "SELECT username, wins, losses, draws, elo FROM users WHERE token = @token";
        private const string _selectTopElo = "SELECT username, wins, losses, draws, elo FROM users ORDER BY elo DESC LIMIT 10;";
        private const string _updateUserDataByUsername = "UPDATE users SET name=@name, bio = @bio, image = @image WHERE username = @username";
        private const string _insertUser = "INSERT INTO users(username, password, token) VALUES (@username, @password, @token)";
        private const string _selectUserByCredentials = "SELECT username, password FROM users WHERE username = @username AND password = @password";
        //private const string _buyCoins = "UPDATE users SET timesCoinsGotBought = timesCoinsGotBought + 1, coins = coins + 5 WHERE token = @token";

        private const string _getUserByName = "SELECT token FROM users WHERE username = @username";

        private const string _truncateAllTables = "TRUNCATE TABLE users, cards, decks, packages, trading RESTART IDENTITY;"; // used in unit testing

        public UserRepo(NpgsqlConnection connection, Mutex mutex)
        {
            _connection = connection;
            this.mutex = mutex;
            CreateTables();
        }

        private void CreateTables()
        {
            using var command = new NpgsqlCommand(_createTables, _connection);

            mutex.WaitOne();
            command.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        public User GetUserByCredentials(string username, string password)
        {
            User user = null;
            using (var command = new NpgsqlCommand(_selectUserByCredentials, _connection))
            {
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("password", password);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                if (reader.Read())
                    user = ReadUser(reader);
            }
            return user;
        }

        public User GetUserByToken(string authToken)
        {
            User user = null;
            using (var command = new NpgsqlCommand(_selectUser, _connection))
            {
                command.Parameters.AddWithValue("token", authToken);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                if (reader.Read())
                    user = ReadUser(reader);
            }
            return user;
        }

        public bool InsertUser(User user)
        {
            var affectedRows = 0;

            try
            {
                using var command = new NpgsqlCommand(_insertUser, _connection);
                command.Parameters.AddWithValue("username", user.Username);
                command.Parameters.AddWithValue("password", user.Password);
                command.Parameters.AddWithValue("token", user.Token);

                mutex.WaitOne();
                affectedRows = command.ExecuteNonQuery();
            }
            catch (PostgresException)
            {
            }
            finally
            {
                mutex.ReleaseMutex();
            }

            return affectedRows > 0;
        }

        public int SelectCoins(string authToken)
        {
            int coins = 0;

            using (var command = new NpgsqlCommand(_selectCoins, _connection))
            {
                command.Parameters.AddWithValue("token", authToken);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                if (reader.Read())
                    coins = ReadCoins(reader).Coins;
            }

            return coins;
        }

        public UserData SelectUserDataByUsername(string username)
        {
            UserData userData = null;

            using (var command = new NpgsqlCommand(_selectUserDataByUsername, _connection))
            {
                command.Parameters.AddWithValue("username", username);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                if (reader.Read())
                    userData = ReadUserData(reader);
            }

            return userData;
        }

        public UserStats SelectUserStats(string authToken)
        {
            UserStats userStats = null;
            using (var command = new NpgsqlCommand(_selectUserStats, _connection))
            {
                command.Parameters.AddWithValue("token", authToken);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                if (reader.Read())
                    userStats = ReadUserStats(reader);
            }
            return userStats;
        }

        public void UpdateCoins(string authToken)
        {
            using (var command = new NpgsqlCommand(_updateCoins, _connection))
            {
                command.Parameters.AddWithValue("token", authToken);


                mutex.WaitOne();
                command.ExecuteNonQuery();
                mutex.ReleaseMutex();
            }
        }

        public void UpdateStatsDraw(string authToken)
        {
            try
            {
                using var command = new NpgsqlCommand(_updateStatsDraw, _connection);
                command.Parameters.AddWithValue("token", authToken);

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

        public void UpdateStatsLoser(string authToken)
        {
            try
            {
                using var command = new NpgsqlCommand(_updateStatsLoser, _connection);
                command.Parameters.AddWithValue("token", authToken);

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

        public void UpdateStatsWinner(string authToken)
        {
            try
            {
                using var command = new NpgsqlCommand(_updateStatsWinner, _connection);
                command.Parameters.AddWithValue("token", authToken);

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

        public void UpdateUserData(string username, UserData userData)
        {
            using (var command = new NpgsqlCommand(_updateUserDataByUsername, _connection))
            {
                command.Parameters.AddWithValue("name", userData.Name);
                command.Parameters.AddWithValue("bio", userData.Bio);
                command.Parameters.AddWithValue("image", userData.Image);
                command.Parameters.AddWithValue("username", username);

                mutex.WaitOne();
                command.ExecuteNonQuery();
                mutex.ReleaseMutex();
            }
        }

        public List<UserStats> GetTop10UserScore()
        {
            var scores = new List<UserStats>();

            using (var command = new NpgsqlCommand(_selectTopElo, _connection))
            {
                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                while (reader.Read())
                {
                    var userScore = ReadUserStats(reader);
                    scores.Add(userScore);
                }
            }

            return scores;
        }

        public void TruncateTables()
        {
            using var command = new NpgsqlCommand(_truncateAllTables, _connection);

            mutex.WaitOne();
            command.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        private User ReadUser(IDataRecord dataRecord) // IDataRecord
        {
            /* 
                IDataRecord["string"]
                Summary:
                Gets the column with the specified name.
        
                Parameters:
                name:
                The name of the column to find.
        
                Returns:
                The column with the specified name as an System.Object.
             */
            var user = new User
            {
                Username = Convert.ToString(dataRecord["username"]),
                Password = Convert.ToString(dataRecord["password"])
            };

            return user;
        }

        private User ReadCoins(IDataRecord dataRecord)
        {
            var user = new User
            {
                Coins = Convert.ToInt32(dataRecord["coins"])
            };

            return user;
        }

        private UserData ReadUserData(IDataRecord dataRecord)
        {
            var userData = new UserData
            {
                Name = Convert.ToString(dataRecord["name"]),
                Bio = Convert.ToString(dataRecord["bio"]),
                Image = Convert.ToString(dataRecord["image"])
            };

            return userData;
        }

        private UserStats ReadUserStats(IDataRecord dataRecord)
        {
            var userData = new UserStats
            {
                Username = Convert.ToString(dataRecord["username"]),
                Wins = Convert.ToInt32(dataRecord["wins"]),
                Losses = Convert.ToInt32(dataRecord["losses"]),
                Draws = Convert.ToInt32(dataRecord["draws"]),
                Elo = Convert.ToInt32(dataRecord["elo"])
            };

            return userData;
        }

        public string GetUserByUsername(string username)
        {
            string token = "";

            using (var command = new NpgsqlCommand(_getUserByName, _connection))
            {
                command.Parameters.AddWithValue("username", username);

                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                if (reader.Read())
                    token = Convert.ToString(reader["token"]);
            }

            return token;
        }
    }
}
