using System.Reflection;

namespace CodeImp.Boss.TypeHandlers
{
	public class FixedObjectHandler : BossTypeHandler
	{
		public override byte BossType => (byte)BossTypeCode.FixedObject;
		public override Type? ClassType => null;

		public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
		{
			List<MemberInfo> members = serializer.GetSerializableMembers(value.GetType());
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
				MemberInfo? memberinfo = serializer.FindSerializableMember(objtype, membername);
				if(memberinfo == null)
				{
					// To continue deserializing, we MUST call deserialize to skip this data.
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
