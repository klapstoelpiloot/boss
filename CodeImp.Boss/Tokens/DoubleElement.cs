namespace CodeImp.Boss.Tokens
{
	public class DoubleElement : BossElement
	{
		public double Value { get; private set; }

		public DoubleElement(string name, double value) : base(name)
		{
			Type = (byte)BossElementTypes.Double;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadDouble();
		}
	}
}
