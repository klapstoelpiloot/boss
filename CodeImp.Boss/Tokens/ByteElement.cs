namespace CodeImp.Boss.Tokens
{
	public class ByteElement : BossElement
	{
		public byte Value { get; private set; }

		public ByteElement(string name, byte value) : base(name)
		{
			Type = (byte)BossElementTypes.Byte;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadByte();
		}
	}
}
