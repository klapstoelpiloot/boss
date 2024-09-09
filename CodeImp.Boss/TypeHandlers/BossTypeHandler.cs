namespace CodeImp.Boss.TypeHandlers
{
	public abstract class BossTypeHandler
	{
		public abstract byte BossType { get; }

		public abstract IEnumerable<Type> ClassTypes { get; }

		// Constructor
		protected BossTypeHandler()
		{
		}

		public abstract void WriteTo(BossWriter writer, object value);
		public abstract object? ReadFrom(BossReader reader, Type basetype);
	}
}
