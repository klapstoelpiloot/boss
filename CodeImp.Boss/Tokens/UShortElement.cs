namespace CodeImp.Boss.Tokens
{
	public class UShortElement : BossElement
	{
		public ushort Value { get; private set; }

		public UShortElement(string name, ushort value) : base(name)
		{
			Type = (byte)BossElementTypes.UShort;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadUInt16();
		}
	}
}
