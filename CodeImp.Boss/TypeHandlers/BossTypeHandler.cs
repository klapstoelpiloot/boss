namespace CodeImp.Boss.TypeHandlers
{
	public abstract class BossTypeHandler
	{
		public byte BossType { get; protected set; }

		public IEnumerable<Type> ClassTypes { get; protected set; } = [];

		// Constructor
		protected BossTypeHandler()
		{
		}

		public abstract void WriteTo(BossSerializer serializer, BossWriter writer, object value);
		public abstract object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype);
	}
}
