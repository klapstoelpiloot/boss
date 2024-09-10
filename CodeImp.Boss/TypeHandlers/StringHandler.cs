namespace CodeImp.Boss.TypeHandlers
{
    public class StringHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.String;

		public override IEnumerable<Type> ClassTypes => [typeof(string)];

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
