namespace CodeImp.Boss.Tokens
{
	public class NullTypeHandler : BossTypeHandler
	{
		public NullTypeHandler()
		{
			BossType = (byte)BossElementTypes.Null;
			ClassTypes = [];
		}

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
		}

		public override object ReadFrom(BossReader reader)
		{
			return new object();
		}
	}
}
