using System;

namespace CodeImp.Boss
{
	public enum DefaultValueBehavior
	{
		/// <summary>
		/// Default behavior: Properties and fields that have their default value are not serialized.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Include this member even when the value is the default value.
		/// </summary>
		Include = 1
	}
}
