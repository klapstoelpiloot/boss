using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CodeImp.Boss.TypeHandlers
{
	public class FixedArrayHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.FixedArray;
		public override Type? ClassType => null;

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			Type elementtype = BossSerializer.GetCollectionElementType(value.GetType());
			int elementcount = BossSerializer.GetCollectionElementCount(value.GetType(), value);
			BossTypeHandler handler = serializer.SelectTypeHandler(elementtype, elementtype, false);

			writer.WriteVLQ(elementcount);
			writer.Write(handler.BossType);

            if(elementtype.IsPrimitive && !elementtype.IsEnum)
            {
			    switch(value)
			    {
				    // Special optimized code for primitives to avoid boxing
				    case bool[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case byte[] array: writer.Write(array); return;
				    case sbyte[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case short[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case ushort[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case int[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case uint[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case long[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case ulong[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case float[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case double[] array: for(int i = 0; i < array.Length; i++) { writer.Write(array[i]); } return;
				    case List<bool> list: list.ForEach(v => writer.Write(v)); return;
				    case List<byte> list: list.ForEach(v => writer.Write(v)); return;
				    case List<sbyte> list: list.ForEach(v => writer.Write(v)); return;
				    case List<short> list: list.ForEach(v => writer.Write(v)); return;
				    case List<ushort> list: list.ForEach(v => writer.Write(v)); return;
				    case List<int> list: list.ForEach(v => writer.Write(v)); return;
				    case List<uint> list: list.ForEach(v => writer.Write(v)); return;
				    case List<long> list: list.ForEach(v => writer.Write(v)); return;
				    case List<ulong> list: list.ForEach(v => writer.Write(v)); return;
				    case List<float> list: list.ForEach(v => writer.Write(v)); return;
				    case List<double> list: list.ForEach(v => writer.Write(v)); return;
                }
            }

			// Use an oldschool enumerator and the type handler.
			// This causes boxing to an object and is slightly slower.
			IEnumerable enumerable = value as IEnumerable;
			foreach(object e in enumerable)
			{
				handler.WriteTo(serializer, writer, e);
			}
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			int elementcount = reader.ReadVLQ();
			byte elementtypecode = reader.ReadByte();
			BossTypeHandler handler = serializer.GetTypeHandler(elementtypecode);
			Type elementtype = BossSerializer.GetCollectionElementType(basetype);

			if(basetype.IsArray)
			{
                if(elementtype.IsPrimitive && !elementtype.IsEnum)
                {
				    switch(Type.GetTypeCode(elementtype))
				    {
					    // Special optimized code for primitives to avoid boxing
					    case TypeCode.Boolean: { bool[] array = new bool[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadBoolean(); } return array; }
					    case TypeCode.Byte: { byte[] array = reader.ReadBytes(elementcount); return array; }
					    case TypeCode.SByte: { sbyte[] array = new sbyte[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadSByte(); } return array; }
					    case TypeCode.Int16: { short[] array = new short[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadInt16(); } return array; }
					    case TypeCode.UInt16: { ushort[] array = new ushort[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadUInt16(); } return array; }
					    case TypeCode.Int32: { int[] array = new int[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadInt32(); } return array; }
					    case TypeCode.UInt32: { uint[] array = new uint[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadUInt32(); } return array; }
					    case TypeCode.Int64: { long[] array = new long[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadInt64(); } return array; }
					    case TypeCode.UInt64: { ulong[] array = new ulong[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadUInt64(); } return array; }
					    case TypeCode.Single: { float[] array = new float[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadSingle(); } return array; }
					    case TypeCode.Double: { double[] array = new double[elementcount]; for(int i = 0; i < array.Length; i++) { array[i] = reader.ReadDouble(); } return array; }
				    }
                }

                // Standard reading using the type handler
				object[] oarray = new object[elementcount];
				for(int i = 0; i < elementcount; i++)
					oarray[i] = handler.ReadFrom(serializer, reader, elementtype);
				Array result = Array.CreateInstance(elementtype, elementcount);
				Array.Copy(oarray, result, elementcount);
				return result;
			}
			else
			{
				if((basetype.GetGenericTypeDefinition() == typeof(List<>)) && elementtype.IsPrimitive && !elementtype.IsEnum)
				{
					// Special optimized code for primitives to avoid boxing
					switch(Type.GetTypeCode(elementtype))
					{
						case TypeCode.Boolean: { List<bool> list = new List<bool>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadBoolean()); } return list; }
						case TypeCode.Byte: { List<byte> list = new List<byte>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadByte()); } return list; }
						case TypeCode.SByte: { List<sbyte> list = new List<sbyte>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadSByte()); } return list; }
						case TypeCode.Int16: { List<short> list = new List<short>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadInt16()); } return list; }
						case TypeCode.UInt16: { List<ushort> list = new List<ushort>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadUInt16()); } return list; }
						case TypeCode.Int32: { List<int> list = new List<int>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadInt32()); } return list; }
						case TypeCode.UInt32: { List<uint> list = new List<uint>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadUInt32()); } return list; }
						case TypeCode.Int64: { List<long> list = new List<long>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadInt64()); } return list; }
						case TypeCode.UInt64: { List<ulong> list = new List<ulong>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadUInt64()); } return list; }
						case TypeCode.Single: { List<float> list = new List<float>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadSingle()); } return list; }
						case TypeCode.Double: { List<double> list = new List<double>(elementcount); for(int i = 0; i < elementcount; i++) { list.Add(reader.ReadDouble()); } return list; }
					}
				}

				// Read elements into an object[]
				object[] array = new object[elementcount];
				for(int i = 0; i < elementcount; i++)
					array[i] = handler.ReadFrom(serializer, reader, elementtype);

				// Create the collection through reflection
				object collection = Activator.CreateInstance(basetype);
				PropertyInfo? capproperty = basetype.GetProperty("Capacity");
				capproperty?.SetValue(collection, elementcount);

				// Add items
				MethodInfo? addmethod = basetype.GetMethod("Add");
				if(addmethod != null)
				{
					for(int i = 0; i < array.Length; i++)
						addmethod.Invoke(collection, new object[] {array[i]});
					return collection;
				}

				// Enqueue items
				MethodInfo? enqueuemethod = basetype.GetMethod("Enqueue");
				if(enqueuemethod != null)
				{
					for(int i = 0; i < array.Length; i++)
						enqueuemethod.Invoke(collection, new object[] {array[i]});
					return collection;
				}

				// Push items
				MethodInfo? pushmethod = basetype.GetMethod("Push");
				if(pushmethod != null)
				{
					for(int i = 0; i < array.Length; i++)
						pushmethod.Invoke(collection, new object[] {array[i]});
					return collection;
				}

				throw new NotSupportedException($"Adding to the collection type '{basetype}' is not supported.");
			}
		}
	}
}
