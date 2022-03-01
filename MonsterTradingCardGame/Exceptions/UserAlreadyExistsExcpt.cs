using System;

namespace MonsterTradingCard.Exceptions
{
    public class UserAlreadyExistsExcpt : Exception
    {
        public UserAlreadyExistsExcpt() { }
        public UserAlreadyExistsExcpt(string message) : base(message) { }
        public UserAlreadyExistsExcpt(string message, Exception innerException) : base(message, innerException) { }
    }
}