using System.Collections;
using System.Reflection;

namespace CodeImp.Boss.TypeHandlers
{
	public class FixedArrayHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.FixedArray;

		public override Type? ClassType => null;

		public override void WriteTo(BossWriter writer, object value)
		{
			Type elementtype = BossSerializer.GetCollectionElementType(value.GetType());
			int elementcount = BossSerializer.GetCollectionElementCount(value.GetType(), value);
			BossTypeHandler handler = BossSerializer.SelectTypeHandler(elementtype, elementtype);

			writer.WriteVLQ(elementcount);
			writer.Write(handler.BossType);

			// This must have horrible performance, because we know these are
			// value types and now we are intentionally boxing them! SMH.
			// But first we make it work, then we optimize...
			IEnumerable enumerable = value as IEnumerable;
			foreach(object e in enumerable)
			{
				handler.WriteTo(writer, e);
			}
		}

		public override object? ReadFrom(BossReader reader, Type basetype)
		{
			int elementcount = reader.ReadVLQ();
			byte elementtypecode = reader.ReadByte();
			BossTypeHandler handler = BossSerializer.GetTypeHandler(elementtypecode);
			Type elementtype = BossSerializer.GetCollectionElementType(basetype);

			// Again with the boxing...
			object[] list = new object[elementcount];
			for(int i = 0; i < elementcount; i++)
				list[i] = handler.ReadFrom(reader, elementtype);

			if(basetype.IsArray)
			{
				Array newarray = Array.CreateInstance(elementtype, elementcount);
				Array.Copy(list, newarray, elementcount);
				return newarray;
			}
			else
			{
				object collection = Activator.CreateInstance(basetype);
				PropertyInfo? capproperty = basetype.GetProperty("Capacity");
				capproperty?.SetValue(collection, elementcount);
				MethodInfo? addmethod = basetype.GetMethod("Add");
				if(addmethod != null)
				{
					for(int i = 0; i < elementcount; i++)
						addmethod.Invoke(collection, [list[i]]);
					return collection;
				}

				MethodInfo? enqueuemethod = basetype.GetMethod("Enqueue");
				if(enqueuemethod != null)
				{
					for(int i = 0; i < elementcount; i++)
						enqueuemethod.Invoke(collection, [list[i]]);
					return collection;
				}

				MethodInfo? pushmethod = basetype.GetMethod("Push");
				if(pushmethod != null)
				{
					for(int i = 0; i < elementcount; i++)
						pushmethod.Invoke(collection, [list[i]]);
					return collection;
				}

				throw new NotSupportedException($"Adding to the collection type '{basetype}' is not supported.");
			}
		}
	}
}
