using System;
using System.Net;
using MonsterTradingCardGame.DAL;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.RouteCommands;
using SWE1HttpServer.Core.Routing;
using SWE1HttpServer.Core.Server;
using Newtonsoft.Json;
using SWE1HttpServer.Core.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    class Program
    {
        static void Main()
        {
            var p = new Program();
            Task.Run(async () => await p.MTCG());
        }

        public async Task MTCG()
        {
            // init db
            string connectionString =
                "Host=localhost;" +
                "Port=5432;" + // default
                "Username=postgres;" +
                "Password=postgres;" +
                "Database=mctgdb;" +
                "Pooling=true;" + // default
                "MinPoolSize=0;" +
                //"Connection Idle Lifetime=0;" + // indefinite idle lifetime?
                "ConnectionLifetime=0;" + // default
                "Timeout=20;";

            await using Database db = new Database(connectionString);

            // init repoManager
            var repoManager = new RepoManager(db, db.UserRepository, db.CardRepository, db.PackageRepository, db.DeckRepository, db.TradeRepository);

            // init msg identity privider
            var identityProvider = new IdentityProvider(db.UserRepository);

            // init router(parser)
            var routeParser = new RouteParser();
            var router = new Router(routeParser, identityProvider);

            // register routes
            RouteRegistry(router, repoManager);

            // init server
            var httpServer = new HttpServer(IPAddress.Any, 10001, router);

            /*
             * FRAGEN AN PAUL
             * 
             * Wenn "Task.Run(() => HandleClient(client));" die Funktion asynchron aufrufbar macht, wieso koennen sich 2 clients gleichzeitig verbinden auch wenn nur
             * "HandleClient(client)";?
             *
             * Wenn bei jedem request eine neue db connection aufgebaut wird. wie muessen mutexes dann verwendet werden? Ein zentraler Mutex macht ja wieder alles seriell
             */

            httpServer.Start();
        }

        private static void RouteRegistry(Router router, IRepoManager repoManager)
        {
            //######################## Public Routes #############################
            //------------------------- User Routes ------------------------------
            // create user
            router.AddRoute(HttpMethod.Post, "/users", (r, p) => new RegisterCommand(repoManager, Deserialize<Credentials>(r.Payload)));
            // login users
            router.AddRoute(HttpMethod.Post, "/sessions", (r, p) => new LoginCommand(repoManager, Deserialize<Credentials>(r.Payload)));

            //######################## Private Routes ############################
            //------------------------ Package Routes ----------------------------
            // create package - admin
            router.AddProtectedRoute(HttpMethod.Post, "/packages", (r, p) => new CreatePackageCommand(repoManager, Deserialize<List<Card>>(r.Payload)));
            // acquire package
            router.AddProtectedRoute(HttpMethod.Post, "/transactions/packages", (r, p) => new AcquirePackageCommand(repoManager));
            
            //---------------------- Card/Deck Routes ----------------------------
            // show acquired cards
            router.AddProtectedRoute(HttpMethod.Get, "/cards", (r, p) => new ShowCardsCommand(repoManager));
            // show deck 
            router.AddProtectedRoute(HttpMethod.Get, "/deck", (r, p) => new ShowDeckCommand(repoManager));
            // config deck
            router.AddProtectedRoute(HttpMethod.Put, "/deck", (r, p) => new ConfigureDeckCommand(repoManager, Deserialize<List<string>>(r.Payload)));
            // show deck -> plain format
            router.AddProtectedRoute(HttpMethod.Get, "/deck\\?format=plain", (r, p) => new ShowDeckPlainFormatCommand(repoManager));
           
            //---------------------- User Data Routes ----------------------------
            // show user data
            router.AddProtectedRoute(HttpMethod.Get, "/users/{id}", (r, p) => new ShowUserDataCommand(repoManager, p["id"]));
            // update user data
            router.AddProtectedRoute(HttpMethod.Put, "/users/{id}", (r, p) => new UpdateUserDataCommand(repoManager, p["id"], Deserialize<UserData>(r.Payload)));
            // show user stats
            router.AddProtectedRoute(HttpMethod.Get, "/stats", (r, p) => new ShowUserStatsCommand(repoManager));
            // show scoreboard
            router.AddProtectedRoute(HttpMethod.Get, "/score", (r, p) => new ShowScoreBoardCommand(repoManager));
            // buy coins
            router.AddProtectedRoute(HttpMethod.Post, "/transactions/coins", (r, p) => new AcquireCoinsCommand(repoManager));
        }

        private static T Deserialize<T>(string payload) where T : class
        {
            var deserializedData = JsonConvert.DeserializeObject<T>(payload);

            return deserializedData;
        }
    }
}
