# Adeptar
A small .NET file extension aimed to be fast and easy to use, aimed for modding purposes, with manual work in mind.
The current source code is not to be used, as development was abandoned half way through a refactor attempt.
For source code you need the last commit before the current 1.1 release.

An example:

```cs
public class ComponentClass
{  
	public int ItemType;
	public int Id;
	public string Name;
	public int DisplayCategory;
	public int Availability;
	public int ComponentStatsId;
	public int Faction;
	public int Level;
	public string Icon;
	public string Color;
	public string Layout;
	public string CellType;
	public int WeaponId;
	public int AmmunitionId { get; set; }
	public string WeaponSlotType;
	public int[] PossibleModifications;
	public Dictionary<int, int> dict;
}
```

Is serialized as:

```cs
{
        AmmunitionId: 1002,
        Availability: 4,
        CellType: "M",
        Color: "#23444",
        ComponentStatsId: 999,
        dict: [
                1:222,
                2:333
        ],
        DisplayCategory: 3,
        Faction: 123,
        Icon: "weapon.png",
        Id: 1000,
        ItemType: 4,
        Layout: "1020340303232311222000",
        Level: 400,
        Name: "Some weapon",
        PossibleModifications: [
                1,
                2,
                3,
                5
        ],
        WeaponId: 1001,
        WeaponSlotType: "L"
}
```
When reading from a file order of properties or fields doesn't matter.
When deserializing Adeptar ignores what would be a field or a property if it doesn't belong to the class or struct.

----------------------------------------------------------------
<h1>Adeptar Unity</h1>
A port of Adeptar for Unity, be default such libraries like this are not compatible with Unity.

When using with Unity, the library is not to be used with any class that inherits a Unity class ( like Monobehaviour ).
Instead you serialize and deserialize a class like this.
```cs
    [System.Serializable]
    public class UnitBuild
    {
        public DroneData[] Drones;
        public BarrelEntry[] Weapons;
        public Quality Quality;
    }
```
To port drag the "Adeptar Unity" folder into the Unity project's folder.
----------------------------------------------------------------
<h1>AdeptarConverter</h1>
 
This class provides methods for the conversion of objects into .Adeptar and vise versa. Such as:

```cs
// Serializes the object to a .Adeptar string.
// Accepts an AdeptarSettings object to set serialization rules.
- AdeptarConverter.Serialize()

// Serializes the object to a .Adeptar string using specified formatting style, and appends it to a file.
// If an object with the provided id already exists will throw an exception.
// The id is limited to numbers and letters. Accepts an AdeptarSettings object to set serialization rules.
- AdeptarConverter.SerializeAppend()

// Serializes the object to a .Adeptar string using specified formatting style, and appends it to a file.
// The current object with the provided id is rewritten in the file.
// The id is limited to numbers and letters. Accepts an AdeptarSettings object to set serialization rules.
- AdeptarConverter.SerializeRewriteAppended()

// Sets or rewrites the shared data of an ID object collection using specified formatting style.
// Fields or properties with null or default values are not serialized.
- AdeptarConverter.SerializeSetShared()

// Serializes the object to a .Adeptar string and writes it to a file, the file is overwritten.
// Accepts an AdeptarSettings object to set serialization rules.
- AdeptarConverter.SerializeWrite()

// Deserializes the .Adeptar string from the file at the specified path ( file ) to the .NET 
- AdeptarConverter.Deserialize()
- AdeptarConverter.Deserialize<T>()

// Deserializes an object serialized at the specified path ( file ) with the ID feature.
- AdeptarConverter.DeserializeAppended()
- AdeptarConverter.DeserializeAppended<T>()

// Deserializes an object serialized with the ID feature using shared data of the object pool
// to override field and property values at deserialization.
- AdeptarConverter.DeserializeAppendedWithSharedData<T>()

// Deserializes the .Adeptar string to the specified .NET type.
- AdeptarConverter.DeserializeString()
- AdeptarConverter.DeserializeString<T>()
```
----------------------------------------------------------------

<h1> AdeptarConfiguration </h1>

AdeptarConfiguration is a class that provides functionality for field/property serialization manipulations.
An example usage:

```cs
class MyClassWithConfig
{
	// AdeptarConfiguration is put directly in the class.
	// It is ignored by the AdeptarWriter and the ClassWriter.
	public AdeptarConfiguration Config = new()
	{
	};
	public SerializableType Type = SerializableType.Ignore;
	public int Number;
	public int Number2;
	public int[] IntArray;
	public Dictionary<int, string> SimpleDictionary;
	public DateTime Date;
}
```

Currently AdeptarConfiguration has only 1 use case, which is to tell the AdeptarWriter which properties or fields to not serialize.
It can be done like so.

