using System.Drawing;

namespace CodeImp.Boss.Tests
{
	internal class CollectionTests : TestBase
	{
		public class ObjWithPrimitiveArray<T>
		{
			public T[] Numbers { get; set; } = [];
		}

		[Test]
		public void PrimitiveArray()
		{
			TestPrimitiveArrayType<bool>([true, false, true], "11-00-00-00-00-00-00-00-0F-01-01-0D-03-01-01-00-01-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<byte>([3, 2, 1], "11-00-00-00-00-00-00-00-0F-01-01-0D-03-02-03-02-01-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<sbyte>([3, 2, 1], "11-00-00-00-00-00-00-00-0F-01-01-0D-03-03-03-02-01-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<short>([3, 2, 1], "14-00-00-00-00-00-00-00-0F-01-01-0D-03-04-03-00-02-00-01-00-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<ushort>([3, 2, 1], "14-00-00-00-00-00-00-00-0F-01-01-0D-03-05-03-00-02-00-01-00-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<int>([3, 2, 1], "1A-00-00-00-00-00-00-00-0F-01-01-0D-03-06-03-00-00-00-02-00-00-00-01-00-00-00-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<uint>([3, 2, 1], "1A-00-00-00-00-00-00-00-0F-01-01-0D-03-07-03-00-00-00-02-00-00-00-01-00-00-00-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<long>([3, 2, 1], "26-00-00-00-00-00-00-00-0F-01-01-0D-03-08-03-00-00-00-00-00-00-00-02-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<ulong>([3, 2, 1], "26-00-00-00-00-00-00-00-0F-01-01-0D-03-09-03-00-00-00-00-00-00-00-02-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<float>([3.0f, 2.0f, 1.0f], "1A-00-00-00-00-00-00-00-0F-01-01-0D-03-0A-00-00-40-40-00-00-00-40-00-00-80-3F-01-07-4E-75-6D-62-65-72-73");
			TestPrimitiveArrayType<double>([3.0000000003, 2.0000000002, 1.0000000001], "26-00-00-00-00-00-00-00-0F-01-01-0D-03-0B-D4-4E-0A-00-00-00-08-40-38-DF-06-00-00-00-00-40-38-DF-06-00-00-00-F0-3F-01-07-4E-75-6D-62-65-72-73");
		}

		private void TestPrimitiveArrayType<T>(T[] values, string expecteddata)
		{
			ObjWithPrimitiveArray<T> obj = new ObjWithPrimitiveArray<T>();
			obj.Numbers = values;
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, expecteddata);

			stream.Position = 0;
			ObjWithPrimitiveArray<T>? result = BossSerializer.Deserialize<ObjWithPrimitiveArray<T>>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithPrimitiveArray<T>>());
			Assert.That(result.Numbers, Is.EqualTo(obj.Numbers));
		}

		public class ObjWithList
		{
			public List<int> Numbers { get; set; } = [];
		}

		[Test]
		public void PrimitiveList()
		{
			ObjWithList obj = new ObjWithList();
			obj.Numbers = [3, 2, 1];
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "1A-00-00-00-00-00-00-00-0F-01-01-0D-03-06-03-00-00-00-02-00-00-00-01-00-00-00-01-07-4E-75-6D-62-65-72-73");

			stream.Position = 0;
			ObjWithList? result = BossSerializer.Deserialize<ObjWithList>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithList>());
			Assert.That(result.Numbers, Is.EqualTo(obj.Numbers));
		}

		public class ObjWithQueue
		{
			public Queue<int> Numbers { get; set; } = [];
		}

		[Test]
		public void PrimitiveQueue()
		{
			ObjWithQueue obj = new ObjWithQueue();
			obj.Numbers = new Queue<int>();
			obj.Numbers.Enqueue(3);
			obj.Numbers.Enqueue(2);
			obj.Numbers.Enqueue(1);
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "1A-00-00-00-00-00-00-00-0F-01-01-0D-03-06-03-00-00-00-02-00-00-00-01-00-00-00-01-07-4E-75-6D-62-65-72-73");

			stream.Position = 0;
			ObjWithQueue? result = BossSerializer.Deserialize<ObjWithQueue>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithQueue>());
			Assert.That(result.Numbers, Is.EqualTo(obj.Numbers));
		}

		public class ObjWithStructArray
		{
			public Point[] Points { get; set; } = [];
		}

