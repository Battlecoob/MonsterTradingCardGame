using System;

namespace MonsterTradingCard.Exceptions
{
    public class UserDoesntExistExcpt : Exception
    {
        public UserDoesntExistExcpt() { }
        public UserDoesntExistExcpt(string message) : base(message) { }
        public UserDoesntExistExcpt(string message, Exception innerException) : base(message, innerException) { }
    }
}