```cs
class MyClassWithConfig
{
	// AdeptarConfiguration is put directly in the class.
	// It is ignored by the AdeptarWriter and the ClassWriter.
	public AdeptarConfiguration Config = new()
	{
		// The fields IntArray and SimpleDictionary will not be serialized.
		ToIgnore = new string[] { "IntArray", "SimpleDictionary" }
	};
	public SerializableType Type = SerializableType.Ignore;
	public int Number;
	public int Number2;
	public int[] IntArray;
	public Dictionary<int, string> SimpleDictionary;
	public DateTime Date;
}
```

An alternative to AdeptarConfiguration is the following attribute:
```cs
[AdeptarIgnore]
```
Which is used like this:
```cs
class MyClassWithConfig
{
	[AdeptarIgnore] // This field will be ignored by the AdeptaWriter.
	public SerializableType Type = SerializableType.Ignore;
	public int Number;
	public int Number2;
	public int[] IntArray;
	public Dictionary<int, string> SimpleDictionary;
	public DateTime Date;
}
```
AdeptarIgnore attribute requires custom AdeptarSettings, by default the AdeptarWriter will look for an AdeptarConfiguration object.
It is highly recommended to use AdeptarConfiguration, it is several times faster to instruct the AdeptarWriter to not serialize properties or fields.
Than to use the [AdeptarIgnore] attribute.

----------------------------------------------------------------

<h1> AdeptarDynamic </h1>

A class for deserializing .Adeptar objects without knowing their types at first.
Such as when multiple different .Adeptar objects are serialized in the the same file using the ID feature.
Class is restricted for objects of type class or struct.

```cs
// An AdeptarDynamic object can be created via:

// Creates an empty AdeptarDynamic object.
AdeptarDynamic obj = new AdeptarDynamic();

// Creates an AdeptarDynamic object and feeds it a .Adeptar string from the file at the given path as data.
AdeptarDynamic obj = new AdeptarDynamic().FromFile();

// Creates an AdeptarDynamic object and feeds it a .Adeptar string as data.
AdeptarDynamic obj = new AdeptarDynamic().FromString();


// The data of an AdeptarDynamic object can be deserialized like this:

string str = AdeptarConverter.Serialize(new TestClass());

AdeptarDynamic obj = new AdeptarDynamic().FromString(str);

TestClass val = obj.Deserialize<TestClass>();
```

Using one AdeptarDynamic for multiple class types.
```cs
class Test1
{
	public int Num;
	public int Num2
}

class Test2
{
	public int Num2;
	public int Num3;
}

AdeptarDynamic obj = new AdeptarDynamic().FromString(AdeptarConverter.Serialize( new Test1() ) ); 

// Both are valid.
// Since the AdeptarDynamic was fed a .Adeptar string of a Test1 class, this will assign both Num and Num2 values.
Test1 class1 = obj.Deserialize<Test1>();

// Since the AdeptarDynamic was fed a .Adeptar string of a Test1 class, this will assign only the Num2 value to the class.
// Since it is the only field with a name that is present in both class types.
Test2 class2 = obj.Deserialize<Test2>();
```

Other AdeptarDynamic functionalities are:
```cs
AdeptarDynamic obj;

// Checks if the AdeptarDynamic object contains the field/property with the given name.
obj.ContainsKey( "FieldOrPropertyName" );

// Takes a key name of a field/property. If the key is not found throws an exception.
// If the key is found deserializes its .Adeptar string to the provided type.
obj.GetValue<T>( "FieldOrPropertyName" );

// For example
class Test1
{
	public int Num;
	public int Num2
}

obj = new AdeptarDynamic().FromString(AdeptarConverter.Serialize( new Test1() ) ); 
int num;

if (obj.ContainsKey( "Num2" )) {
	num = obj.GetValue<int>( "Num2" )
}

// Fetches a Dictionary<string, string> where the keys are the
// property/field names, and the values are their .Adeptar strings.
var x = obj.KeyMaps;

// Given 2 class types
class Test1
{
	public int Num1;
	public int Num2;
}

class Test2
{
	public int Num3;
	public int Num4;
}

// You can use a "map" to assign the values of Test2 to Test1.
// Deserializes the AdeptarDynamic object to the provided .Net type.
// Accepts a Dictionary<string,string> map for field/property mapping.
Test1 class1 = obj.DeserializeWithMap<Test1>( new Dictionary<string, string>() {} );

// For example.

AdeptarDynamic obj = new AdeptarDynamic().FromString(AdeptarConverter.Serialize( new Test2() ) );

// The value of Num3 will be assigned to Num1 and the value of Num4 assigned to Num2.
Test1 x = obj.DeserializeWithMap<Test1>(new Dictionary<string, string>() { { "Num3", "Num1" }, { "Num4", "Num2" } } );
```
----------------------------------------------------------------

