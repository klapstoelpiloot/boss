namespace CodeImp.Boss.Tests
{
	internal class DynamicObjectsTests : TestBase
	{
		public interface ICanDoSomething
		{
		}

		public class DynamicClass1 : ICanDoSomething
		{
		}

		public class ObjWithInterfaceProperty
		{
			public ICanDoSomething? Dyna { get; set; }
		}

		[Test]
		public void ObjectWithInterfacePropertyNull()
		{
			ObjWithInterfaceProperty obj = new ObjWithInterfaceProperty();
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "0C-00-00-00-00-00-00-00-0E-01-01-00-01-04-44-79-6E-61");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithInterfaceProperty? result = BossSerializer.Deserialize<ObjWithInterfaceProperty>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithInterfaceProperty>());
			Assert.That(result.Dyna, Is.Null);
		}

		[Test]
		public void ObjectWithInterfacePropertySet()
		{
			ObjWithInterfaceProperty obj = new ObjWithInterfaceProperty();
			obj.Dyna = new DynamicClass1();
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "0E-00-00-00-00-00-00-00-0E-01-01-0F-02-00-02-04-44-79-6E-61-0D-44-79-6E-61-6D-69-63-43-6C-61-73-73-31");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithInterfaceProperty? result = BossSerializer.Deserialize<ObjWithInterfaceProperty>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithInterfaceProperty>());
			Assert.That(result.Dyna, Is.InstanceOf<DynamicClass1>());
		}

		public class DynamicClass2
		{
		}

		public class DerivedDynamicClass2 : DynamicClass2
		{
		}

		public class ObjWithDerivedClassProperty
		{
			public DynamicClass2? Dyna { get; set; }
		}

		[Test]
		public void ObjectWithDerivedClassPropertyNull()
		{
			ObjWithDerivedClassProperty obj = new ObjWithDerivedClassProperty();
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "0C-00-00-00-00-00-00-00-0E-01-01-00-01-04-44-79-6E-61");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithDerivedClassProperty? result = BossSerializer.Deserialize<ObjWithDerivedClassProperty>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithDerivedClassProperty>());
			Assert.That(result.Dyna, Is.Null);
		}

		[Test]
		public void ObjectWithDerivedClassPropertySet()
		{
			ObjWithDerivedClassProperty obj = new ObjWithDerivedClassProperty();
			obj.Dyna = new DerivedDynamicClass2();
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "0E-00-00-00-00-00-00-00-0E-01-01-0F-02-00-02-04-44-79-6E-61-14-44-65-72-69-76-65-64-44-79-6E-61-6D-69-63-43-6C-61-73-73-32");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithDerivedClassProperty? result = BossSerializer.Deserialize<ObjWithDerivedClassProperty>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithDerivedClassProperty>());
			Assert.That(result.Dyna, Is.InstanceOf<DerivedDynamicClass2>());
		}

	}
}
