using CodeImp.Boss.TypeHandlers;

namespace CodeImp.Boss.Tests
{
	internal class CustomTypeHandlerTests : TestBase
	{
		public struct Vector3
		{
			public float x;
			public float y;
			public float z;

			public Vector3(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public Vector3 Vec
			{
				get => new Vector3(x, y, z);
				set
				{
					x = value.x;
					y = value.y;
					z = value.z;
				}
			}
		}

		public class Vector3TypeHandler : BossTypeHandler
		{
			public Vector3TypeHandler()
			{
				BossType = 64;
				ClassTypes = [typeof(Vector3)];
			}

			public override object? ReadFrom(BossReader reader, Type basetype)
			{
				Vector3 v = new Vector3();
				v.x = reader.ReadSingle();
				v.y = reader.ReadSingle();
				v.z = reader.ReadSingle();
				return v;
			}

			public override void WriteTo(BossWriter writer, object value)
			{
				Vector3 v = (Vector3)value;
				writer.Write(v.x);
				writer.Write(v.y);
				writer.Write(v.z);
			}
		}

		public class ObjWithVector3
		{
			public Vector3 Pos { get; set; }
		}

		[Test]
		public void CustomTypeHandler()
		{
			BossSerializer.RegisterTypeHandler(new Vector3TypeHandler());

			ObjWithVector3 obj = new ObjWithVector3();
			obj.Pos = new Vector3(1.0f, 2.0f, 3.0f);
			MemoryStream stream = new MemoryStream();
			BossSerializer.Serialize(obj, stream);

			AssertStreamIsEqualTo(stream, "18-00-00-00-00-00-00-00-0E-01-01-40-00-00-80-3F-00-00-00-40-00-00-40-40-01-03-50-6F-73");

			stream.Seek(0, SeekOrigin.Begin);
			ObjWithVector3? result = BossSerializer.Deserialize<ObjWithVector3>(stream);
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<ObjWithVector3>());
			Assert.That(result.Pos, Is.InstanceOf<Vector3>());
			Assert.That(result.Pos.x, Is.EqualTo(1.0f));
			Assert.That(result.Pos.y, Is.EqualTo(2.0f));
			Assert.That(result.Pos.z, Is.EqualTo(3.0f));
		}

	}
}
