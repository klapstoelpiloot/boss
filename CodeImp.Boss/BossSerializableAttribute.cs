using System;

namespace CodeImp.Boss
{
	/// <summary>
	/// This attribute configures the serialization/deserialization behavior for the property or field that it is applied to.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class BossSerializableAttribute : Attribute
    {
		public static readonly BossSerializableAttribute Default = new BossSerializableAttribute();

		/// <summary>
		/// Controls how members with their default value are serialized.
		/// </summary>
		public DefaultValueBehavior DefaultValueBehavior { get; set; } = DefaultValueBehavior.Default;

		/// <summary>
		/// Indicates that the member must always be serialized as a dynamic object.
		/// When polymorphics members are defined using interfaces or abstract classes,
		/// they are automatically stored as dynamic objects. In other cases you want
		/// to apply this attribute where polymorphism is needed.
		/// </summary>
		public bool Polymorphic { get; set; } = false;
    }
}
