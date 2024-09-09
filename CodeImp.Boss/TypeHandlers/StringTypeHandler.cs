namespace CodeImp.Boss.TypeHandlers
{
	public class StringTypeHandler : BossTypeHandler
	{
		public StringTypeHandler()
		{
			BossType = (byte)BossElementTypes.String;
			ClassTypes = [typeof(string)];
		}

		public override void WriteTo(BossWriter writer, object value)
		{
			writer.Write((string?)value);
		}

		public override object? ReadFrom(BossReader reader, Type basetype)
		{
			return reader.ReadString();
		}
	}
}
