using NUnit.Framework;
using MonsterTradingCardGame.DAL;
using MonsterTradingCardGame.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using MonsterTradingCard.Exceptions;

namespace UnitTests
{
    public class Tests
    {

        static Database testDb = new Database("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mctgdb"); // mctgdbtest (db just for testing) throws excpt at connection 
        static RepoManager testRepoManager = new RepoManager(testDb.UserRepository, testDb.CardRepository, testDb.PackageRepository, testDb.DeckRepository, testDb.TradeRepository);

        static private Package testPackage;
        static private List<Card> testCards;
        static private UserData testUserData;
        static private UserStats testUserStats;
        static private User testUser1, testUser2;
        static private Card testCard1, testCard2, testCard3, testCard4, testCard5, testCard6;
        static List<string> testCardIds = new List<string> { "1", "2", "3", "4", "5", "6" };

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

            testUser2 = new User
            {
                Username = "user2",
                Password = "password2"
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
                Name = "WaterGoblin",
                Damage = 10.0
            };

            testCard3 = new Card
            {
                Id = testCardIds[2],
                Name = "WaterGoblin",
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

            testCard6 = new Card
            {
                Id = testCardIds[5],
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
        }

        [Test] // 1
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

        [Test] // 2
        public void TestWrongCredentialsAtLogin()
        {
            var wrongtestCred = new Credentials
            {
                Username = "wrong",
                Password = "wrong"
            };

            Assert.Catch<UserDoesntExistExcpt>(() => testRepoManager.LoginUser(wrongtestCred));
        }

        [Test] // 3
        public void TestAddDuplicateCard()
        {
            TruncateDatabase();

            testRepoManager.AddCard(testCard1);
            Assert.Catch<CardAlreadyExistsExcpt>(() => testRepoManager.AddCard(testCard1));
        }

        [Test] // 4
        public void TestAddCard()
        {
            TruncateDatabase();

            testRepoManager.AddCard(testCard1);
            var testCard = testRepoManager.GetCardById(testCard1.Id);
            Assert.AreEqual(JsonConvert.SerializeObject(testCard), JsonConvert.SerializeObject(testCard1));
        }

        [Test] // 5
        public void TestCheckCoinAmountOfUser()
        {
            TruncateDatabase();

            var testCred = new Credentials
            {
                Username = "user1",
                Password = "password1"
            };

            testRepoManager.CreateUser(testCred);
            var user = testRepoManager.LoginUser(testCred);
            Assert.IsTrue(testRepoManager.CheckCoinAmount(user.Token));
        }

        [Test] // 6
        public void TestCreatePackage()
        {
            TruncateDatabase();

            foreach (Card card in testCards)
                testRepoManager.AddCard(card);

            testRepoManager.CreatePackage(testCards);
            var pack = testRepoManager.GetFirstPack();

            Assert.AreEqual(JsonConvert.SerializeObject(pack), JsonConvert.SerializeObject(testPackage));
        }

        [Test] // 7
        public void TestAcquirePackage()
        {
        }

        [Test] // 8
        public void TestBuyPackageWithoutCoins()
        {
        }

        [Test] // 9
        public void TestAcquirePackage()
        {
        }

        [Test] // 10
        public void TestDeckCreation()
        {
        }

        [Test] // 11
        public void TestUpdateDeck()
        {
        }

        [Test] // 12
        public void TestShowDeck()
        {
        }

        [Test] // 13
        public void TestIfDeckExists()
        {
        }

        [Test] // 14
        public void TestShowUserData()
        {
        }

        [Test] // 15
        public void TestUpdateUserData()
        {
        }
    }
}