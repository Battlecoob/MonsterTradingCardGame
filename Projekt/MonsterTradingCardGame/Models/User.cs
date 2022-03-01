using SWE1HttpServer.Core.Authentication;

namespace MonsterTradingCardGame.Models
{
    public class User : IIdentity
    {
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Draws { get; set; }
        public int Coins { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public float Winrate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Token => $"{Username}-mtcgToken";
    }
}
