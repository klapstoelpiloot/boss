namespace CodeImp.Boss.Tokens
{
	public class IntElement : BossElement
	{
		public int Value { get; private set; }

		public IntElement(string name, int value) : base(name)
		{
			Type = (byte)BossElementTypes.Int;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadInt32();
		}
	}
}
