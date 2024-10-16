using System;

namespace CodeImp.Boss.TypeHandlers
{
	public class TimeSpanHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.TimeSpan;
		public override Type? ClassType => typeof(TimeSpan);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			TimeSpan ts = (TimeSpan)value;
			writer.Write(ts.Ticks);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			long ticks = reader.ReadInt64();
			return new TimeSpan(ticks);
		}
	}
}
