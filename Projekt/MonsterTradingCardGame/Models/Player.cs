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

        public Player(string name, Card card)
        {
            // -1 as default values -> need to be changed during battle

            Card = card;
            Damage = -1;
            CardsLeft = -1;
            Username = name;
            ElementMultiplier = -1;
            MonsterSpeciality = MonsterSpecialities.none;
        }
    }
}
