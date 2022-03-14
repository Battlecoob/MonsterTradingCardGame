using MonsterTradingCardGame.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Models
{
    public class Player
    {
        public Card Card { get; set; }
        public float Damage { get; set; }
        public int CardsLeft { get; set; }
        public string Username { get; set; }
        public double ElementMultiplier { get; set; }
        public MonsterSpecialities MonsterSpeciality{ get; set; }
    }
}
