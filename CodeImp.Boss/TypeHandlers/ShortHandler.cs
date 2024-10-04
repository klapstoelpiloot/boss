using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class ShortHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Short;
		public override Type? ClassType => typeof(short);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((short)value);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			short v = reader.ReadInt16();
            if(basetype.IsEnum)
                return Enum.ToObject(basetype, v);
            else
                return v;
		}
	}
}
