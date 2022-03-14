using MonsterTradingCardGame.Models;
using System.Collections.Generic;

namespace MonsterTradingCardGame.DAL
{
    public interface ICardRepo
    {
        void InsertCard(Card card);
        Card SelectCardById(string cardId);
        void UpdateCardOwner(string cardId, string authToken);
        IEnumerable<Card> SelectCardsByToken(string username);
        Card SelectCardByIdAndToken(string cardId, string authToken);
    }
}
