using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class UShortHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.UShort;
		public override Type? ClassType => typeof(ushort);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((ushort)value);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			ushort v = reader.ReadUInt16();
            if(basetype.IsEnum)
                return Enum.ToObject(basetype, v);
            else
                return v;
		}
	}
}
