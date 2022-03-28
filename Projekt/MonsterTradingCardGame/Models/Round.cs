using System.Collections.Generic;

namespace MonsterTradingCardGame.Models
{
    public struct Round
    {
        public bool Draw { get; set; }
        public Player Loser { get; set; }
        public Player Winner { get; set; }
        public int RoundNumber { get; set; }

        public Round(int round)
        {
            Draw = false;
            Loser = null;
            Winner = null;
            RoundNumber = round;
        }
    }
}
