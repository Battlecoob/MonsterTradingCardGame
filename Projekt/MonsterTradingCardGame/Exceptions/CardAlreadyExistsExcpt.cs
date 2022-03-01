using System;

namespace MonsterTradingCard.Exceptions
{
    public class CardAlreadyExistsExcpt : Exception
    {
        public CardAlreadyExistsExcpt() { }
        public CardAlreadyExistsExcpt(string message) : base(message) { }
        public CardAlreadyExistsExcpt(string message, Exception innerException) : base(message, innerException) { }
    }
}