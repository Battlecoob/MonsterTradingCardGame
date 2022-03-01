using MonsterTradingCardGame.Models;
using System.Collections.Generic;

namespace MonsterTradingCardGame.DAL
{
    public interface ITradingRepo
    {
        Trading SelectTradeByCardId(string cardId);
        IEnumerable<Trading> SelectOpenTrades();
        Trading SelectTradeAndTokenById(string trade);
        int InsertTrade(Trading trade, string authToken);
        int DeleteTradeByIdAndToken(string id, string authToken);
    }
}
