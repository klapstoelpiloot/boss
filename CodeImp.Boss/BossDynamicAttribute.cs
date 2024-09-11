using System;

namespace CodeImp.Boss
{
	/// <summary>
	/// This attribute indicates that the property or field that it is applied to must always be serialized as a dynamic object.
	/// In most cases the serializer can detect this automatically, but in some cases, this attribute is needed.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class BossDynamicAttribute : Attribute
	{
	}
}
