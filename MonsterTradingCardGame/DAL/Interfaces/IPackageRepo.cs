using MonsterTradingCardGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.DAL
{
    public interface IPackageRepo
    {
        Package GetFirstPack();
        void InsertPackage(Package package);
        void UpdatePackageOwner(int packageId, string authToken);
    }
}
