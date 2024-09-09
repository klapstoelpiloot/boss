namespace CodeImp.Boss.TypeHandlers
{
	public class NullTypeHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossElementTypes.Null;

		public override IEnumerable<Type> ClassTypes => [];

		public override void WriteTo(BossWriter writer, object value)
		{
		}

		public override object? ReadFrom(BossReader reader, Type basetype)
		{
			return null;
		}
	}
}
