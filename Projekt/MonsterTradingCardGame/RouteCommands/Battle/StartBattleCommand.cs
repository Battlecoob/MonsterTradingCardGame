using MonsterTradingCardGame.Models;
using Newtonsoft.Json;
using SWE1HttpServer.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MonsterTradingCardGame.Exceptions;
using MonsterTradingCardGame.Managers;

namespace MonsterTradingCardGame.RouteCommands
{
    class StartBattleCommand : ProtectedRouteCommand
    {
        private readonly IRepoManager _repoManager;
        private static Queue<(User, Deck, ManualResetEvent, Action<BattleLog>)> _queue = new();

        public StartBattleCommand(IRepoManager repoManager)
        {
            _repoManager = repoManager;
        }

        public override Response Execute()
        {
            Deck deckPlayer;

            try
            {
                if ((deckPlayer = _repoManager.GetDeck(User.Token)) == null)
                    throw new InvalidDeckExcpt();

                if (_queue.Count == 0)
                {
                    BattleLog entry = new BattleLog();
                    ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                    Action<BattleLog> log = new Action<BattleLog>(a => { entry = a; });

                    _queue.Enqueue((User, deckPlayer, manualResetEvent, log));
                    manualResetEvent.WaitOne();

                    return new Response()
                    {
                        StatusCode = StatusCode.Ok,
                        Payload = JsonConvert.SerializeObject(entry)
                    };
                }
                else
                {
                    (User enemy, Deck deckEnemy, ManualResetEvent manualResetEvent, Action<BattleLog> log) = _queue.Dequeue();

                    BattleLog battle = new BattleManager(_repoManager, deckPlayer, User.Username, deckEnemy, enemy.Username).StartBattle();
                    log(battle);

                    manualResetEvent.Set();

                    return new Response()
                    {
                        StatusCode = StatusCode.Ok,
                        Payload = JsonConvert.SerializeObject(battle)
                    };
                }

            }
            catch(InvalidDeckExcpt)
            {
                return new Response()
                {
                    StatusCode = StatusCode.Conflict,
                    Payload = "Player has no deck."
                };

            }
        }
    }
}
