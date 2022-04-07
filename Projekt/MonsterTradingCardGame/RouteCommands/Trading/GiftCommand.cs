using System;
using SWE1HttpServer.Core.Response;
using MonsterTradingCardGame.Models;

namespace MonsterTradingCardGame.RouteCommands
{
    class GiftCommand : ProtectedRouteCommand
    {
        private readonly string cardId;
        private readonly string receiverUid;
        private readonly IRepoManager repoManager;

        public GiftCommand(IRepoManager repoManager, string receiverUid, string cardId)
        {
            this.cardId = cardId;
            this.receiverUid = receiverUid;
            this.repoManager = repoManager;

        }

        public override Response Execute()
        {
            Card card;
            string receiverToken;
            var response = new Response();

            // get user id from gift receiver by usernmae
            if((receiverToken = repoManager.GetTokenByUsername(receiverUid)) != null)
            {
                // check if user id != receiver id
                if(User.Token != receiverToken)
                {
                    // check if user has card
                    if(repoManager.CardAndUserExistForTrade(cardId, User.Token))
                    {
                        if((card = repoManager.GetCardById(cardId)) != null)
                        {
                            // change owner
                            repoManager.UpdateCardOwner(cardId, receiverToken);
                            response.StatusCode = StatusCode.Ok;
                            response.Payload = "Card Gift was sent out.";
                        }
                        else
                        {
                            response.StatusCode = StatusCode.InternalServerError;
                            response.Payload = "Internal Server error while trying to gift card.";
                        }
                    }
                    else
                    {
                        response.StatusCode = StatusCode.Conflict;
                        response.Payload = "You currently don't have this card.";
                    }
                }
                else
                {
                    response.StatusCode = StatusCode.Conflict;
                    response.Payload = "You can't gift yourself.";
                }
            }
            else
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = "The User you want to send a gift to doesn't exist.";
            }

            return response;
        }
    }
}
