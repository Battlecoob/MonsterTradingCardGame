using MonsterTradingCardGame.Exceptions;
using MonsterTradingCardGame.Models;
using Newtonsoft.Json;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.RouteCommands
{
    class CheckTradingDealsCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;

        public CheckTradingDealsCommand(IRepoManager repoManager)
        {
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            var response = new Response();

            if ((response.Payload = JsonConvert.SerializeObject(repoManager.CheckTradingDeals())) != null)
                response.StatusCode = StatusCode.Ok;
            else
                response.StatusCode = StatusCode.NotFound;

            return response;
        }
    }
}
