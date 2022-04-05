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
            response.Payload = JsonConvert.SerializeObject(repoManager.CheckTradingDeals());

            if (response.Payload == "[]")
            {
                response.Payload = "No Trades found.";
                response.StatusCode = StatusCode.NotFound;
            }
            else
                response.StatusCode = StatusCode.Ok;

            return response;
        }
    }
}
