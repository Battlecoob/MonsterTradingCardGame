using MonsterTradingCardGame.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWE1HttpServer.Core.Server;
using MonsterTradingCardGame.Models;
using SWE1HttpServer.Core.Routing;

namespace MonsterTradingCardGame.Managers
{
    class MTCGManager
    {
        private static Database _db;
        private static HttpServer _httpServer;
        private static IdentityProvider _identityProvider;
        private static RouteParser _routeParser;
        private static Router _router;

        public MTCGManager(string connection)
        {
            _db = new Database(connection);
            _identityProvider = new IdentityProvider(_db.UserRepository);

        }
    }
}
