using MonsterTradingCardGame.Exceptions;
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
    class ConfigureDeckCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;
        private readonly List<string> cardIds;

        public ConfigureDeckCommand(IRepoManager repoManager, List<string> cardIds)
        {
            this.cardIds = cardIds;
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            int deckId;
            var response = new Response();

            if(cardIds.Count != 4)
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = "You have to provide four cards to configure your deck.";
                return response;
            }

            try
            {
                foreach(string cardId in cardIds)
                {
                    if (!repoManager.UserHasCard(cardId, User.Token))
                        throw new UserDoesntHaveCardExcpt(cardId);
                }

                if(repoManager.DeckExists(User.Token))
                {
                    repoManager.UpdateDeck(User.Token, cardIds);
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = "Deck sucessfully updated.";
                }
                else
                {
                    deckId = repoManager.CreateDeck(User.Token, cardIds);
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = "Deck sucessfully created. Id: " + deckId;
                }
            }
            catch(UserDoesntHaveCardExcpt e)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Payload = "User doesn't have this Card: " + e;
            }

            return response;
        }
    }
}
