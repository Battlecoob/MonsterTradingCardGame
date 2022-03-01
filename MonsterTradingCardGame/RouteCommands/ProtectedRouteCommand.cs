using SWE1HttpServer.Core.Routing;
using SWE1HttpServer.Core.Response;
using MonsterTradingCardGame.Models;
using SWE1HttpServer.Core.Authentication;

namespace MonsterTradingCardGame.RouteCommands
{
    abstract class ProtectedRouteCommand : IProtectedRouteCommand
    {
        public abstract Response Execute();
        public User User => (User)Identity;
        public IIdentity Identity { get; set; }
    }
}
