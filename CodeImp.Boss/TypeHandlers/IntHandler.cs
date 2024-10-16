using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class IntHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Int;
		public override Type ClassType => typeof(int);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((int)value);
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			int v = reader.ReadInt32();
            if(basetype.IsEnum)
                return Enum.ToObject(basetype, v);
            else
                return v;
		}
	}
}
