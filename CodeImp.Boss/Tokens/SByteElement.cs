namespace CodeImp.Boss.Tokens
{
	public class SByteElement : BossElement
	{
		public sbyte Value { get; private set; }

		public SByteElement(string name, sbyte value) : base(name)
		{
			Type = (byte)BossElementTypes.SByte;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadSByte();
		}
	}
}
