using MonsterTradingCard.Exceptions;
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
    class AcquirePackageCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;
        public List<Card> Cards { get; private set; }

        public AcquirePackageCommand(IRepoManager repoManager)
        {
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            Package package = null;

            package = repoManager.GetFirstPack();

            if(package == null)
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = "No packs available.";
                return response;
            }

            if(!repoManager.CheckCoinAmount(User.Token))
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = "Not enough coins for purchase.";
                return response;
            }

            repoManager.AcquirePackage(package, User.Token);
            response.StatusCode = StatusCode.Ok;
            response.Payload = "Package sucessfully purchased.";

            return response;
        }
    }
}
