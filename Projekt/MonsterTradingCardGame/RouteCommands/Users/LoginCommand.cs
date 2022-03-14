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
    class LoginCommand : IRouteCommand
    {
        private readonly IRepoManager repoManager;
        public Credentials credentials { get; private set; }

        public LoginCommand(IRepoManager repoManager, Credentials credentials)
        {
            this.credentials = credentials;
            this.repoManager = repoManager;
        }

        public Response Execute()
        {
            User user;
            var response = new Response();

            try
            {
                user = repoManager.LoginUser(credentials);
            }
            catch(UserDoesntExistExcpt)
            {
                user = null;
            }

            if (user != null)
            {
                response.StatusCode = StatusCode.Ok;
                response.Payload = user.Username + " logged in. Token: " + user.Token;
            }
            else
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = "Invalid Credentials.";
            }

            return response;
        }
    }
}
