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
    class ShowDeckPlainFormatCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;

        public ShowDeckPlainFormatCommand(IRepoManager repoManager)
        {
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            string str = "";

            foreach (Card card in repoManager.GetCardsInDeck(User.Token))
                str += card.ToString() + "\n";

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