<h2> AdeptarHelpers & TypeGetters </h2>
AdeptarHelpers is a class for helper methods for different uses.

TypeGetters is a class that provides methods to determine object types.
	
```cs
// For AdeptarHelpers currently only:
// Checks if the file at the given path already has an object appended using the Id feature with the specified id.
// The id is limited to numbers and letters.
if (AdeptarHelpers.ContainsId( "path", "id" )) {
	...
}
```
----------------------------------------------------------------

<h1> AdeptarSettings </h1>
A class that controls object serialization rules used in AdeptarConverter methods.

```cs
// Example
AdeptarSettings settings = new AdeptarSettings();
string str = AdeptarConverter.Serialize( new TestClass(), settings ));
```

AdeptarSettings can be used to:
- Specify whether to serialize with indentation.
- Check Class Attributes ( such as AdeptarIgnore ).
- Specify whether to serialize fields/properties that have default values.
- Specify whether to serialize fields/properties that are null.

If you don't specify custom AdeptarSettings, the AdeptarWriter will use the default settings when serializing, which are:
```cs
internal static AdeptarSettings DefaultSettings = new()
{
	CheckClassAttributes = false,
	UseIndentation = true,
	IgnoreDefaultValues = false,
	IgnoreNullValues = false,
};
```
----------------------------------------------------------------

<h1>ID Feature</h1>
Index feature is used to serialize multiple objects into one.
ach object that is appended to a file using the ID feature musthave a unique ID. The ID is used to find the necessary object. A file cant contain two objects with the same ID, if you try to serialize an object with an already existing ID, an exception will be thrown. The ID is taken as a string. The id is limited to numbers and letters.

----------------------------------------------------------------
```cs
// Appends the .Adeptar string of the provided object to the file at the given path with an id.
Adeptar.AdeptarConverter.SerializeAppend( path, new int[] { 1, 3, 5, 7, 9 }, "Odd", Adeptar.Formatting.NoIndentation );
Adeptar.AdeptarConverter.SerializeAppend( path, new int[] { 2, 4, 6, 8 }, "Even", Adeptar.Formatting.NoIndentation );
Adeptar.AdeptarConverter.SerializeAppend( path, new int[] { 1, 12, 6, -3, 0 }, "Random", Adeptar.Formatting.NoIndentation );

// This creates a file at the path with the following content.

~Odd~
[1,3,5,7,9]
~Even~
[2,4,6,8]
~Random~
[1,12,6,-3,0]

// Deserializing appended objects.

int[] obj = AdeptarConverter.DeserializeAppended<int[]>( path, "Even" );
// Outputs "2".
Console.WriteLine( obj[0] );

// Rewriting appended objects.
// With this as data.

~Odd~
[1,3,5,7,9]
~Even~
[2,4,6,8]
~Random~
[1,12,6,-3,0]

// This will rewrite the "Random" integer array in the ID feature file.
AdeptarConverter.SerializeRewriteAppended( path, new int[] { -1, -5, 0, 2, 5 }, "Random" );

// The contents of the file will become this.
~Odd~
[1,3,5,7,9]
~Even~
[2,4,6,8]
~Random~
[-1,-5,0,2,5]
```

<h2> Shared Data In Id Feature </h2>

Multiple .Adeptar objects in a file can have Shared data.

```cs
// For Example.

class Test1
{
	public string Name;
	public int Age;
	public string Gender;
}

// This puts 3 Test1 objects into a file, and sets their shared data.
// In this case the "Gender" field.
AdeptarConverter.SerializeSetShared( serializePath, new Test1() { Gender = "Male" } );
AdeptarConverter.SerializeAppend( serializePath, new Test1() { Name = "Mike", Age = 20, Gender = "Male" }, "Mike" );
AdeptarConverter.SerializeAppend( serializePath, new Test1() { Name = "Tom", Age = 22, Gender = "Male" }, "Tom" );
AdeptarConverter.SerializeAppend( serializePath, new Test1() { Name = "Ann", Age = 35, Gender = "Female" }, "Ann" );

// The following is generated in the file.
&{
	Gender: "Male",
}&
~Mike~
{
	Age: 20,
	Gender: "Male",
	Name: "Mike"
}
~Tom~
{
	Age: 22,
	Gender: "Male",
	Name: "Tom"
}
~Ann~
{
	Age: 35,
	Gender: "Female",
	Name: "Ann"
}

// By default you can get an object by:
Test1 x = AdeptarConverter.DeserializeAppended<Test1>( path, "Tom" );

// To use shared data call AdeptarConverter.DeserializeAppendedWithSharedData<>()
Test1 y = AdeptarConverter.DeserializeAppendedWithSharedData<Test1>(serializePath, "Ann");

// This will output "Male" because the shared data in this instance is the "Gender" field, which is set to "Male".
Console.Writeline(y.Gender);
```
