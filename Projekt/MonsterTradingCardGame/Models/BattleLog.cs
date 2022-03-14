using System.Collections.Generic;

namespace MonsterTradingCardGame.Models
{
    public class BattleLog
    {
        public bool Draw { get; set; }
        public string Loser { get; set; }
        public string Winner { get; set; }
        public int RoundCount { get; set; }
        public List<Deck> Decks { get; set; }
        public List<Round> Rounds { get; set; }
    }
}
