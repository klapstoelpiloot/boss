using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class ULongHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.ULong;
		public override Type ClassType => typeof(ulong);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((ulong)value);
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			ulong v = reader.ReadUInt64();
            if(basetype.IsEnum)
                return Enum.ToObject(basetype, v);
            else
                return v;
		}
	}
}
