using CodeImp.Boss.TypeHandlers;
using System.Reflection;
using System.Security.AccessControl;

namespace CodeImp.Boss
{
	public class BossSerializer
	{
		private BossTypeHandler[] typehandlers = new BossTypeHandler[256];
		private Dictionary<string, Type> typenamelookup = new Dictionary<string, Type>();

		// Constructor
		public BossSerializer()
		{
			// Internal type handlers
			RegisterTypeHandler(BossElementTypes.Null, new NullTypeHandler());
			RegisterTypeHandler(BossElementTypes.Int, new IntTypeHandler());
			RegisterTypeHandler(BossElementTypes.String, new StringTypeHandler());
			RegisterTypeHandler(BossElementTypes.FixedObject, new FixedObjectTypeHandler());
			RegisterTypeHandler(BossElementTypes.DynamicObject, new DynamicObjectTypeHandler());
		}

		private void RegisterTypeHandler(BossElementTypes typecode, BossTypeHandler handler)
		{
			RegisterTypeHandler((byte)typecode, handler);
		}

		public void RegisterTypeHandler(byte typecode, BossTypeHandler handler)
		{
			typehandlers[typecode] = handler;
		}

		public void RegisterTypeLookup(Type t)
		{
			typenamelookup[t.Name] = t;
		}

		public void Serialize<T>(T obj, Stream stream)
		{
			using BossWriter writer = new BossWriter(stream);
			writer.BeginWriting();
			Serialize(obj, typeof(T), writer);
			writer.EndWriting();
		}

		public T? Deserialize<T>(Stream stream)
		{
			using BossReader reader = new BossReader(stream);
			reader.BeginReading();
			return (T?)Deserialize(reader, typeof(T));
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
					if((type != null) && !type.IsPrimitive)
					{
						if(type.IsInterface || type.IsAbstract || (type != obj.GetType()))
						{
							// We need a dynamic object
							return typehandlers[(byte)BossElementTypes.DynamicObject];
						}
						else
						{
							// Class or struct object
							return typehandlers[(byte)BossElementTypes.FixedObject];
						}
					}
					else
					{
						throw new InvalidDataException($"No type handler registered to serialize type '{type}'");
					}
				}
			}
		}

		/// <summary>
		/// Serializes the given object.
		/// </summary>
		internal void Serialize(object? obj, Type? type, BossWriter writer)
		{
			BossTypeHandler h = SelectTypeHandler(type, obj);
			SerializeWithHandler(obj, h, writer);
		}

		internal object? Deserialize(BossReader reader, Type basetype)
		{
			byte typecode = reader.ReadByte();
			BossTypeHandler h = typehandlers[typecode] ?? throw new InvalidDataException($"No type handler registered to deserialize type code '{typecode}'");
			return h.ReadFrom(this, reader, basetype);
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
		/// This tries to find a type by its name and basetype.
		/// </summary>
		internal Type? FindType(string typename, Type basetype)
		{
			// Can we get a quick result from cache?
			if(typenamelookup.TryGetValue(typename, out Type? type) && type.IsAssignableTo(basetype))
				return type;

			// Chances are high that the specified type resides in the same assembly as the base type
			type = basetype.Assembly.GetTypes().FirstOrDefault(t => (t.Name == typename) && t.IsAssignableTo(basetype));
			if(type != null)
			{
				RegisterTypeLookup(type);
				return type;
			}

			// Search for the type in all other assemblies... this is slow.
			foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies().Where(a => a != basetype.Assembly))
			{
				type = a.GetTypes().FirstOrDefault(t => (t.Name == typename) && t.IsAssignableTo(basetype));
				if(type != null)
				{
					RegisterTypeLookup(type);
					return type;
				}
			}

			// Nothing found
			return null;
		}
		
		/// <summary>
		/// Helper method to get all serializable members from the specified type.
		/// </summary>
		internal static List<MemberInfo> GetSerializableMembers(Type type)
		{
			List<MemberInfo> members =
			[
				.. type.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(f => !f.IsInitOnly && !f.IsLiteral),
				.. type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite),
			];

			return members
				.Where(m => (m.GetCustomAttribute<BossIgnoreAttribute>(true) == null))
				.ToList();
		}
		
		/// <summary>
		/// Helper method to get a specific serializable member from the specified type.
		/// </summary>
		internal static MemberInfo? FindSerializableMember(Type type, string name)
		{
			List<MemberInfo> members = GetSerializableMembers(type);
			return members.FirstOrDefault(m => m.Name == name);
		}
	}
}
