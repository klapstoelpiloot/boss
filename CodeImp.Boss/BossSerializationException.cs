using System;

namespace CodeImp.Boss
{
    public class BossSerializationException : Exception
    {
        public BossSerializationException(string message, Exception innerException) : base(message, innerException) { }
        public BossSerializationException(string message) : base(message) { }
    }
}
