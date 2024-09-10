namespace CodeImp.Boss.TypeHandlers
{
	public class DynamicObjectHandler : FixedObjectHandler
	{
		public override byte BossType => (byte)BossTypeCode.DynamicObject;

		public override IEnumerable<Type> ClassTypes => [];

		public override void WriteTo(BossWriter writer, object value)
		{
			Type valuetype = value.GetType();
			writer.Write(valuetype.Name);
			base.WriteTo(writer, value);
		}

		public override object? ReadFrom(BossReader reader, Type basetype)
		{
			return base.ReadFrom(reader, basetype);
		}

		protected override object? CreateInstance(BossReader reader, Type basetype)
		{
			string classname = reader.ReadString() ?? throw new InvalidDataException("Class names cannot be null strings.");
			Type? classtype = BossSerializer.FindType(classname, basetype);
			if(classtype != null)
			{
				return Activator.CreateInstance(classtype, false);
			}
			else
			{
				return null;
			}
		}
	}
}
