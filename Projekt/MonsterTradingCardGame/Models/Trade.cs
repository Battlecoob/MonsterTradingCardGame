using MonsterTradingCardGame.Models.Enums;

namespace MonsterTradingCardGame.Models
{
    public class Trading
    {
        public string Id { get; set; }
        public int? MinDmg { get; set; }
        public string Token { get; set; }
        public CardType? Type { get; set; }
        public Element? Element { get; set; }
        public Species? Species { get; set; }
        public string Card2Trade { get; set; }
    }
}
