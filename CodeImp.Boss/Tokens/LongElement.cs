namespace CodeImp.Boss.Tokens
{
	public class LongElement : BossElement
	{
		public long Value { get; private set; }

		public LongElement(string name, long value) : base(name)
		{
			Type = (byte)BossElementTypes.Long;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadInt64();
		}
	}
}
