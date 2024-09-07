# Binary Object Serialization and Storage

This is a software library and file format to serialize C# objects to a binary file or data stream. It is designed for compact storage without the performance overhead of compression.

## File format
In this notation we use the datatype indication `vlq` for a flexible number of bytes indicating a positive number (VLQ code) up to the size of an int (2,147,483,647). For more information see https://en.m.wikipedia.org/wiki/Variable-length_quantity

The data stream starts with a `long` which indicates the position (in number of bytes) where the strings table begins (see below).

Then follows the root element, which is always a **fixed type object**. Note that the type and name are ommitted, because the type is known and no name is needed (this is root the object being serialized/deserialized). First a `vlq` indicating the number of elements in the object. Then follow the elements.

For each element:

`byte`
Indicates the type of element that follows. When bit 8 is set, then an array of that element type follows.

`vlq`
Index in the string table for the Name of this element.

Primitive elements: **int, uint, long, ulong, float, double, bool, byte, sbyte, short, ushort**.
These are just written as is. So for an integer an `int` is written. The size of this element and how it should be read is directly known from this element type.

**String** element: Written as `vlq` and stored in strings table.

Custom element types can be implemented by way of extension. For example **Vector2, Vector3, Vector4, Plane, Quaternion**... Same as primitive elements, we can directly write the known members. We do not expect these to change. So for **Vector3** that would be a `float` for X, a `float` for Y and a `float` for Z.

Array of primitives or strings: First a `vlq` indicating the number of items. Then each primitive item is written as described above.

**Fixed type object**: 
Same as above. First a `vlq` for number of elements. Then follow the elements.

**Dynamic type object**:
First a `vlq` that is the index in strings table for the class type name of the object. Then same as **fixed type object**.

Arrays of fixed and dynamic objects work the same as for primitive types: First a `vlq` for the number of elements and then the fixed or dynamic objects follow.

After all elements follows the strings table. Every string starts with a `vlq` for the length of the string in number of bytes (not characters) and then follows the string in UTF8 format. There is no null terminator.

Multidimensional arrays:
Set both the Type to **Array** and the array bit (8) high to indicate the array will contain arrays. Another `byte` follows to indicate if it will contain more dimensions or the data type of the elements in the array.

Dictionaries and lists of key-value pairs:
**Fixed Dictionary** has string keys and one type for all values. A single `byte` indicates the type of all values. Then a `vlq` follows for the number of elements. Each element begins with a `vlq` for the index in the strings table for the key. The bytes after that depend on the element type.

**Dynamic Dictionary** has string keys and each value can be of a different type. First a `vlq` for the number of elements. Each element is the same as for an object.
