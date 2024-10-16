using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class UIntHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.UInt;
		public override Type ClassType => typeof(uint);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((uint)value);
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			uint v = reader.ReadUInt32();
            if(basetype.IsEnum)
                return Enum.ToObject(basetype, v);
            else
                return v;
		}
	}
}
