namespace CodeImp.Boss.Tests
{
	internal class SimpleTypeTests : TestBase
	{
		[Test]
		public void Integer()
		{
			int i = 42;
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(i, stream);

			AssertStreamIsEqualTo(stream, "0D-00-00-00-00-00-00-00-06-2A-00-00-00-00");

			stream.Position = 0;
			int result = BossSerializer.Deserialize<int>(stream);
			Assert.That(result, Is.EqualTo(42));
		}

		public class ObjWithInt
		{
			public int Age { get; set; } = 6;
		}

		[Test]
		public void ObjectWithIntProperty()
		{
			ObjWithInt obj = new ObjWithInt();
			obj.Age = 18;
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "10-00-00-00-00-00-00-00-0F-01-01-06-12-00-00-00-01-03-41-67-65");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithInt? result = BossSerializer.Deserialize<ObjWithInt>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithInt>());
			Assert.That(result.Age, Is.EqualTo(obj.Age));
		}

		public class ObjWithClassObj
		{
			public ObjWithInt? ClassObj { get; set; }
		}

		[Test]
		public void ObjectWithNullProperty()
		{
			ObjWithClassObj obj = new ObjWithClassObj();
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "0C-00-00-00-00-00-00-00-0F-01-01-00-01-08-43-6C-61-73-73-4F-62-6A");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithClassObj? result = BossSerializer.Deserialize<ObjWithClassObj>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithClassObj>());
			Assert.That(result.ClassObj, Is.Null);
		}

		[Test]
		public void ObjectWithObjectProperty()
		{
			ObjWithClassObj obj = new ObjWithClassObj();
			obj.ClassObj = new ObjWithInt();
			obj.ClassObj.Age = 42;
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "13-00-00-00-00-00-00-00-0F-01-01-0F-01-02-06-2A-00-00-00-02-08-43-6C-61-73-73-4F-62-6A-03-41-67-65");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithClassObj? result = BossSerializer.Deserialize<ObjWithClassObj>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithClassObj>());
			Assert.That(result.ClassObj, Is.Not.Null);
			Assert.That(result.ClassObj, Is.InstanceOf<ObjWithInt>());
			Assert.That(result.ClassObj.Age, Is.EqualTo(obj.ClassObj.Age));
		}

		public class ObjWithFields
		{
			public string firstname = string.Empty;
			public string samevalue = string.Empty;
		}

		[Test]
		public void FieldsWithSameString()
		{
			ObjWithFields obj = new ObjWithFields();
			obj.firstname = "Freddy";
			obj.samevalue = "Freddy";
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "10-00-00-00-00-00-00-00-0F-02-01-0C-02-03-0C-02-03-09-66-69-72-73-74-6E-61-6D-65-06-46-72-65-64-64-79-09-73-61-6D-65-76-61-6C-75-65");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithFields? result = BossSerializer.Deserialize<ObjWithFields>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithFields>());
			Assert.That(result.firstname, Is.EqualTo(obj.firstname));
			Assert.That(result.samevalue, Is.EqualTo(obj.samevalue));
		}
	}
}

