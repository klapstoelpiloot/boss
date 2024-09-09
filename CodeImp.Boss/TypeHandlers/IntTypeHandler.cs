namespace CodeImp.Boss.TypeHandlers
{
	public class IntTypeHandler : BossTypeHandler
	{
		public IntTypeHandler()
		{
			BossType = (byte)BossElementTypes.Int;
			ClassTypes = [typeof(int)];
		}

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((int)value);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return reader.ReadInt32();
		}
	}
}
