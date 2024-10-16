using System;

namespace CodeImp.Boss.TypeHandlers
{
	public class DateTimeHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.DateTime;
		public override Type? ClassType => typeof(DateTime);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			DateTime dt = (DateTime)value;
			writer.Write(dt.Ticks);
			writer.Write((byte)(int)dt.Kind);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			long ticks = reader.ReadInt64();
			byte kind = reader.ReadByte();
			return new DateTime(ticks, (DateTimeKind)(int)kind);
		}
	}
}
