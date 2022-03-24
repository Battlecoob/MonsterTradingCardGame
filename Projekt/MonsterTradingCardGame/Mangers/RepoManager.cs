using MonsterTradingCardGame.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Exceptions;

namespace MonsterTradingCardGame.Models
{
    public class RepoManager : IRepoManager
    {
        private readonly IUserRepo UserRepository;
        private readonly ICardRepo CardRepository;
        private readonly IPackageRepo PackageRepository;
        private readonly IDeckRepo DeckRepository;
        private readonly ITradingRepo TradeRepository;

        public RepoManager(IUserRepo userRepo, ICardRepo cardRepo, IPackageRepo packageRepo, IDeckRepo deckRepo, ITradingRepo tradeRepo)
        {
            UserRepository = userRepo;
            CardRepository = cardRepo;
            DeckRepository = deckRepo;
            TradeRepository = tradeRepo;
            PackageRepository = packageRepo;
        }

        public void TruncateDB()
        {
            UserRepository.TruncateTables();
        }

        public void CreateUser(Credentials credentials)
        {
            var user = new User()
            {
                Username = credentials.Username,
                Password = credentials.Password
            };

            if (!UserRepository.InsertUser(user))
                throw new UserAlreadyExistsExcpt();
        }

        public User LoginUser(Credentials credentials)
        {
            var user = UserRepository.GetUserByCredentials(credentials.Username, credentials.Password);

            if (user == null)
                throw new UserDoesntExistExcpt();

            return user;
        }

        public void CardExists(List<Card> cards)
        {
            foreach(Card card in cards)
            {
                if (CardRepository.SelectCardById(card.Id) != null) // card already exists and mustn't be added again
                    throw new CardAlreadyExistsExcpt(card.Id);
            }
        }

        public Card GetCardById(string cardId)
        {
            return CardRepository.SelectCardById(cardId);
        }

        public Card GetCardByIdToken(string cardId, string authToken)
        {
            return CardRepository.SelectCardByIdAndToken(cardId, authToken);
        }

        public void AddCard(Card card)
        {
            var newCard = new Card()
            {
                Id = card.Id,
                Name = card.Name,
                Damage = Convert.ToInt32(card.Damage)
            };

            var testCard = new List<Card>();
            testCard.Add(card);
            CardExists(testCard); // check if card is already in db

            CardRepository.InsertCard(newCard);
        }

        public void CreatePackage(List<Card> cards)
        {
            var package = new Package();
            List<string> cardIds = new List<string>();

            foreach (Card card in cards)
                cardIds.Add(card.Id);

            package.CardIds = cardIds;
            PackageRepository.InsertPackage(package);
        }

        public Package GetFirstPack()
        {
            return PackageRepository.GetFirstPack();
        }

        public bool CheckCoinAmount(string authToken)
        {
            int coins = UserRepository.SelectCoins(authToken);
            return coins >= 5; 
        }

        public void AcquirePackage(Package package, string authToken)
        {
            UserRepository.UpdateCoins(authToken);
            PackageRepository.UpdatePackageOwner(package.Id, authToken);
            
            foreach(string cardId in package.CardIds)
                CardRepository.UpdateCardOwner(cardId, authToken);
        }

        public IEnumerable<Card> GetCards(string authToken)
        {
            return CardRepository.SelectCardsByToken(authToken);
        }

        public List<Card> GetCardsInDeck(string authToken)
        {
            var deck = DeckRepository.SelectDeck(authToken);
            var cards = new List<Card>();

            if (deck == null)
                return cards;

            foreach (string cardId in deck.CardIds)
                cards.Add(CardRepository.SelectCardById(cardId));

            return cards;
        }

        public Deck GetDeck(string authToken)
        {
            var deck = DeckRepository.SelectDeck(authToken);

            if (deck != null && deck.CardIds.Count != 4)
                throw new InvalidDeckExcpt();

            return deck;
        }

        public bool UserHasCard(string cardId, string authToken)
        {
            if (CardRepository.SelectCardByIdAndToken(cardId, authToken) == null)
                return false;

            return true;
        }
        
        public bool DeckExists(string authToken)
        {
            if (DeckRepository.SelectDeck(authToken) == null)
                return false;

            return true;
        }

        public void UpdateDeck(string authToken, List<string> cardIds)
        {
            DeckRepository.UpdateDeck(authToken, cardIds);
        }

        public int CreateDeck(string authToken, List<string> cardIds)
        {
            return DeckRepository.InsertDeck(authToken, cardIds);
        }

        public UserData GetUserData(string username)
        {
            return UserRepository.SelectUserDataByUsername(username);
        }

        public void UpdateUserData(string username, UserData userData)
        {
            UserRepository.UpdateUserData(username, userData);
        }

        public UserStats GetUserStats(string authToken)
        {
            return UserRepository.SelectUserStats(authToken);
        }

        public List<UserStats> GetTopElo()
        {
            return UserRepository.GetTop10UserScore();
        }
    }
}
