using MonsterTradingCardGame.Exceptions;
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
    class ShowUserDataCommand : ProtectedRouteCommand
    {
        private string str = string.Empty;
        private string givenUsername { get; set; }
        private readonly IRepoManager repoManager;

        public ShowUserDataCommand(IRepoManager repoManager, string givenUsername)
        {
            
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
                response.Payload = "Unauthorized access to " + givenUsername + "'s data!";
                return response;
            }

            str = JsonConvert.SerializeObject(repoManager.GetUserData(username));

            if (str == string.Empty)
            {
                response.StatusCode = StatusCode.NoContent;
                response.Payload = "No user data existing.";
                return response;
            }

            response.StatusCode = StatusCode.Ok;
            response.Payload = str;
            return response;
        }
    }
}
