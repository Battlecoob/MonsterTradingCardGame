using System;

namespace MonsterTradingCardGame.Exceptions
{
    public class InvalidDeckExcpt : Exception
    {
        public InvalidDeckExcpt() { }
        public InvalidDeckExcpt(string message) : base(message) { }
        public InvalidDeckExcpt(string message, Exception innerException) : base(message, innerException) { }
    }
}
