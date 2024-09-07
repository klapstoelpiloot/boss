using CodeImp.Boss.Tokens;

namespace CodeImp.Boss
{
	public class BossReader : BinaryReader
	{
		private readonly List<string?> stringstable = new List<string?>();

		public BossReader(Stream input) : base(input)
		{
		}

		public void BeginReading()
		{
			// First read the strings table
			long strpos = ReadInt64();
			BaseStream.Seek(strpos, SeekOrigin.Begin);
			int numstrings = ReadVLQ();
			stringstable.Capacity = Math.Max(numstrings + 1, stringstable.Capacity);
			stringstable.Add(null);
			for(int i = 0; i < numstrings; i++)
			{
				string str = base.ReadString();
				stringstable.Add(str);
			}

			// Jump back to begin reading elements
			BaseStream.Seek(sizeof(long), SeekOrigin.Begin);
		}

		public FixedObjectElement ReadRootElement()
		{
			FixedObjectElement e = new FixedObjectElement(string.Empty);
			e.ReadFromStream(this);
			return e;
		}

		public BossElement ReadNextElement()
		{
			byte type = ReadByte();
			string name = ReadString();

			// TODO: Make a BossElementFactory that poops out a specific element with this information

		}

		// We do strings differently
		new public string? ReadString()
		{
			int index = ReadVLQ();
			return stringstable[index];
		}

		public int ReadVLQ()
		{
			return Read7BitEncodedInt();
		}
	}
}
