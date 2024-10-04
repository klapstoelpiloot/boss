# Binary Object Serialization and Storage

This is a software library and file format to serialize C# objects to a binary file or data stream. It is designed for fast and compact storage while keeping the flexibility of allowing changes in the model being serialized/deserialized. To allow this flexibility, the data must be serialized along with the names and types of the model members and deserialized with a little resilience towards missing members and type changes.

Since [Microsoft has banned the BinaryFormatter for security risks](https://learn.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide), the Boss serialization is a good alternative to Json when you need to get your data smaller and still want the flexibility that Json provides.

## Usage
To serialize an object to a binary data stream, call one of the static methods in the `BossConvert` class like this:
```C#
BossConvert.ToStream(obj, stream);
```
To deserialize a binary data stream back to an object, call the deserialization equivalent method:
```C#
MyClass? result = BossConvert.FromStream<MyClass>(stream);
```
There are also convenient methods that allow serialization to byte arrays and Base64 strings as well. Each method also has an optional `compress` parameter that you can set to `true` to squeeze the data a little smaller.

You can use the `BossIgnore` attribute on field and property members to indicate that they should not be serialized or deserialized:
```C#
public class ObjWithIgnoredProperty
{
    public int Age { get; set; }

    [BossIgnore]
    public int IgnoredProperty { get; set; }
}
```
Use the `BossSerialize` attribute to control more complex behaviors. Set the `DefaultValueBehavior` setting to `DefaultValueBehavior.Include` to force the serializer to include this field or property even when the value is the default value.
```C#
public class ObjWithClassObjDefaultNullInclude
{
    [BossSerializable(DefaultValueBehavior = DefaultValueBehavior.Include)]
    public ObjWithInt? ClassObj { get; set; } = null;
}
```
Set the `Polymorphic` settings to `true` to force the serializer to serialize this field or property as a dynamic object (causes data type information to be included). You don't always need this. When polymorphics members are defined using interfaces or abstract classes, they are automatically stored as dynamic objects. But in some cases you want to apply this attribute where polymorphism is needed, just to be sure.
```C#
public class ObjWithPolymorhpicMember
{
    [BossSerializable(Polymorphic = true)]
    public BaseClass ClassObj { get; set; } = new DerivedClass();
}
```
To control how Enumerations are serialized, you can use the `BossEnumOptions` attribute. This allows you to choose the method of serialization (by value or by enum member name). Obviously, serializing the members by name will generate a larger amount of data. But it provides the flexibility of changing your member values without breaking compatibility with your serialized data.
```C#
[BossEnumOptions(Method = EnumSerializationMethod.MemberNames)]
public enum Forces
{
    None = 0,
    Gravity = 1,
    Electromagnetism = 2,
    Weak = 3,
    Strong = 4
};
```


## Type handlers
A type handler controls how a specific datatype is serialized and deserialized. For most of the basic types there is already a type handler implemented internally. However, you can (and should) write your own type handler for classes/structs that are unlikely to change structure. This can significantly reduce the size of the data written and improve performance.

For example, in Unity there is a `Vector3` struct which has 3 float members: x, y and z. Without a custom type handler, this struct is serialized inefficiently. For each member the name and the data type are serialized. But this struct is unlikely to change in the future, so you can write a more efficient type handler which serializes the `Vector3` simply to 3 floating points (3 x 4 bytes).

A type handler must derive from the abstract `BossTypeHandler` class. It must also have a unique number in the range of 64 through 255 (inclusive) which is called the type code. This means you are limited to 192 custom type handlers. The type handler implements serialization in the `WriteTo` method and deserialization in the `ReadFrom` method. Note that the type handler must be stateless and thread-safe. This means you cannot temporarily store data in class members.

Here is a good example for the Unity Vector3:
```C#
public class Vector3Handler : BossTypeHandler
{
    // This is the Boss typecode that this handler will deal with.
    public override byte BossType => 64;
    
    // .NET class type that this handler will deal with.
    public override Type? ClassType => typeof(Vector3);

    // Called to serialized an object to stream.
    // The data written must match exactly with the data read in the ReadFrom method.
    public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
    {
        Vector3 v = (Vector3)value;
        writer.Write(v.x);
        writer.Write(v.y);
        writer.Write(v.z);
    }

    // Called to deserialize an object from stream.
    // The data read must match exactly with the written data in the WriteTo method.
    public override object? ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        return new Vector3(x, y, z);
    }
}
```
You must register your custom type handler by calling the `RegisterTypeHandler` method on the `BossSerializer` class, somewhere at the start of your application.
```C#
BossSerializer.RegisterTypeHandler(new Vector3Handler());
```

## Storage format
In this notation we use the datatype indication `vlq` for a flexible number of bytes indicating an integer number (VLQ code) up to the size of an int (2,147,483,647). For more information see https://en.m.wikipedia.org/wiki/Variable-length_quantity

The data stream starts with a `long` which indicates the position (in number of bytes) where the strings table begins (see below).

Then follows the root element. The type of the root element is first indicated with a type code, which is a single `byte`. After that follows the contents for that type. For example, if the type is an `int` then only one `int` follows. The root element is often a **fixed type object** and thus a `vlq` indicating the number of elements in the object follows. Then follow the elements of that object (see **fixed type object** below for more information).

Primitive elements: **int, uint, long, ulong, float, double, bool, byte, sbyte, short, ushort**.
These are just written as is. So for an integer an `int` is written. The size of this element and how it should be read is directly known from this element type.

**String** element: Written as `vlq` and stored in strings table.

Custom element types can be implemented by way of extension. For example **Vector2, Vector3, Vector4, Plane, Quaternion**... Same as primitive elements, we can directly write the known members. We do not expect these to change. So for **Vector3** that would be a `float` for X, a `float` for Y and a `float` for Z.

**Fixed Object**: First a `vlq` for number of elements. Then follow the elements. For each element there is first a `vlq` which is the index in the string table for the Name of this element. Then a `byte` which indicates the type of element that follows. When bit 8 is set, then an array of that element type follows.

**Dynamic Object**: First a `vlq` that is the index in strings table for the class type name of the object. Then same as **fixed type object**.

**Fixed Array**: First a `vlq` indicating the number of items. Then a `byte` for the typecode of the elements. All elements are the same type. After that each element is written as described by the typecode.

**Dynamic Array**: First a `vlq` indicating the number of items. The each element follows, beginning with a `byte` for the typecode of the element.

**Fixed Dictionary**: Key-value pairs with one type of all keys and one type for all values. First `byte` indicates the type of the key. A second `byte` indicates the type of all values. Then a `vlq` follows for the number of elements. Then the key is written according to its type and then the value is written according to its type.

**Dynamic Dictionary**: Key-value pairs with variable type keys and variable type values. First a `vlq` for the number of elements. Then for each there is a `byte` that indicates the type of the key and a `byte` for the type of the value. Then the key is written according to its type and then the value is written according to its type.

After all elements follows the strings table. Every string starts with a `vlq` for the length of the string in number of bytes (not characters) and then follows the string in UTF8 format. There is no null terminator.
