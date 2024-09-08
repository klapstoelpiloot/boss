namespace CodeImp.Boss.Tokens
{
	public class StringTypeHandler : BossTypeHandler
	{
		public StringTypeHandler()
		{
			BossType = (byte)BossElementTypes.String;
			ClassTypes = [typeof(string)];
		}

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((string?)value);
		}

		public override object ReadFrom(BossReader reader)
		{
			return reader.ReadString();
		}
	}
}
