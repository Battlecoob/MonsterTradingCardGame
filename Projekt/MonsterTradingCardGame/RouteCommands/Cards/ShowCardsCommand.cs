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
    class ShowCardsCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;

        public ShowCardsCommand(IRepoManager repoManager)
        {
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            string str = "";
            var response = new Response();

            foreach (Card card in repoManager.GetCards(User.Token))
                str += JsonConvert.SerializeObject(card);

            response.StatusCode = StatusCode.Ok;
            response.Payload = str + "\n";

            return response;
        }
    }
}
