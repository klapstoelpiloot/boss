using System;
using System.Collections;
using System.Reflection;

namespace CodeImp.Boss.TypeHandlers
{
	public class DynamicArrayHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.DynamicArray;
		public override Type ClassType => null;

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			Type elementtype = BossSerializer.GetCollectionElementType(value.GetType());
			int elementcount = BossSerializer.GetCollectionElementCount(value.GetType(), value);

			writer.WriteVLQ(elementcount);

			IEnumerable enumerable = value as IEnumerable;
			foreach(object e in enumerable)
			{
				serializer.SerializeInternal(e, elementtype, writer, false);
			}
		}

		public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			int elementcount = reader.ReadVLQ();
			Type elementtype = BossSerializer.GetCollectionElementType(basetype);

			object[] list = new object[elementcount];
			for(int i = 0; i < elementcount; i++)
			{
				list[i] = serializer.DeserializeInternal(reader, elementtype);
			}

			if(basetype.IsArray)
			{
				Array newarray = Array.CreateInstance(elementtype, elementcount);
				Array.Copy(list, newarray, elementcount);
				return newarray;
			}
			else
			{
				object collection = Activator.CreateInstance(basetype);
				PropertyInfo capproperty = basetype.GetProperty("Capacity");
				capproperty?.SetValue(collection, elementcount);
				MethodInfo addmethod = basetype.GetMethod("Add");
				if(addmethod != null)
				{
					for(int i = 0; i < elementcount; i++)
						addmethod.Invoke(collection, new object[] {list[i]});
					return collection;
				}

				MethodInfo enqueuemethod = basetype.GetMethod("Enqueue");
				if(enqueuemethod != null)
				{
					for(int i = 0; i < elementcount; i++)
						enqueuemethod.Invoke(collection, new object[] {list[i]});
					return collection;
				}

				MethodInfo pushmethod = basetype.GetMethod("Push");
				if(pushmethod != null)
				{
					for(int i = 0; i < elementcount; i++)
						pushmethod.Invoke(collection, new object[] {list[i]});
					return collection;
				}

				throw new NotSupportedException($"Adding to the collection type '{basetype}' is not supported.");
			}
		}
	}
}
