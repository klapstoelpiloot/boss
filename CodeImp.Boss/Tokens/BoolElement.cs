
namespace CodeImp.Boss.Tokens
{
	public class BoolElement : BossElement
	{
		public bool Value { get; private set; }

		public BoolElement(string name, bool value) : base(name)
		{
			Type = (byte)BossElementTypes.Bool;
			Value = value;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.Write(Value);
		}

		protected override void ReadFrom(BossReader reader)
		{
			Value = reader.ReadBoolean();
		}
	}
}
