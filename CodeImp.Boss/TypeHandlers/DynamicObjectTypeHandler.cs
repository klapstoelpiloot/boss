using System.Reflection;

namespace CodeImp.Boss.TypeHandlers
{
	public class DynamicObjectTypeHandler : FixedObjectTypeHandler
	{
		public DynamicObjectTypeHandler()
		{
			BossType = (byte)BossElementTypes.DynamicObject;
			ClassTypes = [];
		}

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			Type valuetype = value.GetType();
			writer.Write(valuetype.Name);
			base.WriteTo(serializer, writer, value);
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return base.ReadFrom(serializer, reader, basetype);
		}

		protected override object? CreateInstance(BossSerializer serializer, BossReader reader, Type basetype)
		{
			string classname = reader.ReadString() ?? throw new InvalidDataException("Class names cannot be null strings.");
			Type? classtype = serializer.FindType(classname, basetype);
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
