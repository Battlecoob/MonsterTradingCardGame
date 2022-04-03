using System;

namespace MonsterTradingCardGame.Exceptions
{
    public class InternalPlayerErrorExcpt : Exception
    {
        public InternalPlayerErrorExcpt() { }
        public InternalPlayerErrorExcpt(string message) : base(message) { }
        public InternalPlayerErrorExcpt(string message, Exception innerException) : base(message, innerException) { }
    }
}