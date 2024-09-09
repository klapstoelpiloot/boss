namespace CodeImp.Boss.TypeHandlers
{
	public class NullTypeHandler : BossTypeHandler
	{
		public NullTypeHandler()
		{
			BossType = (byte)BossElementTypes.Null;
			ClassTypes = [];
		}

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return null;
		}
	}
}
