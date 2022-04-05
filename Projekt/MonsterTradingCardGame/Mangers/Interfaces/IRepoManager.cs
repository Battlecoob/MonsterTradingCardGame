using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Models
{
    public interface IRepoManager
    {
        public void CreateUser(Credentials credentials);
        public User LoginUser(Credentials credentials);
        public void CardExists(List<Card> cards);
        void AddCard(Card card);
        void CreatePackage(List<Card> cards);
        Package GetFirstPack();
        bool CheckCoinAmount( string authToken);
        void AcquirePackage(Package package,  string authToken);
        IEnumerable<Card> GetCards( string authToken);
        List<Card> GetCardsInDeck( string authToken);
        List<UserStats> GetTopElo();
        UserStats GetUserStats( string authToken);
        bool UserHasCard(string cardId,  string authToken);
        bool DeckExists( string authToken);
        void UpdateDeck( string authToken, List<string> cardIds);
        int CreateDeck( string authToken, List<string> cardIds);
        UserData GetUserData(string username);
        void UpdateUserData(string username, UserData userData);
        Card GetCardById(string cardId);
        Card GetCardByIdToken(string cardId,  string authToken);
        Deck GetDeck(string authToken);
        void TruncateDB();
        void UpdateWinnerStat(string authToken);
        void UpdateLoserStat(string authToken);
        void UpdateDrawStat(string authToken);
        public IEnumerable<Trade> CheckTradingDeals();
    }
}
