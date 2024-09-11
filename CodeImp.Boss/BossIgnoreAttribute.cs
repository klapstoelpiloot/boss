using System;

namespace CodeImp.Boss
{
	/// <summary>
	/// This attribute indicates that the property or field that it is applied to must be ignored during serialization or deserialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class BossIgnoreAttribute : Attribute
	{
	}
}
