namespace CodeImp.Boss.Tokens
{
	public class IntTypeHandler : BossTypeHandler
	{
		public IntTypeHandler()
		{
			BossType = (byte)BossElementTypes.Int;
			ClassTypes = [typeof(int)];
		}

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			writer.Write((int)value);
		}

		public override object ReadFrom(BossReader reader)
		{
			return reader.ReadInt32();
		}
	}
}
