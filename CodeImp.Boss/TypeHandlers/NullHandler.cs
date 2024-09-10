namespace CodeImp.Boss.TypeHandlers
{
    public class NullHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Null;

		public override Type? ClassType => null;

		public override void WriteTo(BossWriter writer, object value)
		{
		}

		public override object? ReadFrom(BossReader reader, Type basetype)
		{
			return null;
		}
	}
}
