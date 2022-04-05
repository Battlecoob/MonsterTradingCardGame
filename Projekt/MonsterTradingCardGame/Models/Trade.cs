using MonsterTradingCardGame.Models.Enums;

namespace MonsterTradingCardGame.Models
{
    public class Trade
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public CardType? Type { get; set; }
        public Element? Element { get; set; }
        public Species? Species { get; set; }
        public int? MinimumDamage { get; set; }
        public string CardToTrade { get; set; }
    }
}
