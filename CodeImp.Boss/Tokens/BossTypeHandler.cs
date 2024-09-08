namespace CodeImp.Boss.Tokens
{
	public abstract class BossTypeHandler
	{
		public byte BossType { get; protected set; }

		public IEnumerable<Type> ClassTypes { get; protected set; }

		// Constructor
		protected BossTypeHandler()
		{
		}

		public abstract void WriteTo(BossSerializer serializer, BossWriter writer, object value);
		public abstract object ReadFrom(BossReader reader);
	}
}
