namespace CodeImp.Boss.Tests
{
	public class BaseBoy
	{
		public const int CONST_FIELD = 4;

		public readonly int readonlyfield = 99;
		public int intfield = 2;

		public int IntProperty { get; set; } = 6;
		public string StringProperty { get; set; } = "BaseBoy";
		public MemberMama? NullProperty { get; set; }
		public MemberMama ObjectProperty { get; set; } = new MemberMama();
	}

	public class MemberMama
	{
		public string StringProperty { get; set; } = "Mommy";
	}

	public class DerivedDonny : BaseBoy
	{
		public int AnotherIntProperty { get; set; } = 1183572;
	}
}
