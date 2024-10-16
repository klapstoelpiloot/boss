using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class NullHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Null;
		public override Type ClassType => null;

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return null;
		}
	}
}
