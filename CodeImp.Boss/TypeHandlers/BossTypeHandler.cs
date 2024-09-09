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

		public abstract void WriteTo(BossWriter writer, object value);
		public abstract object? ReadFrom(BossReader reader, Type basetype);
	}
}
