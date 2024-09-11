using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class DoubleHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Double;
		public override Type? ClassType => typeof(double);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((double)value);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return reader.ReadDouble();
		}
	}
}
