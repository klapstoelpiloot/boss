﻿using System;

namespace CodeImp.Boss
{
    public enum EnumSerializationMethod
    {
        /// <summary>
        /// Serialzie the enum values as the underlying type of the enum.
        /// </summary>
        Values = 0,

        /// <summary>
        /// Serialize the enum name as a string.
        /// </summary>
        Names = 1
    };

	/// <summary>
	/// This attribute configures the serialization/deserialization behavior for the property or field that it is applied to.
	/// </summary>
	[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class BossEnumOptionsAttribute : Attribute
    {
		public static readonly BossEnumOptionsAttribute Default = new BossEnumOptionsAttribute();

		/// <summary>
		/// Controls how values are serialized.
		/// </summary>
		public EnumSerializationMethod Method { get; set; } = EnumSerializationMethod.Values;
    }
}
