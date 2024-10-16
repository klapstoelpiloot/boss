using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeImp.Boss
{
	public class BossWriter : BinaryWriter
	{
		private readonly Dictionary<string, int> stringstable = new Dictionary<string, int>();

		// Constructor
		public BossWriter(Stream stream) : base(stream, Encoding.UTF8, true)
		{
		}

		// Begin writing
		public void BeginWriting()
		{
			// Write a dummy value where later the location of the stringtable will be written
			Write((long)0);
		}

		public void EndWriting()
		{
			// Remember the position so we know where the strings table is going to be
			Flush();
			long strpos = BaseStream.Position;

			// Append the strings table
			WriteVLQ(stringstable.Count);
			foreach(KeyValuePair<string, int> str in stringstable)
			{
				base.Write(str.Key);
			}

			// Go back to the start to write the string table address
			Flush();
			BaseStream.Seek(0, SeekOrigin.Begin);
			Write(strpos);
			Flush();
			BaseStream.Seek(strpos, SeekOrigin.End);
		}

		// We do strings differently
		new public void Write(string value)
		{
			int index = StoreString(value);
			WriteVLQ(index);
		}

		/// <summary>
		/// Writes a variable length quantity to the stream. The number of bytes written depends on the given quantity.
		/// For more information see https://en.m.wikipedia.org/wiki/Variable-length_quantity
		/// </summary>
		public void WriteVLQ(int value)
		{
			Write7BitEncodedInt(value);
		}

		// Puts a string in the strings table and returns its index
		private int StoreString(string s)
		{
			if(s == null)
				return 0;

			if(!stringstable.TryGetValue(s, out int value))
			{
				// This string table does not include the null string, so we count + 1
				value = stringstable.Count + 1;
				stringstable.Add(s, value);
			}

			return value;
		}
	}
}
