using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeImp.Boss.TypeHandlers
{
	public class FixedObjectHandler : BossTypeHandler
	{
		private struct MemberValueInfo
		{
			public string Name;
			public Type Type;
			public object? Value;
			public bool ForceDynamic;
		};

		public override byte BossType => (byte)BossTypeCode.FixedObject;
		public override Type? ClassType => null;

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			Type valuetype = value.GetType();
			object defaultvalue = Activator.CreateInstance(valuetype);
			List<MemberInfo> members = serializer.GetSerializableMembers(valuetype);
			List<MemberValueInfo> finalmembers = new List<MemberValueInfo>();

			// Get member objects and determine if it we want to serialize this member
			foreach(MemberInfo m in members)
			{
				MemberValueInfo valueinfo;
				valueinfo.Name = m.Name;

				BossSerializableAttribute sa = m.GetCustomAttribute<BossSerializableAttribute>() ?? BossSerializableAttribute.Default;
				valueinfo.ForceDynamic = sa.Polymorphic;

				if(m is FieldInfo f)
				{
					valueinfo.Type = f.FieldType;
					valueinfo.Value = f.GetValue(value);

					// Check if this member has its default value and we should skip it
					if(sa.DefaultValueBehavior == DefaultValueBehavior.Default)
					{
						if(valueinfo.Type.IsValueType)
						{
							if(Equals(valueinfo.Value, f.GetValue(defaultvalue)))
								continue;
						}
						else
						{
							if((valueinfo.Value is null) && (f.GetValue(defaultvalue) is null))
								continue;
						}
					}
				}
				else if(m is PropertyInfo p)
				{
					valueinfo.Type = p.PropertyType;
					valueinfo.Value = p.GetValue(value);

					// Check if this member has its default value and we should skip it
					if(sa.DefaultValueBehavior == DefaultValueBehavior.Default)
					{
						if(valueinfo.Type.IsValueType)
						{
							if(Equals(valueinfo.Value, p.GetValue(defaultvalue)))
								continue;
						}
						else
						{
							if((valueinfo.Value is null) && (p.GetValue(defaultvalue) is null))
								continue;
						}
					}
				}
				else
				{
					throw new NotImplementedException();
				}

				finalmembers.Add(valueinfo);
			}

			// Serialize
			writer.WriteVLQ(finalmembers.Count);
			foreach(MemberValueInfo m in finalmembers)
			{
				writer.Write(m.Name);
				serializer.Serialize(m.Value, m.Type, writer, m.ForceDynamic);
			}
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			object? obj = CreateInstance(serializer, reader, basetype);
			Type objtype = obj?.GetType() ?? basetype;
            List<MemberInfo> objmembers = serializer.GetSerializableMembers(objtype);
			int memberscount = reader.ReadVLQ();
			for(int i = 0; i < memberscount; i++)
			{
				string membername = reader.ReadString() ?? throw new InvalidDataException("Member names cannot be null strings.");
				MemberInfo? memberinfo = objmembers.FirstOrDefault(m => m.Name == membername);;
				if(memberinfo == null)
				{
					// To continue deserializing, we MUST call deserialize to skip this data.
					serializer.Deserialize(reader, typeof(object));
				}
				else if(memberinfo is FieldInfo fieldinfo)
				{
                    // Member is a field
					object? result = serializer.Deserialize(reader, fieldinfo.FieldType);
                    try
                    {
                        if(result == null)
                        {
                            fieldinfo.SetValue(obj, null);
                        }
                        else
                        {
                            Type resulttype = result.GetType();
                            if(IsConversionNeeded(resulttype, fieldinfo.FieldType))
                                fieldinfo.SetValue(obj, Convert.ChangeType(result, fieldinfo.FieldType));
                            else
					            fieldinfo.SetValue(obj, result);
                        }
                    }
                    catch(Exception)
                    {
                        // Maybe we want to implement some public event in BossSerialize that is raised for this exception?
                        // Just to notify the user and the user can choose how to response to this exception.
                    }
				}
				else if(memberinfo is PropertyInfo propinfo)
				{
                    // Member is a property
					object? result = serializer.Deserialize(reader, propinfo.PropertyType);
                    try
                    {
                        if(result == null)
                        {
                            propinfo.SetValue(obj, null);
                        }
                        else
                        {
                            Type resulttype = result.GetType();
                            if(IsConversionNeeded(resulttype, propinfo.PropertyType))
                                propinfo.SetValue(obj, Convert.ChangeType(result, propinfo.PropertyType));
                            else
                                propinfo.SetValue(obj, result);
                        }
                    }
                    catch(Exception)
                    {
                        // Maybe we want to implement some public event in BossSerialize that is raised for this exception?
                        // Just to notify the user and the user can choose how to response to this exception.
                    }
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			return obj;
		}

		protected virtual object? CreateInstance(BossSerializer serializer, BossReader reader, Type basetype)
		{
			return Activator.CreateInstance(basetype, false);
		}

        private bool IsConversionNeeded(Type datatype, Type membertype)
        {
            if(datatype.IsValueType || membertype.IsValueType)
            {
                return (datatype != membertype);
            }
            else
            {
                return !membertype.IsAssignableFrom(datatype);
            }
        }
	}
}
