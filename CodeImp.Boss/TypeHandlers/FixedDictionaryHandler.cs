using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class FixedDictionaryHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.FixedDictionary;
		public override Type ClassType => null;

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			throw new NotImplementedException();
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			throw new NotImplementedException();
		}
	}
}
