namespace CodeImp.Boss
{
    public abstract class BossTypeHandler
    {
        /// <summary>
        /// This is the Boss typecode that this handler will deal with.
        /// Read the README.md and see the BossTypeCode enum for more information.
        /// </summary>
        public abstract byte BossType { get; }

        /// <summary>
        /// .NET class type that this handler will deal with.
        /// </summary>
        public abstract Type? ClassType { get; }

        /// <summary>
        /// Called to serialized an object to stream.
        /// </summary>
        public abstract void WriteTo(BossSerializer serializer, BossWriter writer, object value);

        /// <summary>
        /// Called to deserialize an object from stream.
        /// </summary>
        public abstract object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype);
    }
}
