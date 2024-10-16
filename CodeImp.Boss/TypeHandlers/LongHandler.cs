﻿using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class LongHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Long;
		public override Type ClassType => typeof(long);

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((long)value);
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			long v = reader.ReadInt64();
            if(basetype.IsEnum)
                return Enum.ToObject(basetype, v);
            else
                return v;
		}
	}
}
