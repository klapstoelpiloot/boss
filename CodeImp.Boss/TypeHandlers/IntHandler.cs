﻿namespace CodeImp.Boss.TypeHandlers
{
    public class IntHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.Int;

		public override Type ClassType => typeof(int);

		public override void WriteTo(BossWriter writer, object value)
		{
			writer.Write((int)value);
		}

		public override object? ReadFrom(BossReader reader, Type basetype)
		{
			return reader.ReadInt32();
		}
	}
}
