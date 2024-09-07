namespace CodeImp.Boss.Tokens
{
	public class UIntElement : BossElement
	{
		public uint Value { get; private set; }

		public UIntElement(string name, uint value) : base(name)
		{
			Type = (byte)BossElementTypes.UInt;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadUInt32();
		}
	}
}
