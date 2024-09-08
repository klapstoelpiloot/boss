namespace CodeImp.Boss.Tests
{
	public class SerializationTests
	{
		[Test]
		public void Test1()
		{
			BaseBoy bb = new BaseBoy();
			BossSerializer serializer = new BossSerializer();
			MemoryStream stream = new MemoryStream();
			serializer.Serialize(bb, stream);
			byte[] bytes = stream.ToArray();
			Assert.Pass();
		}
	}
}