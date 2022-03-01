using MonsterTradingCardGame.Models;
using System.Collections.Generic;

namespace MonsterTradingCardGame.DAL
{
    public interface ITradingRepo
    {
        IEnumerable<Trading> SelectTrades();
        Trading SelectTradeById(string trade);
        Trading SelectTradeByCardId(string cardId);
        void InsertTrade(Trading trade, string authToken);
        void DeleteTradeByIdAndToken(string id, string authToken);
    }
}
