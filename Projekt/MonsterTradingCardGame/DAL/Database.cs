using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonsterTradingCardGame.Exceptions;
using Npgsql;


namespace MonsterTradingCardGame.DAL
{
    public class Database
    {
        private static Mutex mutex = new();
        private readonly NpgsqlConnection _connection;
        
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
                _connection.Open();

                UserRepository = new UserRepo(_connection, mutex);
                CardRepository = new CardRepo(_connection, mutex);
                PackageRepository = new PackageRepo(_connection, mutex);
                DeckRepository = new DeckRepo(_connection, mutex);
                //TradeRepository = new TradeRepo(_connection, mutex);
            }
            catch (NpgsqlException e)
            {
                throw new DatabaseAccessFailedExcpt("Database connection / initialisation failed.", e);
            }
        }
    }
}
