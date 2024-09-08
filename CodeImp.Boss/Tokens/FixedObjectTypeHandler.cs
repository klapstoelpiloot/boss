
using System.Reflection;

namespace CodeImp.Boss.Tokens
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

		public override object ReadFrom(BossReader reader)
		{
			// TODO
			return null;
		}
	}
}
