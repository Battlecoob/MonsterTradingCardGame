using MonsterTradingCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Models
{
    interface IRepoManager
    {
        public void OpenConn();
        public void CloseConn();
        public void CreateUser(Credentials credentials);
        public User LoginUser(Credentials credentials);
        public void CardExists(List<Card> cards);
        void AddCard(Card card);
        void CreatePackage(List<Card> cards);
        Package GetFirstPack();
        bool CheckCoinAmount(string token);
        void AcquirePackage(Package package, string token);
        IEnumerable<Card> GetCards(string token);
        List<Card> GetCardsInDeck(string token);
        List<UserStats> GetTopElo();
        UserStats GetUserStats(string token);
        bool UserHasCard(string cardId, string token);
        bool DeckExists(string token);
        void UpdateDeck(string token, List<string> cardIds);
        int CreateDeck(string token, List<string> cardIds);
        UserData GetUserData(string username);
        void UpdateUserData(string username, UserData userData);
        Card GetCardById(string cardId);
    }
}
