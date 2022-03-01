using MonsterTradingCard.Exceptions;
using MonsterTradingCardGame.Models;
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
    class UpdateUserDataCommand : ProtectedRouteCommand
    {
        private UserData userData { get; set; }
        private string givenUsername { get; set; }
        private readonly IRepoManager repoManager;

        public UpdateUserDataCommand(IRepoManager repoManager, string givenUsername, UserData userData)
        {
            this.userData = userData;
            this.repoManager = repoManager;
            this.givenUsername = givenUsername;
        }

        public override Response Execute()
        {
            var response = new Response();

            string username = User.Token.Split('-').FirstOrDefault();

            if(username != givenUsername)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = "Unauthorized access to " + givenUsername + "'s data.";
                return response;
            }

            if(repoManager.GetUserData(username) != null)
            {
                repoManager.UpdateUserData(username, userData);

                response.StatusCode = StatusCode.Ok;
                response.Payload = "User sucessfully updated.";
                return response;
            }

            response.StatusCode = StatusCode.NotFound;
            response.Payload = "User doesn't exist.";
            return response;
        }
    }
}
