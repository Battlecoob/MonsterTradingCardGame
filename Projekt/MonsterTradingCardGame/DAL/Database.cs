using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonsterTradingCard.Exceptions;
using Npgsql;


namespace MonsterTradingCardGame.DAL
{
    public class Database
    {
        private static Mutex mutex = new();
        private NpgsqlConnection _connection;
        
        // repos
        public IUserRepo UserRepository { get; private set; }
        public ICardRepo CardRepository { get; private set; }
        public IPackageRepo PackageRepository { get; private set; }
        public IDeckRepo DeckRepository { get; private set; }
        public ITradingRepo TradeRepository { get; set; }

        public Database(string connection)
        {
            try
            {
                _connection = new NpgsqlConnection(connection);
                Open();
                Console.WriteLine("test");
                //_connection.OpenAsync();

                UserRepository = new UserRepo(_connection, mutex);
                CardRepository = new CardRepo(_connection, mutex);
                PackageRepository = new PackageRepo(_connection, mutex);
                DeckRepository = new DeckRepo(_connection, mutex);
                TradeRepository = new TradeRepo(_connection, mutex);
                
                //Close();
            }
            catch (NpgsqlException e)
            {
                throw new DatabaseAccessFailedExcpt("Database connection / initialisation failed.", e);
            }
        }

        public async void Open()
        {
            await _connection.OpenAsync();
        }

        public async void Close()
        {
            await _connection.CloseAsync();
        }
    }
}
