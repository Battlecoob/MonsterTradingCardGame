using NUnit.Framework;
using MonsterTradingCardGame.DAL;
using MonsterTradingCardGame.Managers;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Models.Enums;
using System.Collections.Generic;
using Newtonsoft.Json;
using MonsterTradingCardGame.Exceptions;
using System.Linq;

namespace UnitTests
{
    public class Tests
    {
        // 'mctgdbtest' (db just for testing) instead of 'mctgdb' throws excpt at connection; couldn't figure out why
        static Database testDb = new Database("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mtcg");


        /* AENDERN! */
        static List<string> deck1CardIds = new List<string> { "deck1-card1", "deck1-card2", "deck1-card3", "deck1-card4" };
        static Deck deck1 = new Deck { Id = 1, Token = "username-1-mtcgToken", CardIds = deck1CardIds };
        static string username1 = "username-1";
        static List<string> deck2CardIds = new List<string> { "deck2-card1", "deck2-card2", "deck2-card3", "deck2-card4" };
        static Deck deck2 = new Deck { Id = 2, Token = "username-2-mtcgToken", CardIds = deck2CardIds };
        static string username2 = "username-2";

        static RepoManager testRepoManager = new RepoManager(testDb.UserRepository, testDb.CardRepository, testDb.PackageRepository, testDb.DeckRepository, testDb.TradeRepository);
        static BattleManager testBattleManager = new BattleManager(testRepoManager, deck1, username1, deck2, username2);

        static private Package testPackage;
        static private List<Card> testCards;
        static private UserData testUserData;
        static private UserStats testUserStats;
        static private User testUser1;
        static private Card testCard1, testCard2, testCard3, testCard4, testCard5;
        static List<string> testCardIds = new List<string> { "1", "2", "3", "4", "5", "6" };
        static Credentials testCred;

        static private void TruncateDatabase()
        {
            testDb.UserRepository.TruncateTables();
        }

        [SetUp]
        public void Setup()
        {
            testUser1 = new User
            {
                Username = "user1",
                Password = "password1"
            };

            testUserData = new UserData
            {
                Name = "name",
                Bio = "bio",
                Image = "img"
            };

            testUserStats = new UserStats
            {
                Elo = 100,
                Wins = 0,
                Draws = 0,
                Losses = 0,
                Username = testUser1.Username
            };

            testCard1 = new Card
            {
                Id = testCardIds[0],
                Name = "WaterGoblin",
                Damage = 10.0
            };

            testCard2 = new Card
            {
                Id = testCardIds[1],
                Name = "FireElf",
                Damage = 10.0
            };

            testCard3 = new Card
            {
                Id = testCardIds[2],
                Name = "Ork",
                Damage = 10.0
            };

            testCard4 = new Card
            {
                Id = testCardIds[3],
                Name = "WaterGoblin",
                Damage = 10.0
            };

            testCard5 = new Card
            {
                Id = testCardIds[4],
                Name = "WaterGoblin",
                Damage = 10.0
            };

            testCards = new List<Card>
            {
                testCard1, testCard2, testCard3, testCard4, testCard5
            };

            testPackage = new Package
            {
                Id = 1,
                CardIds = new List<string> { testCardIds[0], testCardIds[1], testCardIds[2], testCardIds[3], testCardIds[4] }
            };

            testCred = new Credentials
            {
                Username = testUser1.Username,
                Password = testUser1.Password
            };
        }

        [Test]
        public void TestRegisterUser()
        {
            TruncateDatabase();

            var testCred = new Credentials
            {
                Username = "user1",
                Password = "password1"
            };

            testRepoManager.CreateUser(testCred);
            var user = testRepoManager.LoginUser(testCred);

            Assert.AreEqual(JsonConvert.SerializeObject(user), JsonConvert.SerializeObject(testUser1));
        }

        [Test]
        public void TestWrongCredentialsAtLogin()
        {
            var wrongtestCred = new Credentials
            {
                Username = "wrong",
                Password = "wrong"
            };

            Assert.Catch<UserDoesntExistExcpt>(() => testRepoManager.LoginUser(wrongtestCred));
        }

        [Test]
        public void TestAddDuplicateCard()
        {
            TruncateDatabase();

            testRepoManager.AddCard(testCard1);

            Assert.Catch<CardAlreadyExistsExcpt>(() => testRepoManager.AddCard(testCard1));
        }

        [Test]
        public void TestAddCard()
        {
            TruncateDatabase();

            testRepoManager.AddCard(testCard1);
            var testCard = testRepoManager.GetCardById(testCard1.Id);

            Assert.AreEqual(JsonConvert.SerializeObject(testCard), JsonConvert.SerializeObject(testCard1));
        }

