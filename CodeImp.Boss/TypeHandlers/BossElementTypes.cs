namespace CodeImp.Boss.TypeHandlers
{
	/// <summary>
	/// Element Type values.
	/// Values are split up in the following ranges:
	/// 0  .. 64  - Built-in types
	/// 64 .. 127 - Extension types
	/// </summary>
	public enum BossElementTypes : byte
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

		Array = 0x80
	}
}
