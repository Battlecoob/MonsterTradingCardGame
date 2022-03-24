using MonsterTradingCardGame.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;

namespace MonsterTradingCardGame.RouteCommands
{
    public class TruncateDBCommand : IRouteCommand
    {
        private readonly IRepoManager _repoManager;

        public TruncateDBCommand(IRepoManager repoManager)
        {
            _repoManager = repoManager;
        }

        public Response Execute()
        {
            _repoManager.TruncateDB();
            return new Response()
            {
                StatusCode = StatusCode.Ok,
                Payload = "Database cleared for accurate testing."
            };
        }
    }
}
