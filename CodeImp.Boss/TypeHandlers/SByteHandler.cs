using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class SByteHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.SByte;
		public override Type ClassType => typeof(sbyte);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((sbyte)value);
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			sbyte v = reader.ReadSByte();
            if(basetype.IsEnum)
                return Enum.ToObject(basetype, v);
            else
                return v;
		}
	}
}
