namespace CodeImp.Boss.Tests
{
	internal class IgnoreAttributeTests : TestBase
	{
		public class ObjWithIgnoredProperty
		{
			public int Age { get; set; } = 6;

			[BossIgnore]
			public int IgnoredProperty { get; set; } = 8;
		}

		[Test]
		public void ObjectWithIgnoredProperty()
		{
			ObjWithIgnoredProperty obj = new ObjWithIgnoredProperty();
			obj.Age = 18;
			obj.IgnoredProperty = 1200;
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "10-00-00-00-00-00-00-00-0F-01-01-06-12-00-00-00-01-03-41-67-65");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithIgnoredProperty? result = BossSerializer.Deserialize<ObjWithIgnoredProperty>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithIgnoredProperty>());
			Assert.That(result.Age, Is.EqualTo(obj.Age));
			Assert.That(result.IgnoredProperty, Is.EqualTo(new ObjWithIgnoredProperty().IgnoredProperty));
		}

		public class ObjWithIgnoredField
		{
			public int Age = 6;

			[BossIgnore]
			public int IgnoredField = 8;
		}

		[Test]
		public void ObjectWithIgnoredField()
		{
			ObjWithIgnoredField obj = new ObjWithIgnoredField();
			obj.Age = 18;
			obj.IgnoredField = 1200;
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "10-00-00-00-00-00-00-00-0F-01-01-06-12-00-00-00-01-03-41-67-65");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithIgnoredField? result = BossSerializer.Deserialize<ObjWithIgnoredField>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithIgnoredField>());
			Assert.That(result.Age, Is.EqualTo(obj.Age));
			Assert.That(result.IgnoredField, Is.EqualTo(new ObjWithIgnoredField().IgnoredField));
		}
	}
}
