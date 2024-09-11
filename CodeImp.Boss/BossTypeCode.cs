namespace CodeImp.Boss
{
    /// <summary>
    /// Type code values used in the Boss data format.
    /// Values are split up in the following ranges:
    /// 0  .. 63  - Built-in types
    /// 64 .. 255 - Extension types
    /// </summary>
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
        FixedArray = 13,
		DynamicArray = 14,
        FixedObject = 15,
        DynamicObject = 16,
        FixedDictionary = 17,
        DynamicDictionary = 18,

        LastBuiltIn = 63
    }
}
