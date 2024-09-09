using CodeImp.Boss.TypeHandlers;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeImp.Boss
{
	public static class BossSerializer
	{
		// The type handlers by boss typecode
		private static readonly BossTypeHandler[] typehandlers = new BossTypeHandler[256];

		// Lookup cache for Types by their classname only
		private static readonly Dictionary<string, Type> typenamelookup = [];

		// Constructor
		static BossSerializer()
		{
			// Internal type handlers
			RegisterTypeHandlerInternal(new NullTypeHandler());
			RegisterTypeHandlerInternal(new IntTypeHandler());
			RegisterTypeHandlerInternal(new StringTypeHandler());
			RegisterTypeHandlerInternal(new FixedObjectTypeHandler());
			RegisterTypeHandlerInternal(new DynamicObjectTypeHandler());
		}

		/// <summary>
		/// Registers a type handler for the specified Boss typecode.
		/// </summary>
		private static void RegisterTypeHandlerInternal(BossTypeHandler handler)
		{
			if(handler.BossType >= (int)BossElementTypes.ExtensionRangeStart)
				throw new InvalidOperationException("Built-in type handlers should be in the range 0 .. 63");

			typehandlers[handler.BossType] = handler;
		}

		/// <summary>
		/// Registers a type handler for the specified Boss typecode.
		/// </summary>
		public static void RegisterTypeHandler(BossTypeHandler handler)
		{
			if(handler.BossType < (int)BossElementTypes.ExtensionRangeStart)
				throw new InvalidOperationException("Extension type handlers should be in the range 64 .. 127");

			typehandlers[handler.BossType] = handler;
		}

		/// <summary>
		/// Adds a Type to the lookup cache to improve the performance of Type instantiation during deserialization.
		/// </summary>
		public static void AddTypeLookupCache(Type t)
		{
			typenamelookup[t.Name] = t;
		}

		/// <summary>
		/// Clears the Type lookup cache.
		/// </summary>
		public static void ClearTypeLookupCache()
		{
			typenamelookup.Clear();
		}

		public static void Serialize<T>(T obj, Stream stream)
		{
			using BossWriter writer = new BossWriter(stream);
			writer.BeginWriting();
			Serialize(obj, typeof(T), writer);
			writer.EndWriting();
		}

		public static T? Deserialize<T>(Stream stream)
		{
			using BossReader reader = new BossReader(stream);
			reader.BeginReading();
			return (T?)Deserialize(reader, typeof(T));
		}

		private static BossTypeHandler SelectTypeHandler(Type? type, object? obj)
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
		internal static void Serialize(object? obj, Type? type, BossWriter writer)
		{
			BossTypeHandler h = SelectTypeHandler(type, obj);
			SerializeWithHandler(obj, h, writer);
		}

		internal static object? Deserialize(BossReader reader, Type basetype)
		{
			byte typecode = reader.ReadByte();
			BossTypeHandler h = typehandlers[typecode] ?? throw new InvalidDataException($"No type handler registered to deserialize type code '{typecode}'");
			return h.ReadFrom(reader, basetype);
		}

		/// <summary>
		/// Serializes the given object using the specified type handler.
		/// </summary>
		internal static void SerializeWithHandler(object? obj, BossTypeHandler handler, BossWriter writer)
		{
			writer.Write(handler.BossType);
			if(obj != null)
			{
				handler.WriteTo(writer, obj);
			}
		}

		/// <summary>
		/// This tries to find a type by its name and basetype.
		/// </summary>
		internal static Type? FindType(string typename, Type basetype)
		{
			// Can we get a quick result from cache?
			if(typenamelookup.TryGetValue(typename, out Type? type) && type.IsAssignableTo(basetype))
				return type;

			// Chances are high that the specified type resides in the same assembly as the base type
			type = basetype.Assembly.GetTypes().FirstOrDefault(t => (t.Name == typename) && t.IsAssignableTo(basetype));
			if(type != null)
			{
				AddTypeLookupCache(type);
				return type;
			}

			// Search for the type in all other assemblies... this is slow.
			foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies().Where(a => a != basetype.Assembly))
			{
				type = a.GetTypes().FirstOrDefault(t => (t.Name == typename) && t.IsAssignableTo(basetype));
				if(type != null)
				{
					AddTypeLookupCache(type);
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
