using MonsterTradingCardGame.Models;
using Newtonsoft.Json;
using SWE1HttpServer.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.RouteCommands
{
    class ShowUserStatsCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;

        public ShowUserStatsCommand(IRepoManager repoManager)
        {
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            var response = new Response();

            response.Payload = JsonConvert.SerializeObject(repoManager.GetUserStats(User.Token));
            
            if(response.Payload == null)
            {
                response.Payload = "User not found";
                response.StatusCode = StatusCode.NotFound;
                return response;
            }

            response.StatusCode = StatusCode.Ok;
            return response;
        }
    }
}