        [Test]
        public void TestCheckCoinAmountOfUser()
        {
            TruncateDatabase();

            testRepoManager.CreateUser(testCred);

            var user = testRepoManager.LoginUser(testCred);

            Assert.IsTrue(testRepoManager.CheckCoinAmount(user.Token));
        }

        [Test]
        public void TestCreatePackage()
        {
            TruncateDatabase();

            foreach (Card card in testCards)
                testRepoManager.AddCard(card);

            testRepoManager.CreatePackage(testCards);

            var pack = testRepoManager.GetFirstPack();

            Assert.AreEqual(JsonConvert.SerializeObject(pack), JsonConvert.SerializeObject(testPackage));
        }

        [Test]
        public void TestAcquirePackage()
        {
            TruncateDatabase();

            foreach (Card card in testCards)
                testRepoManager.AddCard(card);


            testRepoManager.CreateUser(testCred);
            testRepoManager.CreatePackage(testCards);
            testRepoManager.AcquirePackage(testPackage, testUser1.Token);

            var cardIds = testRepoManager.GetCards(testUser1.Token).Select(c => c.Id);

            Assert.AreEqual(JsonConvert.SerializeObject(testPackage.CardIds), JsonConvert.SerializeObject(cardIds));
        }

        [Test]
        public void GetCardByIdNoCard()
        {
            TruncateDatabase();

            var testCard = testRepoManager.GetCardById(testCard1.Id);

            Assert.AreEqual(JsonConvert.SerializeObject(testCard), JsonConvert.SerializeObject(null));
        }

        [Test]
        public void TestDeckCreation()
        {
            TruncateDatabase();

            foreach (Card card in testCards)
                testRepoManager.AddCard(card);

            testRepoManager.CreateUser(testCred);
            testRepoManager.CreatePackage(testCards);
            testRepoManager.AcquirePackage(testPackage, testUser1.Token);

            var cardIds = testRepoManager.GetCards(testUser1.Token).Select(c => c.Id).ToList();
            cardIds.RemoveAt(4);
            
            testRepoManager.CreateDeck(testUser1.Token, cardIds);
            
            var deckCards = testRepoManager.GetCardsInDeck(testUser1.Token).Select(c => c.Id);

            Assert.AreEqual(JsonConvert.SerializeObject(deckCards), JsonConvert.SerializeObject(cardIds));
        }

        [Test]
        public void TestUpdateDeck()
        {
            TruncateDatabase();

            foreach (Card card in testCards)
                testRepoManager.AddCard(card);

            testRepoManager.CreateUser(testCred);
            testRepoManager.CreatePackage(testCards);
            testRepoManager.AcquirePackage(testPackage, testUser1.Token);

            var cardIds = testRepoManager.GetCards(testUser1.Token).Select(c => c.Id).ToList();
            cardIds.RemoveAt(4);

            testRepoManager.CreateDeck(testUser1.Token, cardIds.ToList());
            testRepoManager.UpdateDeck(testUser1.Token, cardIds.ToList());
            
            var deckCards = testRepoManager.GetCardsInDeck(testUser1.Token).Select(c => c.Id);

            Assert.AreEqual(JsonConvert.SerializeObject(deckCards), JsonConvert.SerializeObject(cardIds));
        }

        [Test]
        public void TestShowDeckWithoutDeck()
        {
            TruncateDatabase();

            foreach (Card card in testCards)
                testRepoManager.AddCard(card);

            testRepoManager.CreateUser(testCred);

            var emptyList = testRepoManager.GetCardsInDeck(testUser1.Token);

            Assert.AreEqual(JsonConvert.SerializeObject(emptyList), JsonConvert.SerializeObject(new List<Card>()));
        }

        [Test]
        public void TestIfDeckExistsNegative()
        {
            TruncateDatabase();

            testRepoManager.CreateUser(testCred);

            var exists = testRepoManager.DeckExists(testUser1.Token);

            Assert.AreEqual(exists, false);
        }

        [Test]
        public void TestIfDeckExistsPositiv()
        {
            TruncateDatabase();

            foreach (Card card in testCards)
                testRepoManager.AddCard(card);

            testRepoManager.CreateUser(testCred);
            testRepoManager.CreatePackage(testCards);
            testRepoManager.AcquirePackage(testPackage, testUser1.Token);

            var cardIds = testRepoManager.GetCards(testUser1.Token).Select(c => c.Id);
            testRepoManager.CreateDeck(testUser1.Token, cardIds.ToList());

            var exists = testRepoManager.DeckExists(testUser1.Token);

            Assert.AreEqual(exists, true);
        }

