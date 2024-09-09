using System.Diagnostics.CodeAnalysis;

namespace CodeImp.Boss.TypeHandlers
{
	/// <summary>
	/// Type code values used in the Boss data format.
	/// Values are split up in the following ranges:
	/// 0  .. 63  - Built-in types
	/// 64 .. 127 - Extension types
	/// </summary>
	[SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "Some of these enums indicate ranges")]
	public enum BossTypeCode : byte
	{
		Null = 0,
		Bool = 1,
		Byte = 2,
		SByte = 3,
		Short = 4,
		UShort = 5,
		Int = 6,
		UInt = 7,
		Long = 8,
		ULong = 9,
		Float = 10,
		Double = 11,
		String = 12,
		MultiArray = 13,
		FixedObject = 14,
		DynamicObject = 15,
		FixedDictionary = 16,
		DynamicDictionary = 17,

		// ...

		ExtensionRangeStart = 64,
		ArrayBit = 0x80
	}
}
