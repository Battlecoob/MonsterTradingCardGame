using System.Collections.Generic;

namespace MonsterTradingCardGame.Models
{
    public class Round
    {
        public bool Draw { get; set; }
        public int Number { get; set; }
        public string Loser { get; set; }
        public string Winner { get; set; }
        public List<Player> Players { get; set; }
    }
}
