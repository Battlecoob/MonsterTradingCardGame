using System;
using SWE1HttpServer.Core.Response;
using MonsterTradingCardGame.Models;

namespace MonsterTradingCardGame.RouteCommands
{
    class TradeCommand : ProtectedRouteCommand
    {
        private readonly string cardId;
        private readonly string tradeId;
        private readonly IRepoManager repoManager;

        public TradeCommand(IRepoManager repoManager, string tradeId, string cardId)
        {
            this.cardId = cardId;
            this.tradeId = tradeId;
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            Card card;
            Trade trade;
            var response = new Response();

            if((trade = repoManager.GetTrade(tradeId)) != null)
            {
                if (User.Token != trade.Token)
                {
                    if((card = repoManager.GetCardByIdToken(cardId, User.Token)) != null)
                    {
                        // card must not be in deck

                        if(TradeIsValid(trade, card))
                        {
                            repoManager.UpdateCardOwner(trade.CardToTrade, User.Token);
                            repoManager.UpdateCardOwner(card.Id, trade.Token);

                            if (repoManager.DeleteTrade(trade.Id, trade.Token))
                            {
                                response.StatusCode = StatusCode.Ok;
                                response.Payload = "Trade was successfull.";
                            }
                            else
                            {
                                repoManager.UpdateCardOwner(card.Id, User.Token);
                                repoManager.UpdateCardOwner(trade.CardToTrade, trade.Token);

                                response.StatusCode = StatusCode.InternalServerError;
                                response.Payload = "Internal Server error while trying to trade card.";
                            }
                        }
                        else
                        {
                            response.StatusCode = StatusCode.InternalServerError;
                            response.Payload = "Card from Trade is not the same as from User.";
                        }
                    }
                    else
                    {
                        response.StatusCode = StatusCode.Conflict;
                        response.Payload = "Card does not exist for this User.";
                    }
                }
                else
                {
                    response.StatusCode = StatusCode.Conflict;
                    response.Payload = "You can't trade with yourself.";
                }
            }
            else
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = "Trade does not exist.";
            }

            return response;
        }

        private bool TradeIsValid(Trade trade, Card card)
        {
            if  (trade.MinimumDamage <= card.Damage &&
                (trade.Type == card.CardType || trade.Type == null) &&
                (trade.Element == card.Element || trade.Element == null) &&
                (trade.Species == card.Species || trade.Species == null))
                return true;

            return false;
        }
    }
}
