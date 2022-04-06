using MonsterTradingCardGame.Models;
using System.Collections.Generic;

namespace MonsterTradingCardGame.DAL
{
    public interface ITradingRepo
    {
        IEnumerable<Trade> SelectTrades();
        Trade SelectTradeById(string trade);
        Trade SelectTradeByCardId(string cardId);
        int InsertTrade(Trade trade, string authToken);
        void DeleteTradeByIdAndToken(string id, string authToken);
    }
}
