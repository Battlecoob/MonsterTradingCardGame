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
    class ShowScoreBoardCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;

        public ShowScoreBoardCommand(IRepoManager repoManager)
        {
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            string str = "";

            foreach(UserStats userStat in repoManager.GetTopElo())
                str += JsonConvert.SerializeObject(userStat) + "\n";

            if (str == "")
            {
                response.StatusCode = StatusCode.NoContent;
            }
            else
            {
                response.StatusCode = StatusCode.Ok;
                response.Payload = str + "\n";
            }

            return response;
        }
    }
}
