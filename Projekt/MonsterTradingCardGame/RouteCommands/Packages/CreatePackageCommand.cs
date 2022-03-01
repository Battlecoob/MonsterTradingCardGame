using MonsterTradingCard.Exceptions;
using MonsterTradingCardGame.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.RouteCommands
{
    class CreatePackageCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager repoManager;
        public List<Card> Cards { get; private set; }

        public CreatePackageCommand(IRepoManager repoManager, List<Card> cards)
        {
            Cards = cards;
            this.repoManager = repoManager;
        }

        public override Response Execute()
        {
            var response = new Response();

            if(User.Token != "admin-mtcgToken")
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = "Only admins can do such actions."; // negativ fall - .bat
                return response;
            }

            if(Cards == null)
            {
                response.StatusCode = StatusCode.NoContent;
                response.Payload = "No card ids.";
                return response;
            }

            if(Cards.Count == 5)
            {
                try
                {
                    repoManager.CardExists(Cards);

                    foreach (Card card in Cards)
                        repoManager.AddCard(card);

                    repoManager.CreatePackage(Cards);

                    response.StatusCode = StatusCode.Ok;
                    response.Payload = "Package successfully created.";
                }
                catch(CardAlreadyExistsExcpt) // e gibt den pfad aus
                {
                    response.StatusCode = StatusCode.Conflict;
                    response.Payload = "Card already exists.";
                    //response.Payload = "Card with the id: " + e + " already exists.";
                }
            }
            else
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = "A Package can only be created with five cards.";
            }

            return response;
        }
    }
}
