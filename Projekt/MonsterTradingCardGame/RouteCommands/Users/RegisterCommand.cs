using MonsterTradingCardGame.Exceptions;
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
    class RegisterCommand : IRouteCommand
    {
        private readonly IRepoManager repoManager;
        public Credentials credentials { get; private set; }

        public RegisterCommand(IRepoManager repoManager, Credentials credentials)
        {
            this.credentials = credentials;
            this.repoManager = repoManager;
        }

        public Response Execute()
        {
            var response = new Response();

            try
            {
                repoManager.CreateUser(credentials);

                response.StatusCode = StatusCode.Created;
                response.Payload = credentials.Username + " created.";
            }
            catch(UserAlreadyExistsExcpt)
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = credentials.Username + " already exists.";
            }

            return response;
        }
    }
}