		[Test]
		public void StructArray()
		{
			ObjWithStructArray obj = new ObjWithStructArray();
			obj.Points = [new Point(1, 2), new Point(3, 4), new Point(5, 6)];
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "35-00-00-00-00-00-00-00-0F-01-01-0D-03-0F-02-02-06-01-00-00-00-03-06-02-00-00-00-02-02-06-03-00-00-00-03-06-04-00-00-00-02-02-06-05-00-00-00-03-06-06-00-00-00-03-06-50-6F-69-6E-74-73-01-58-01-59");

			stream.Position = 0;
			ObjWithStructArray? result = BossSerializer.Deserialize<ObjWithStructArray>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithStructArray>());
			Assert.That(result.Points, Is.EqualTo(obj.Points));
		}

		public class ObjWithStructList
		{
			public List<Point> Points { get; set; } = [];
		}

		[Test]
		public void StructList()
		{
			ObjWithStructList obj = new ObjWithStructList();
			obj.Points = [new Point(1, 2), new Point(3, 4), new Point(5, 6)];
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "35-00-00-00-00-00-00-00-0F-01-01-0D-03-0F-02-02-06-01-00-00-00-03-06-02-00-00-00-02-02-06-03-00-00-00-03-06-04-00-00-00-02-02-06-05-00-00-00-03-06-06-00-00-00-03-06-50-6F-69-6E-74-73-01-58-01-59");

			stream.Position = 0;
			ObjWithStructList? result = BossSerializer.Deserialize<ObjWithStructList>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithStructList>());
			Assert.That(result.Points, Is.EqualTo(obj.Points));
		}

		public abstract class SomeBaseObject { }
		public class DerivedObject1 : SomeBaseObject { }
		public class DerivedObject2 : SomeBaseObject
		{
			public int Number;
		}

		public class ObjWithDynamicArray
		{
			public SomeBaseObject?[] Objs { get; set; } = [];
		}

		[Test]
		public void DynamicArray()
		{
			ObjWithDynamicArray obj = new ObjWithDynamicArray();
			obj.Objs = [new DerivedObject1(), null, new DerivedObject2() { Number = 69 }];
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "1A-00-00-00-00-00-00-00-0F-01-01-0E-03-10-02-00-00-10-03-01-04-06-45-00-00-00-04-04-4F-62-6A-73-0E-44-65-72-69-76-65-64-4F-62-6A-65-63-74-31-0E-44-65-72-69-76-65-64-4F-62-6A-65-63-74-32-06-4E-75-6D-62-65-72");

			stream.Position = 0;
			ObjWithDynamicArray? result = BossSerializer.Deserialize<ObjWithDynamicArray>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithDynamicArray>());
			Assert.That(result.Objs, Is.InstanceOf<SomeBaseObject[]>());
			Assert.That(result.Objs.Length, Is.EqualTo(3));
			Assert.Multiple(() =>
			{
				Assert.That(result.Objs[0], Is.InstanceOf<DerivedObject1>());
				Assert.That(result.Objs[1], Is.Null);
				Assert.That(result.Objs[2], Is.InstanceOf<DerivedObject2>());
				Assert.That((result.Objs[2] as DerivedObject2)?.Number, Is.EqualTo(69));
			});
		}

		public class ObjWithDynamicList
		{
			public List<SomeBaseObject> Objs { get; set; } = [];
		}

		[Test]
		public void DynamicList()
		{
			ObjWithDynamicList obj = new ObjWithDynamicList();
			obj.Objs = [new DerivedObject1(), null, new DerivedObject2() { Number = 69 }];
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "1A-00-00-00-00-00-00-00-0F-01-01-0E-03-10-02-00-00-10-03-01-04-06-45-00-00-00-04-04-4F-62-6A-73-0E-44-65-72-69-76-65-64-4F-62-6A-65-63-74-31-0E-44-65-72-69-76-65-64-4F-62-6A-65-63-74-32-06-4E-75-6D-62-65-72");

			stream.Position = 0;
			ObjWithDynamicList? result = BossSerializer.Deserialize<ObjWithDynamicList>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithDynamicList>());
			Assert.That(result.Objs, Is.InstanceOf<List<SomeBaseObject>>());
			Assert.That(result.Objs.Count, Is.EqualTo(3));
			Assert.Multiple(() =>
			{
				Assert.That(result.Objs[0], Is.InstanceOf<DerivedObject1>());
				Assert.That(result.Objs[1], Is.Null);
				Assert.That(result.Objs[2], Is.InstanceOf<DerivedObject2>());
				Assert.That((result.Objs[2] as DerivedObject2)?.Number, Is.EqualTo(69));
			});
		}
	}
}

