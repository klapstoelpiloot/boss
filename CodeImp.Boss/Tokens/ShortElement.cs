namespace CodeImp.Boss.Tokens
{
	public class ShortElement : BossElement
	{
		public short Value { get; private set; }

		public ShortElement(string name, short value) : base(name)
		{
			Type = (byte)BossElementTypes.Short;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadInt16();
		}
	}
}
