namespace CodeImp.Boss.Tokens
{
	public class StringElement : BossElement
	{
		public string? Value { get; private set; }

		public StringElement(string name, string? value) : base(name)
		{
			Type = (byte)BossElementTypes.String;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadString();
		}
	}
}
