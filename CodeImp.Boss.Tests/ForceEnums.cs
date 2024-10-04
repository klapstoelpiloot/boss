namespace CodeImp.Boss.Tests
{
    public enum Forces
    {
        None = 0,
        Gravity = 1,
        Electromagnetism = 2,
        Weak = 3,
        Strong = 4
    };

    [BossEnumOptions(Method = EnumSerializationMethod.MemberNames)]
    public enum ForcesByName
    {
        None = 0,
        Gravity = 1,
        Electromagnetism = 2,
        Weak = 3,
        Strong = 4
    };
}
