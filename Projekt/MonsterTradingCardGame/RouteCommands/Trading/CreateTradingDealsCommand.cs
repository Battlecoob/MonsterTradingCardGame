using MonsterTradingCardGame.Exceptions;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Models.Enums;
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
    class CreateTradingDealsCommand : ProtectedRouteCommand
    {
        private readonly Trade trade;
        private readonly IRepoManager repoManager;

        public CreateTradingDealsCommand(IRepoManager repoManager, Trade trade)
        {
            this.trade = trade;
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            var response = new Response();

            if(repoManager.CardAndUserExistForTrade(trade.CardToTrade, User.Token))
            {
                //if(trade.Type == CardType.monster || trade.Type == CardType.spell)
                //{
                    if (!repoManager.CardOpenForTrade(trade.CardToTrade))
                    {
                        repoManager.CreateTrade(trade, User.Token);

                        response.Payload = "Trade created.";
                        response.StatusCode = StatusCode.Ok;
                    }
                    else
                    {
                        response.Payload = "Card is currently open for trading.";
                        response.StatusCode = StatusCode.Conflict;
                    }
                //}
            }
            else
            {
                response.Payload = "User doesn't have this card";
                response.StatusCode = StatusCode.Conflict;
            }

            return response;
        }
    }
}
