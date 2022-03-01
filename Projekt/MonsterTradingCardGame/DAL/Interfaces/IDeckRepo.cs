using MonsterTradingCardGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.DAL
{
    public interface IDeckRepo
    {
        Deck SelectDeck(string authToken);
        void UpdateDeck(string authToken, List<string> cardIds);
        int InsertDeck(string authToken, List<string> cardIds);
    }
}
