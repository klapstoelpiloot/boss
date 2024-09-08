using CodeImp.Boss.Tokens;
using System.Reflection;

namespace CodeImp.Boss
{
	public class BossSerializer
	{
		private BossTypeHandler[] typehandlers = new BossTypeHandler[256];

		// Constructor
		public BossSerializer()
		{
			// Internal type handlers
			typehandlers[(byte)BossElementTypes.Null] = new NullTypeHandler();
			typehandlers[(byte)BossElementTypes.Int] = new IntTypeHandler();
			typehandlers[(byte)BossElementTypes.String] = new StringTypeHandler();
			typehandlers[(byte)BossElementTypes.FixedObject] = new FixedObjectTypeHandler();
		}

		public void Serialize(object obj, Stream stream)
		{
			using BossWriter writer = new BossWriter(stream);
			writer.BeginWriting();
			Serialize(obj, obj.GetType(), writer);
			writer.EndWriting();
		}

		private BossTypeHandler SelectTypeHandler(Type? type, object? obj)
		{
			// TODO: Add support for arrays
			
			if(obj is null)
			{
				return typehandlers[(byte)BossElementTypes.Null];
			}
			else
			{
				BossTypeHandler? h = typehandlers.FirstOrDefault(h => (h != null) && h.ClassTypes.Contains(type));
				if(h != null)
				{
					return h;
				}
				else
				{
					if(!type.IsPrimitive)
					{
						return typehandlers[(byte)BossElementTypes.FixedObject];
					}
					else
					{
						throw new InvalidDataException($"No type handler registered to serialize type '{type}'");
					}
				}
			}
		}

		/// <summary>
		/// Serializes the given object using the specified type handler.
		/// </summary>
		internal void Serialize(object? obj, Type? type, BossWriter writer)
		{
			BossTypeHandler h = SelectTypeHandler(type, obj);
			SerializeWithHandler(obj, h, writer);
		}

		/// <summary>
		/// Serializes the given object using the specified type handler.
		/// </summary>
		internal void SerializeWithHandler(object? obj, BossTypeHandler handler, BossWriter writer)
		{
			writer.Write(handler.BossType);
			if(obj != null)
			{
				handler.WriteTo(this, writer, obj);
			}
		}
		
		/// <summary>
		/// Helper method to get all serializable members from the specified type.
		/// </summary>
		internal static List<MemberInfo> GetSerializableMembers(Type type)
		{
			List<MemberInfo> members =
			[
				.. type.GetFields(BindingFlags.Instance | BindingFlags.Public),
				.. type.GetProperties(BindingFlags.Instance | BindingFlags.Public),
			];
			// TODO: Filter by Ignore attributes
			return members;
		}
	}
}
