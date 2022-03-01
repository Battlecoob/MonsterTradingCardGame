namespace MonsterTradingCardGame.Models
{
    public class UserStats
    {
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public string Username { get; set; }
    }
}
