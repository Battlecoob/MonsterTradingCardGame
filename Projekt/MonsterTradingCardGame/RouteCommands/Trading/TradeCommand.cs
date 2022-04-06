﻿using System;
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
            if (trade.Type == card.CardType &&
                trade.Element == card.Element &&
                trade.Species == card.Species &&
                trade.MinimumDamage <= card.Damage)
                return true;

            return false;
        }
    }
}