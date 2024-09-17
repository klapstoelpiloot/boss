# Binary Object Serialization and Storage

This is a software library and file format to serialize C# objects to a binary file or data stream. It is designed for fast and compact storage while keeping the flexibility of allowing changes in the model being serialized/deserialized. To allow this flexibility, the data must be serialized along with the names and types of the model members and deserialized with a little resilience towards missing members and type changes.

## File format
In this notation we use the datatype indication `vlq` for a flexible number of bytes indicating a positive number (VLQ code) up to the size of an int (2,147,483,647). For more information see https://en.m.wikipedia.org/wiki/Variable-length_quantity

The data stream starts with a `long` which indicates the position (in number of bytes) where the strings table begins (see below).

Then follows the root element. The type of the root element is first indicated with a `byte`. After that follows the contents for that type. For example, if the type is an `int` then only one `int` follows. The root element is often a **fixed type object** and thus a `vlq` indicating the number of elements in the object follows. Then follow the elements of that object (see **fixed type object** below for more information).

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
