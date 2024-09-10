namespace CodeImp.Boss.TypeHandlers
{
    public class FloatHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Float;

		public override Type? ClassType => typeof(float);

		public override void WriteTo(BossWriter writer, object value)
		{
			writer.Write((float)value);
		}

		public override object? ReadFrom(BossReader reader, Type basetype)
		{
			return reader.ReadSingle();
		}
	}
}
