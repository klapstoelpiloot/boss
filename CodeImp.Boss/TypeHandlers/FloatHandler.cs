using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class FloatHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Float;
		public override Type? ClassType => typeof(float);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((float)value);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return reader.ReadSingle();
		}
	}
}
