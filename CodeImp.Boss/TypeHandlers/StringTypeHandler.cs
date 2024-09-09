namespace CodeImp.Boss.TypeHandlers
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

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return reader.ReadString();
		}
	}
}
