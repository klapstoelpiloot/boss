﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CodeImp.Boss
{
    public class BossSerializer
    {
        // The type handlers
        private static readonly BossTypeHandler[] typehandlers = new BossTypeHandler[256];
        private static readonly Dictionary<Type, BossTypeHandler> handlersbyclasstype = new Dictionary<Type, BossTypeHandler>();
        private static readonly ReaderWriterLock handlerslock = new ReaderWriterLock();

        // Lookup caches
        private readonly Dictionary<string, Type> typenamelookup = new Dictionary<string, Type>();
        private readonly Dictionary<Type, List<MemberInfo>> serializablememberscache = new Dictionary<Type, List<MemberInfo>>();

        // Serialization options
        public bool ThrowOnDeserializationFailure { get; set; } = false;

        // Static constructor
        static BossSerializer()
        {
            // Register all built-in type handlers
            Type basetype = typeof(BossTypeHandler);
            List<Type> handlertypes = Assembly.GetExecutingAssembly().GetTypes()
                                      .Where(t => !t.IsAbstract && basetype.IsAssignableFrom(t)).ToList();
            foreach (Type t in handlertypes)
            {
                if (Activator.CreateInstance(t) is BossTypeHandler handler)
                    RegisterTypeHandlerInternal(handler);
            }
        }

        // Instance constructor
        public BossSerializer()
        {
        }

        /// <summary>
        /// Registers a type handler for the specified Boss typecode.
        /// </summary>
        private static void RegisterTypeHandlerInternal(BossTypeHandler handler)
        {
            handlerslock.AcquireWriterLock(TimeSpan.FromSeconds(1));
            try
            {
                typehandlers[handler.BossType] = handler;
                if (handler.ClassType != null)
                {
                    handlersbyclasstype[handler.ClassType] = handler;
                }
            }
            finally
            {
                handlerslock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Registers a type handler for the specified Boss typecode.
        /// </summary>
        public static void RegisterTypeHandler(BossTypeHandler handler)
        {
            if (handler.BossType <= (int)BossTypeCode.LastBuiltIn)
                throw new InvalidOperationException("Extension type handlers should be in the range 64 .. 255");

            RegisterTypeHandlerInternal(handler);
        }

        /// <summary>
        /// Serializes the given object to the specified stream.
        /// This does not close or dispose the stream.
        /// </summary>
        public void Serialize<T>(T obj, Stream stream)
        {
            handlerslock.AcquireReaderLock(TimeSpan.FromSeconds(1));
            try
            {
                using BossWriter writer = new BossWriter(stream);
                writer.BeginWriting();
                SerializeInternal(obj, typeof(T), writer, false);
                writer.EndWriting();
            }
            finally
            {
                handlerslock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Deserializes an object from the specified stream.
		/// This does not close or dispose the stream.
		/// </summary>
		public T Deserialize<T>(Stream stream)
        {
            handlerslock.AcquireReaderLock(TimeSpan.FromSeconds(1));
            try
            {
                using BossReader reader = new BossReader(stream);
                reader.BeginReading();
                return (T)DeserializeInternal(reader, typeof(T));
            }
            finally
            {
                handlerslock.ReleaseReaderLock();
            }
        }

        // Returns the handler for the specified Boss type code
        internal BossTypeHandler GetTypeHandler(BossTypeCode typecode)
        {
            return typehandlers[(byte)typecode];
        }

        /// <summary>
        /// Returns the handler for the specified Boss type code.
        /// </summary>
        public BossTypeHandler GetTypeHandler(byte typecode)
        {
            return typehandlers[typecode];
        }

        /// <summary>
        /// Find a matching type handler for the given object type.
        /// The argument membertype is the type as which the property/field is declared.
        /// The argument objecttype is the actual object's type, which may be different.
        /// Set it to null when the object is null.
        /// </summary>
        public BossTypeHandler SelectTypeHandler(Type membertype, Type objecttype, bool forcedynamic)
        {
            // Null is just that. Null.
            if (objecttype is null)
                return GetTypeHandler(BossTypeCode.Null);

            // See if we have a type handler for this kind of thing...
            if (handlersbyclasstype.TryGetValue(membertype, out BossTypeHandler handler))
            {
                return handler;
            }
            // Primitive type? int, float, bool, etc...
            else if (membertype.IsPrimitive)
            {
                // For primitives we really do need a type handler ¯\_(ツ)_/¯
                throw new InvalidDataException($"No type handler registered to serialize type '{membertype}'");
            }
            // If this is a collection type, then pick one of the array handlers...
            else if (membertype.IsArray || membertype.IsEnumerable() || membertype.IsGenericEnumerable())
            {
                Type elementtype = GetCollectionElementType(membertype);
                if (elementtype.IsValueType || elementtype.IsPrimitive || elementtype.IsEnum)
                {
                    // A collection of structs or primitives never contains null or any other type,
                    // so we use a simplified array storage method...
                    return GetTypeHandler(BossTypeCode.FixedArray);
                }
                else
                {
                    // Reference type elements can be null or can be derived classes...
                    return GetTypeHandler(BossTypeCode.DynamicArray);
                }
            }
            else if (membertype.IsEnum)
            {
                BossEnumOptionsAttribute ea = membertype.GetCustomAttribute<BossEnumOptionsAttribute>() ?? BossEnumOptionsAttribute.Default;
                switch (ea.Method)
                {
                    case EnumSerializationMethod.MemberValues:
                        Type realtype = membertype.GetEnumUnderlyingType();
                        return handlersbyclasstype[realtype];

                    case EnumSerializationMethod.MemberNames:
                        return handlersbyclasstype[typeof(string)];

                    default:
                        throw new NotImplementedException();
                }
            }
            // Pick a generic type handler for struct or class objects
            else if (membertype.IsInterface || membertype.IsAbstract || (membertype != objecttype) || forcedynamic)
            {
                // When the object type can differ from the member base type, we need to use
                // the DynamicObject handler, which also serializes the object class name...
                return GetTypeHandler(BossTypeCode.DynamicObject);
            }
            else
            {
                // Fixed object type. Not so complicated.
                return GetTypeHandler(BossTypeCode.FixedObject);
            }
        }

        // Serializes the given object
        internal void SerializeInternal(object obj, Type type, BossWriter writer, bool forcedynamic)
        {
            BossTypeHandler handler = SelectTypeHandler(type, obj?.GetType() ?? null, forcedynamic);
            SerializeWithHandler(obj, handler, writer);
        }

        // Deserializes an object
        internal object DeserializeInternal(BossReader reader, Type basetype)
        {
            byte typecode = reader.ReadByte();
            BossTypeHandler handler = typehandlers[typecode];
            return handler.ReadFrom(this, reader, basetype);
        }

        // Serializes the given object using the specified type handler.
        internal void SerializeWithHandler(object obj, BossTypeHandler handler, BossWriter writer)
        {
            writer.Write(handler.BossType);
            if (obj != null)
            {
                handler.WriteTo(this, writer, obj);
            }
        }

        // This tries to find a type by its name and basetype.
        internal Type FindType(string typename, Type basetype)
        {
            // Can we get a quick result from cache?
            if (typenamelookup.TryGetValue(typename, out Type type) && basetype.IsAssignableFrom(type))
                return type;

            // Chances are high that the specified type resides in the same assembly as the base type
            type = basetype.Assembly.GetTypes().FirstOrDefault(t => (t.Name == typename) && basetype.IsAssignableFrom(t));
            if (type != null)
            {
                typenamelookup[type.Name] = type;
                return type;
            }

            // Search for the type in all other assemblies... this is slow.
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies().Where(a => a != basetype.Assembly))
            {
                type = a.GetTypes().FirstOrDefault(t => (t.Name == typename) && basetype.IsAssignableFrom(t));
                if (type != null)
                {
                    typenamelookup[type.Name] = type;
                    return type;
                }
            }

            // Nothing found
            return null;
        }

        internal static Type GetCollectionElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            else if (type.IsGenericEnumerable())
            {
                Type[] generictypes = type.GetGenericArguments();
                return generictypes[0];
            }
            else if (type.IsEnumerable())
            {
                return typeof(object);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static int GetCollectionElementCount(Type type, object collection)
        {
            if (type.IsArray)
            {
                Array a = collection as Array ?? throw new InvalidOperationException();
                if (a.Rank > 1)
                    throw new NotSupportedException("Multidimensional arrays are not supported.");
                return a.Length;
            }
            else if (type.IsEnumerable())
            {
                IEnumerable e = collection as IEnumerable ?? throw new InvalidOperationException();
                return e.Count();
            }
            else if (type.IsGenericEnumerable())
            {
                PropertyInfo countproperty = type.GetProperty("Count") ?? throw new InvalidOperationException();
                return (int)countproperty.GetValue(collection);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        // Helper method to get all serializable members from the specified type.
        internal List<MemberInfo> GetSerializableMembers(Type type)
        {
            if (!serializablememberscache.TryGetValue(type, out List<MemberInfo> members))
            {
                List<MemberInfo> potentialmembers = new List<MemberInfo>();
                potentialmembers.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(f => !f.IsInitOnly && !f.IsLiteral));
                potentialmembers.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite));
                members = potentialmembers
                    .Where(m => (m.GetCustomAttribute<BossIgnoreAttribute>(true) == null))
                    .ToList();
                serializablememberscache.Add(type, members);
            }
            return members;
        }
    }
}
