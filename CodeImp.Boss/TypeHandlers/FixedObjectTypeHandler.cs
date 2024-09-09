
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

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
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
				serializer.Serialize(membervalue, membertype, writer);
			}
		}

		public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
		{
			object? obj = CreateInstance(serializer, reader, basetype);
			Type objtype = obj?.GetType() ?? basetype;
			int memberscount = reader.ReadVLQ();
			for(int i = 0; i < memberscount; i++)
			{
				string membername = reader.ReadString() ?? throw new InvalidDataException("Member names cannot be null strings.");
				MemberInfo? memberinfo = BossSerializer.FindSerializableMember(objtype, membername);
				if(memberinfo == null)
				{
					// To keep deserializing properly, we MUST call deserialize, even when the member cannot be found.
					// So when the we can't find this member, we just make up a dummy type and discard the deserialization result.
					serializer.Deserialize(reader, typeof(object));
				}
				else if(memberinfo is FieldInfo fieldinfo)
				{
					object? result = serializer.Deserialize(reader, fieldinfo.FieldType);
					fieldinfo.SetValue(obj, result);
				}
				else if(memberinfo is PropertyInfo propinfo)
				{
					object? result = serializer.Deserialize(reader, propinfo.PropertyType);
					propinfo.SetValue(obj, result);
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
	}
}
