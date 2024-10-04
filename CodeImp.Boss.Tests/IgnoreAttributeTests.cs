namespace CodeImp.Boss.Tests
{
    internal class IgnoreAttributeTests : TestBase
    {
        public class ObjWithAllProperties
        {
            public int Age { get; set; } = 3;

            public int IgnoredProperty { get; set; } = 4;
        }

        public class ObjWithIgnoredProperty
        {
            public int Age { get; set; } = 6;

            [BossIgnore]
            public int IgnoredProperty { get; set; } = 8;
        }

        [Test]
        public void SerializeWithIgnoredProperty()
        {
            ObjWithIgnoredProperty obj = new ObjWithIgnoredProperty();
            obj.Age = 18;
            obj.IgnoredProperty = 1200;
            MemoryStream stream = new MemoryStream();
            BossConvert.ToStream(obj, stream);

            AssertStreamIsEqualTo(stream, "10-00-00-00-00-00-00-00-0F-01-01-06-12-00-00-00-01-03-41-67-65");

            stream.Seek(0, SeekOrigin.Begin);
            ObjWithAllProperties? result = BossConvert.FromStream<ObjWithAllProperties>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithAllProperties>());
            Assert.That(result.Age, Is.EqualTo(obj.Age));
            Assert.That(result.IgnoredProperty, Is.EqualTo(new ObjWithAllProperties().IgnoredProperty));
        }

        [Test]
        public void DeserializeWithIgnoredProperty()
        {
            ObjWithAllProperties obj = new ObjWithAllProperties();
            obj.Age = 18;
            obj.IgnoredProperty = 1200;
            MemoryStream stream = new MemoryStream();
            BossConvert.ToStream(obj, stream);

            AssertStreamIsEqualTo(stream, "16-00-00-00-00-00-00-00-0F-02-01-06-12-00-00-00-02-06-B0-04-00-00-02-03-41-67-65-0F-49-67-6E-6F-72-65-64-50-72-6F-70-65-72-74-79");

            stream.Seek(0, SeekOrigin.Begin);
            ObjWithIgnoredProperty? result = BossConvert.FromStream<ObjWithIgnoredProperty>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithIgnoredProperty>());
            Assert.That(result.Age, Is.EqualTo(obj.Age));
            Assert.That(result.IgnoredProperty, Is.EqualTo(new ObjWithIgnoredProperty().IgnoredProperty));
        }

        public class ObjWithAllFields
        {
            public int Age = 3;

            [BossIgnore]
            public int IgnoredField = 4;
        }

        public class ObjWithIgnoredField
        {
            public int Age = 6;

            [BossIgnore]
            public int IgnoredField = 8;
        }

        [Test]
        public void SerializeWithIgnoredField()
        {
            ObjWithIgnoredField obj = new ObjWithIgnoredField();
            obj.Age = 18;
            obj.IgnoredField = 1200;
            MemoryStream stream = new MemoryStream();
            BossConvert.ToStream(obj, stream);

            AssertStreamIsEqualTo(stream, "10-00-00-00-00-00-00-00-0F-01-01-06-12-00-00-00-01-03-41-67-65");

            stream.Seek(0, SeekOrigin.Begin);
            ObjWithAllFields? result = BossConvert.FromStream<ObjWithAllFields>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithAllFields>());
            Assert.That(result.Age, Is.EqualTo(obj.Age));
            Assert.That(result.IgnoredField, Is.EqualTo(new ObjWithAllFields().IgnoredField));
        }

        [Test]
        public void DeserializeWithIgnoredField()
        {
            ObjWithAllFields obj = new ObjWithAllFields();
            obj.Age = 18;
            obj.IgnoredField = 1200;
            MemoryStream stream = new MemoryStream();
            BossConvert.ToStream(obj, stream);

            AssertStreamIsEqualTo(stream, "10-00-00-00-00-00-00-00-0F-01-01-06-12-00-00-00-01-03-41-67-65");

            stream.Seek(0, SeekOrigin.Begin);
            ObjWithIgnoredField? result = BossConvert.FromStream<ObjWithIgnoredField>(stream);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ObjWithIgnoredField>());
            Assert.That(result.Age, Is.EqualTo(obj.Age));
            Assert.That(result.IgnoredField, Is.EqualTo(new ObjWithIgnoredField().IgnoredField));
        }
    }
}
