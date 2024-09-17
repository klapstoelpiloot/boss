using System.Drawing;

namespace CodeImp.Boss.Tests
{
    internal class RoundtripTests : TestBase
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

        [Test]
        public void RoundtripNormalStream()
        {
            ObjWithAllFields obj = new ObjWithAllFields();
            MemoryStream stream = new MemoryStream();
            BossSerializer.Serialize(obj, stream);
            stream.Seek(0, SeekOrigin.Begin);
            ObjWithAllFields? result = BossSerializer.Deserialize<ObjWithAllFields>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithAllFields>());
            Assert.That(result.name, Is.EqualTo(obj.name));
            Assert.That(result.number, Is.EqualTo(obj.number));
            Assert.That(result.smallnumber, Is.EqualTo(obj.smallnumber));
        }

        [Test]
        public void RoundtripCompressedStream()
        {
            ObjWithAllFields obj = new ObjWithAllFields();
            MemoryStream stream = new MemoryStream();
            BossSerializer.SerializeCompressed(obj, stream);
            stream.Seek(0, SeekOrigin.Begin);
            ObjWithAllFields? result = BossSerializer.DeserializeCompressed<ObjWithAllFields>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithAllFields>());
            Assert.That(result.name, Is.EqualTo(obj.name));
            Assert.That(result.number, Is.EqualTo(obj.number));
            Assert.That(result.smallnumber, Is.EqualTo(obj.smallnumber));
        }
    }
}
