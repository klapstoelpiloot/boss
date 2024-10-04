using System;
using System.IO;
using System.IO.Compression;

namespace CodeImp.Boss
{
    /// <summary>
    /// Convenience class with simplified methods for most common (de)serialization actions.
    /// </summary>
    public class BossConvert
    {
        /// <summary>
        /// Serializes the given object to the specified stream.
        /// This does not close or dispose the stream.
        /// </summary>
        public static void ToStream<T>(T obj, Stream stream, bool compress = false)
        {
            BossSerializer s = new BossSerializer();
            if (compress)
            {
                using MemoryStream memstream = new MemoryStream();
                s.Serialize(obj, memstream);
                using DeflateStream zipstream = new DeflateStream(stream, CompressionMode.Compress, true);
                memstream.WriteTo(zipstream);
                zipstream.Flush();
                stream.Flush();
            }
            else
            {
                s.Serialize(obj, stream);
            }
        }

        /// <summary>
        /// Serializes the given object to an array of bytes.
        /// </summary>
        public static byte[] ToBytes<T>(T obj, bool compress = false)
        {
            using MemoryStream stream = new MemoryStream();
            ToStream(obj, stream, compress);
            return stream.ToArray();
        }

        /// <summary>
        /// Serializes the given object as Base64 string.
        /// </summary>
        public static string ToBase64<T>(T obj, bool compress = false)
        {
            return Convert.ToBase64String(ToBytes(obj, compress));
        }

        /// <summary>
        /// Deserializes an object from the specified stream.
		/// This does not close or dispose the stream.
		/// </summary>
		public static T? FromStream<T>(Stream stream, bool decompress = false)
        {
            BossSerializer s = new BossSerializer();
            if (decompress)
            {
                using MemoryStream memstream = new MemoryStream();
                using DeflateStream zipstream = new DeflateStream(stream, CompressionMode.Decompress, true);
                zipstream.CopyTo(memstream);
                memstream.Seek(0, SeekOrigin.Begin);
                return s.Deserialize<T>(memstream);
            }
            else
            {
                return s.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// Deserializes an object from the specified byte array.
        /// </summary>
        public static T? FromBytes<T>(byte[] data, bool decompress = false)
        {
            using MemoryStream stream = new MemoryStream(data);
            return FromStream<T>(stream, decompress);
        }

        /// <summary>
        /// Deserializes an object from the specified Base64 string.
        /// </summary>
        public static T? FromBase64<T>(string str, bool decompress = false)
        {
            byte[] data = Convert.FromBase64String(str);
            return FromBytes<T>(data, decompress);
        }
    }
}
