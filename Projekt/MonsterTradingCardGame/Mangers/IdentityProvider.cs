using MonsterTradingCardGame.DAL;
using SWE1HttpServer.Core.Authentication;
using SWE1HttpServer.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Models
{
    class IdentityProvider : IIdentityProvider
    {
        private readonly IUserRepo userRepo;

        public IdentityProvider(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        public IIdentity GetIdentyForRequest(RequestContext request)
        {
            User currentUser = null;

            if (request.Header.TryGetValue("Authorization", out string authToken))
            {
                const string prefix = "Basic ";
                if (authToken.StartsWith(prefix))
                    currentUser = userRepo.GetUserByToken(authToken.Substring(prefix.Length));
            }

            return currentUser;
        }
    }
}
