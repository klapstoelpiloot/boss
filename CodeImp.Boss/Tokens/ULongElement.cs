namespace CodeImp.Boss.Tokens
{
	public class ULongElement : BossElement
	{
		public ulong Value { get; private set; }

		public ULongElement(string name, ulong value) : base(name)
		{
			Type = (byte)BossElementTypes.ULong;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadUInt64();
		}
	}
}
