namespace CodeImp.Boss.Tokens
{
	public class FloatElement : BossElement
	{
		public float Value { get; private set; }

		public FloatElement(string name, float value) : base(name)
		{
			Type = (byte)BossElementTypes.Float;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadSingle();
		}
	}
}
