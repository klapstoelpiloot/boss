using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class BoolHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Bool;
		public override Type ClassType => typeof(bool);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((bool)value);
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return reader.ReadBoolean();
		}
	}
}
