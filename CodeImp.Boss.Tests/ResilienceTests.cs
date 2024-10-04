using System.Drawing;

namespace CodeImp.Boss.Tests
{
    internal class ResilienceTests : TestBase
    {
        public class ObjWithAllFields
        {
            [BossSerializable(DefaultValueBehavior = DefaultValueBehavior.Include)]
            public string name = "Darth Vader";

            [BossSerializable(DefaultValueBehavior = DefaultValueBehavior.Include)]
            public int number = 42;

            [BossSerializable(DefaultValueBehavior = DefaultValueBehavior.Include)]
            public double smallnumber = 3.1415926535897932384626;
        }

        public class ObjWithMissingFields
        {
            public string name = string.Empty;
        }

        [Test]
        public void FieldsMissing()
        {
            ObjWithAllFields obj = new ObjWithAllFields();
            MemoryStream stream = new MemoryStream();
            BossConvert.ToStream(obj, stream);

            stream.Seek(0, SeekOrigin.Begin);
            ObjWithMissingFields? result = BossConvert.FromStream<ObjWithMissingFields>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithMissingFields>());
            Assert.That(result.name, Is.EqualTo(obj.name));
        }

        public class ObjWithChangedFields
        {
            public string name = string.Empty;
            public byte number;
            public float smallnumber;
        }

        [Test]
        public void FieldsChanged()
        {
            ObjWithAllFields obj = new ObjWithAllFields();
            MemoryStream stream = new MemoryStream();
            BossConvert.ToStream(obj, stream);

            stream.Seek(0, SeekOrigin.Begin);
            ObjWithChangedFields? result = BossConvert.FromStream<ObjWithChangedFields>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithChangedFields>());
            Assert.That(result.name, Is.EqualTo(obj.name));
            Assert.That(result.number, Is.EqualTo(Convert.ToByte(obj.number)));
            Assert.That(result.smallnumber, Is.EqualTo(Convert.ToSingle(obj.smallnumber)));
        }

        public class ObjWithIncompatibleFields
        {
            public int name;
            public ObjWithAllFields? number;
            public Point smallnumber;
        }

        [Test]
        public void IncompatibleFieldsWithException()
        {
            ObjWithAllFields obj = new ObjWithAllFields();
            MemoryStream stream = new MemoryStream();
            BossConvert.ToStream(obj, stream);

            stream.Seek(0, SeekOrigin.Begin);
            
            BossSerializer serializer = new BossSerializer();
            serializer.ThrowOnDeserializationFailure = true;
            Assert.Catch<BossSerializationException>(() => serializer.Deserialize<ObjWithIncompatibleFields>(stream));
        }

        [Test]
        public void IncompatibleFields()
        {
            ObjWithAllFields obj = new ObjWithAllFields();
            MemoryStream stream = new MemoryStream();
            BossConvert.ToStream(obj, stream);

            stream.Seek(0, SeekOrigin.Begin);
            
            BossSerializer serializer = new BossSerializer();
            serializer.ThrowOnDeserializationFailure = false;
            ObjWithIncompatibleFields? result = serializer.Deserialize<ObjWithIncompatibleFields>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithIncompatibleFields>());
            Assert.That(result.name, Is.EqualTo(0));
            Assert.That(result.number, Is.EqualTo(null));
            Assert.That(result.smallnumber, Is.EqualTo(new Point()));
        }
    }
}
