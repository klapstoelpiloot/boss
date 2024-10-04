using System;

namespace CodeImp.Boss
{
    internal class BossSerializationException : Exception
    {
        public BossSerializationException(string message, Exception innerException) : base(message, innerException) { }
        public BossSerializationException(string message) : base(message) { }
    }
}
