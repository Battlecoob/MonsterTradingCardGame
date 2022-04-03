using System.Collections.Generic;

namespace MonsterTradingCardGame.Models
{
    public struct Round
    {
        public bool Draw { get; set; }
        public string Loser { get; set; }
        public string Winner { get; set; }
        public int RoundNumber { get; set; }
        public List<Player> Players { get; set; }

        public Round(int round)
        {
            Draw = false;
            Loser = null;
            Winner = null;
            RoundNumber = round;
            Players = new List<Player>();
        }
    }
}
