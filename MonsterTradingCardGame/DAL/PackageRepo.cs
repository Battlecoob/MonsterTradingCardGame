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
    class PackageRepo : IPackageRepo
    {
        private Mutex mutex;
        private readonly NpgsqlConnection _connection;

        private const string _createTable   = @"create table if not exists packages
                                                (
                                                    package_id serial primary key,
                                                    card1_id   text not null references cards   on update cascade on delete cascade,
                                                    card2_id   text not null references cards   on update cascade on delete cascade,
                                                    card3_id   text not null references cards   on update cascade on delete cascade,
                                                    card4_id   text not null references cards   on update cascade on delete cascade,
                                                    card5_id   text not null references cards   on update cascade on delete cascade,
                                                    owner      text references users (token)    on update cascade on delete cascade
                                                );

                                                    alter table packages
                                                        owner to postgres;

                                                    create unique index if not exists packages_card1_uindex
                                                        on packages (card1_id);

                                                    create unique index if not exists packages_card2_uindex
                                                        on packages (card2_id);

                                                    create unique index if not exists packages_card3_uindex
                                                        on packages (card3_id);

                                                    create unique index if not exists packages_card4_uindex
                                                        on packages (card4_id);

                                                    create unique index if not exists packages_card5_uindex
                                                        on packages (card5_id);
                                                ";

        private const string _updatePackageOwnerByToken         = "UPDATE packages SET owner=@owner WHERE package_id=@package_id";
        private const string _selectRandomPackageWithNoOwner    = "SELECT package_id, card1_id, card2_id, card3_id, card4_id, card5_id FROM packages WHERE owner IS NULL";
        private const string _insertPackage                     = "INSERT INTO packages(card1_id, card2_id, card3_id, card4_id, card5_id) VALUES (@card1_id, @card2_id, @card3_id, @card4_id, @card5_id)";

        public PackageRepo(NpgsqlConnection connection, Mutex mutex)
        {
            _connection = connection;
            this.mutex = mutex;
            CreateTables();
        }

        public void InsertPackage(Package package)
        {
            using var command = new NpgsqlCommand(_insertPackage, _connection);
            command.Parameters.AddWithValue("card1_id", package.CardIds[0]);
            command.Parameters.AddWithValue("card2_id", package.CardIds[1]);
            command.Parameters.AddWithValue("card3_id", package.CardIds[2]);
            command.Parameters.AddWithValue("card4_id", package.CardIds[3]);
            command.Parameters.AddWithValue("card5_id", package.CardIds[4]);

            mutex.WaitOne();
            var result = command.ExecuteScalar();
            mutex.ReleaseMutex();
        }

        public Package GetFirstPack()
        {
            var packages = new List<Package>();

            using (var command = new NpgsqlCommand(_selectRandomPackageWithNoOwner, _connection))
            {
                mutex.WaitOne();
                using var reader = command.ExecuteReader();
                mutex.ReleaseMutex();

                while (reader.Read())
                {
                    var package = ReadPackageWithoutOwner(reader);
                    packages.Add(package);
                }
            }

            return packages.Count > 0 ? packages[0] : null;
        }

        public void UpdatePackageOwner(int packageId, string authToken)
        {
            using var command = new NpgsqlCommand(_updatePackageOwnerByToken, _connection);
            command.Parameters.AddWithValue("owner", authToken);
            command.Parameters.AddWithValue("package_id", packageId);
            
            mutex.WaitOne();
            command.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        private void CreateTables()
        {
            using var command = new NpgsqlCommand(_createTable, _connection);

            mutex.WaitOne();
            command.ExecuteNonQuery();
            mutex.ReleaseMutex();
        }

        private Package ReadPackageWithoutOwner(IDataRecord record)
        {
            var cardIds = new List<string>();

            cardIds.Add(Convert.ToString(record["card1_id"]));
            cardIds.Add(Convert.ToString(record["card2_id"]));
            cardIds.Add(Convert.ToString(record["card3_id"]));
            cardIds.Add(Convert.ToString(record["card4_id"]));
            cardIds.Add(Convert.ToString(record["card5_id"]));

            var package = new Package
            {
                Id = Convert.ToInt32(record["package_id"]),
                CardIds = cardIds
            };
            return package;
        }
    }
}
