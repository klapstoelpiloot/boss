namespace CodeImp.Boss.TypeHandlers
{
    public class ByteHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Byte;
		public override Type? ClassType => typeof(byte);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((byte)value);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return reader.ReadByte();
		}
	}
}
