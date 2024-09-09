
using System.Reflection;

namespace CodeImp.Boss.TypeHandlers
{
	public class FixedObjectTypeHandler : BossTypeHandler
	{
		public FixedObjectTypeHandler()
		{
			BossType = (byte)BossElementTypes.FixedObject;
			ClassTypes = [];
		}

		public override void WriteTo(BossWriter writer, object value)
		{
			List<MemberInfo> members = BossSerializer.GetSerializableMembers(value.GetType());
			writer.WriteVLQ(members.Count);
			foreach(MemberInfo m in members)
			{
				Type membertype;
				object? membervalue;
				if(m is FieldInfo f)
				{
					membertype = f.FieldType;
					membervalue = f.GetValue(value);
				}
				else if(m is PropertyInfo p)
				{
					membertype = p.PropertyType;
					membervalue = p.GetValue(value);
				}
				else
				{
					throw new NotImplementedException();
				}

				writer.Write(m.Name);
				BossSerializer.Serialize(membervalue, membertype, writer);
			}
		}

		public override object? ReadFrom(BossReader reader, Type basetype)
		{
			object? obj = CreateInstance(reader, basetype);
			Type objtype = obj?.GetType() ?? basetype;
			int memberscount = reader.ReadVLQ();
			for(int i = 0; i < memberscount; i++)
			{
				string membername = reader.ReadString() ?? throw new InvalidDataException("Member names cannot be null strings.");
				MemberInfo? memberinfo = BossSerializer.FindSerializableMember(objtype, membername);
				if(memberinfo == null)
				{
					// To continue deserializing, we MUST call deserialize to skip this data.
					BossSerializer.Deserialize(reader, typeof(object));
				}
				else if(memberinfo is FieldInfo fieldinfo)
				{
					object? result = BossSerializer.Deserialize(reader, fieldinfo.FieldType);
					fieldinfo.SetValue(obj, result);
				}
				else if(memberinfo is PropertyInfo propinfo)
				{
					object? result = BossSerializer.Deserialize(reader, propinfo.PropertyType);
					propinfo.SetValue(obj, result);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			return obj;
		}

		protected virtual object? CreateInstance(BossReader reader, Type basetype)
		{
			return Activator.CreateInstance(basetype, false);
		}
	}
}