        [Test]
        public void TestShowUserNewUser()
        {
            TruncateDatabase();

            testRepoManager.CreateUser(testCred);

            var data = testRepoManager.GetUserData(testUser1.Token);

            Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(null));
        }

        [Test]
        public void TestShowUserAfterDataUpdate()
        {
            TruncateDatabase();

            testRepoManager.CreateUser(testCred);
            
            var username = testUser1.Token.Split('-').FirstOrDefault();
            testRepoManager.UpdateUserData(username, testUserData);

            var data = testRepoManager.GetUserData(username);

            Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(testUserData));
        }

        [Test]
        public void TestUserHasCardNegative()
        {
            TruncateDatabase();

            testRepoManager.CreateUser(testCred);

            var hasCard = testRepoManager.UserHasCard(testCard1.Id, testUser1.Token);

            Assert.AreEqual(hasCard, false);
        }

        [Test]
        public void TestUserHasCardPositive()
        {
            TruncateDatabase();

            foreach (Card card in testCards)
                testRepoManager.AddCard(card);

            testRepoManager.CreateUser(testCred);
            testRepoManager.CreatePackage(testCards);
            testRepoManager.AcquirePackage(testPackage, testUser1.Token);

            var hasCard = testRepoManager.UserHasCard(testPackage.CardIds[0], testUser1.Token);

            Assert.AreEqual(hasCard, true);
        }

        [Test]
        public void TestShowTopElo()
        {
            TruncateDatabase();

            testRepoManager.CreateUser(testCred);
            var list = new List<UserStats>();
            list.Add(testUserStats);

            var data = testRepoManager.GetTopElo();

            Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(list));
        }

        [Test]
        public void TestGetTopEloWithoutUser()
        {
            TruncateDatabase();

            var data = testRepoManager.GetTopElo();

            Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(new List<UserStats>()));
        }

        [Test]
        public void TestShowUserStats()
        {
            TruncateDatabase();

            testRepoManager.CreateUser(testCred);
            var data = testRepoManager.GetUserStats(testUser1.Token);

            Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(testUserStats));
        }

        [Test]
        [TestCase("Dragon", "WaterGoblin", ExpectedResult = Specialities.none )]
        [TestCase("WaterGoblin", "Dragon", ExpectedResult = Specialities.afraid )]
        [TestCase("Wizzard", "Ork", ExpectedResult = Specialities.none )]
        [TestCase("Ork", "Wizzard", ExpectedResult = Specialities.controlled )]
        [TestCase("WaterSpell", "Knight", ExpectedResult = Specialities.none )]
        [TestCase("Knight", "WaterSpell", ExpectedResult = Specialities.drowned )]
        [TestCase("Kraken", "FireSpell", ExpectedResult = Specialities.immune )]
        [TestCase("Kraken", "WaterSpell", ExpectedResult = Specialities.immune )]
        [TestCase("FireSpell", "Kraken", ExpectedResult = Specialities.none )]
        [TestCase("Kraken", "RegularSpell", ExpectedResult = Specialities.immune )]
        [TestCase("WaterSpell", "Kraken", ExpectedResult = Specialities.none )]
        [TestCase("RegularSpell", "Kraken", ExpectedResult = Specialities.none )]
        [TestCase("Dragon", "FireElf", ExpectedResult = Specialities.none )]
        [TestCase("FireElf", "Dragon", ExpectedResult = Specialities.friend )]
        public Specialities TestSpecialities(string card1Name, string card2Name)
        {
            var card1 = new Card { Name = card1Name };
            var card2 = new Card { Name = card2Name };
            Specialities speciality = testBattleManager.CalculateSpeciality(card1, card2);

            return speciality;
        }

        [Test]
        [TestCase(Element.fire, ExpectedResult = (double) 2f)]
        [TestCase(Element.normal, ExpectedResult = (double) 0.5f)]
        [TestCase(Element.water, ExpectedResult = (double) 1f)]
        public double TestBattleElementMultiplierWater(Element element)
        {
            return testCard1.CalculateElementMultiplicator(element);
        }

        [Test]
        [TestCase(Element.fire, ExpectedResult = (double) 1f)]
        [TestCase(Element.normal, ExpectedResult = (double) 2f)]
        [TestCase(Element.water, ExpectedResult = (double) 0.5f)]
        public double TestBattleElementMultiplierFire(Element element)
        {
            return testCard2.CalculateElementMultiplicator(element);
        }

        [Test]
        [TestCase(Element.fire, ExpectedResult = (double) 0.5f)]
        [TestCase(Element.normal, ExpectedResult = (double) 1f)]
        [TestCase(Element.water, ExpectedResult = (double) 2f)]
        public double TestBattleElementMultiplierNormal(Element element)
        {
            return testCard3.CalculateElementMultiplicator(element);
        }
    }
}
