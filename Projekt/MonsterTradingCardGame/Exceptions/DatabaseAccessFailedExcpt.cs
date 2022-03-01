using System;

namespace MonsterTradingCard.Exceptions
{
    public class DatabaseAccessFailedExcpt : Exception
    {
        public DatabaseAccessFailedExcpt() { }
        public DatabaseAccessFailedExcpt(string message) : base(message) { }
        public DatabaseAccessFailedExcpt(string message, Exception innerException) : base(message, innerException) { }
    }
}