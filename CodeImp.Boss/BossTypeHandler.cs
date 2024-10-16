using System;

namespace CodeImp.Boss
{
    /// <summary>
    /// Handles a specific data type encountered during serialization/deserialization.
    /// Type handlers must be stateless and thread-safe. They are instantiated only once for a serializer.
    /// Internal type handlers are instantiated by the static constructor of BossSerializer.
    /// To add a custom handler, derive from this class and register with BossSerializer.RegisterTypeHandler(h).
    /// </summary>
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
        public abstract Type ClassType { get; }

        /// <summary>
        /// Called to serialized an object to stream.
        /// The data written must match exactly with the data read in the ReadFrom method.
        /// </summary>
        public abstract void WriteTo(BossSerializer serializer, BossWriter writer, object value);

        /// <summary>
        /// Called to deserialize an object from stream.
        /// The data read must match exactly with the written data in the WriteTo method.
        /// </summary>
        public abstract object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype);
    }
}
