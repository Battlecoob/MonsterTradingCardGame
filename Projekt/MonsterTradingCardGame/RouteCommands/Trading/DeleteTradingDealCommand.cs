using System;
using SWE1HttpServer.Core.Response;
using MonsterTradingCardGame.Models;

namespace MonsterTradingCardGame.RouteCommands
{
    class DeleteTradingDealCommand : ProtectedRouteCommand
    {
        private readonly string tradeId;
        private readonly IRepoManager repoManager;

        public DeleteTradingDealCommand(IRepoManager repoManager, string tradeId)
        {
            this.tradeId = tradeId;
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            var response = new Response();

            if (repoManager.DeleteTrade(tradeId, User.Token))
            {
                response.Payload = "Trade got deleted.";
                response.StatusCode = StatusCode.Ok;
            }
            else
            {
                response.Payload = "Trade either doesn't exist or is not from this user.";
                response.StatusCode = StatusCode.Conflict;
            }

            return response;
        }
    }
}
