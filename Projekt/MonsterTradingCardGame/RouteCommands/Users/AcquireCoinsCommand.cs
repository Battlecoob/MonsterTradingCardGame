using MonsterTradingCardGame.Exceptions;
using MonsterTradingCardGame.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.RouteCommands
{
    class AcquireCoinsCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;
        public List<Card> Cards { get; private set; }

        public AcquireCoinsCommand(IRepoManager repoManager)
        {
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            throw new NotImplementedException();
        }
    }
}
