# Adeptar
A small .NET file extension aimed to be fast and easy to use, aimed for modding purposes, with manual work in mind.

First some benchmarks, both serialization and deserialization.
Benchmakrs are compared to NewtonSoft.Json library
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1706 (21H2)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT
```

```
|                                   Method |        Mean |      Error |     StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|----------------------------------------- |------------:|-----------:|-----------:|------------:|-------:|-------:|----------:|
|                        ClassAdeptarEmpty | 1,087.05 ns |  21.382 ns |  20.001 ns | 1,087.85 ns | 0.0839 |      - |     528 B |
|                           ClassJsonEmpty | 1,250.78 ns |  92.865 ns | 264.950 ns | 1,100.23 ns | 0.2613 |      - |   1,640 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                             ClassAdeptar | 2,934.19 ns |  51.595 ns |  48.262 ns | 2,941.56 ns | 0.1984 |      - |   1,256 B |
|                                ClassJson | 3,471.10 ns |  66.198 ns |  78.804 ns | 3,480.28 ns | 0.3777 |      - |   2,392 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                ClassAdeptarNoIndentation | 2,833.97 ns |  54.562 ns |  60.645 ns | 2,842.79 ns | 0.1984 |      - |   1,256 B |
|                   ClassJsonNoIndentation | 3,544.21 ns | 110.422 ns | 309.635 ns | 3,643.85 ns | 0.3777 |      - |   2,392 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                             TupleAdeptar | 3,653.29 ns |  72.908 ns |  81.037 ns | 3,629.46 ns | 0.1602 | 0.0038 |   1,024 B |
|                                TupleJson | 3,802.93 ns |  75.430 ns |  89.794 ns | 3,788.11 ns | 0.3395 |      - |   2,152 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|            DictionaryWithArrayKeyAdeptar | 1,890.86 ns |  91.686 ns | 270.339 ns | 1,843.84 ns | 0.1469 |      - |     928 B |
|               DictionaryWithArrayKeyJson | 2,418.28 ns | 148.710 ns | 438.476 ns | 2,304.53 ns | 0.3433 |      - |   2,176 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                        DictionaryAdeptar |   463.47 ns |  36.369 ns | 107.233 ns |   416.27 ns | 0.0725 |      - |     456 B |
|                           DictionaryJson | 1,036.20 ns |  41.037 ns | 118.401 ns | 1,055.12 ns | 0.2651 |      - |   1,672 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|              FourDimensionalArrayAdeptar | 5,220.65 ns |  96.863 ns |  80.885 ns | 5,227.68 ns | 0.1984 |      - |   1,248 B |
|                 FourDimensionalArrayJson | 5,443.65 ns | 107.571 ns | 219.739 ns | 5,462.09 ns | 0.4044 |      - |   2,568 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                        NestedListAdeptar | 1,506.38 ns |  30.058 ns |  66.606 ns | 1,512.89 ns | 0.1259 |      - |     800 B |
|                           NestedListJson | 2,452.20 ns |  47.772 ns |  84.915 ns | 2,446.16 ns | 0.3090 |      - |   1,952 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                             ArrayAdeptar | 1,684.29 ns |  51.335 ns | 151.362 ns | 1,629.37 ns | 0.0706 |      - |     448 B |
|                                ArrayJson | 1,734.95 ns |  39.289 ns | 113.359 ns | 1,733.54 ns | 0.2480 |      - |   1,568 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                            StringAdeptar |    86.15 ns |   2.222 ns |   6.193 ns |    84.84 ns | 0.0076 |      - |      48 B |
|                               StringJson |   276.81 ns |   5.316 ns |   5.689 ns |   275.68 ns | 0.2036 | 0.0005 |   1,280 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                              LongAdeptar |    77.87 ns |   1.580 ns |   1.478 ns |    77.09 ns | 0.0191 |      - |     120 B |
|                                 LongJson |   245.58 ns |   4.372 ns |   4.859 ns |   246.60 ns | 0.2074 |      - |   1,304 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                              BoolAdeptar |    39.26 ns |   0.782 ns |   0.768 ns |    39.27 ns | 0.0089 |      - |      56 B |
|                                 BoolJson |   200.52 ns |   3.680 ns |   3.443 ns |   200.86 ns | 0.1898 | 0.0005 |   1,192 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                            DoubleAdeptar |   216.04 ns |   3.820 ns |   3.573 ns |   216.75 ns | 0.0191 |      - |     120 B |
|                               DoubleJson |   391.66 ns |   7.203 ns |   6.738 ns |   392.98 ns | 0.1998 | 0.0005 |   1,256 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                              EnumAdeptar |    61.84 ns |   1.253 ns |   1.492 ns |    62.05 ns | 0.0101 |      - |      64 B |
|                                 EnumJson |   201.31 ns |   3.848 ns |   3.599 ns |   201.82 ns | 0.1886 | 0.0005 |   1,184 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                              ListAdeptar |   402.53 ns |   7.549 ns |   7.062 ns |   399.45 ns | 0.0443 |      - |     280 B |
|                                 ListJson |   644.88 ns |  12.821 ns |  11.993 ns |   649.81 ns | 0.2394 | 0.0010 |   1,504 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                  ClassAdeptarDeserialize | 2,643.98 ns |  52.244 ns |  51.310 ns | 2,665.43 ns | 0.2632 |      - |   1,672 B |
|                     ClassJsonDeserialize | 2,880.33 ns |  48.169 ns |  45.057 ns | 2,874.44 ns | 0.5684 | 0.0038 |   3,576 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                  TupleAdeptarDeserialize | 4,729.70 ns |  91.846 ns |  90.205 ns | 4,738.46 ns | 0.3738 |      - |   2,392 B |
|                     TupleJsonDeserialize | 5,169.00 ns |  99.131 ns |  97.360 ns | 5,173.68 ns | 0.6180 |      - |   3,904 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
| DictionaryWithArrayKeyAdeptarDeserialize | 1,564.01 ns |  27.140 ns |  25.387 ns | 1,562.37 ns | 0.1602 |      - |   1,016 B |
|    DictionaryWithArrayKeyJsonDeserialize | 2,011.21 ns |  26.142 ns |  24.453 ns | 2,015.49 ns | 0.5493 |      - |   3,464 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|             DictionaryAdeptarDeserialize |   616.63 ns |  12.179 ns |  27.490 ns |   611.89 ns | 0.0887 |      - |     560 B |
|                DictionaryJsonDeserialize |   788.00 ns |  15.076 ns |  14.103 ns |   791.01 ns | 0.4644 |      - |   2,920 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|   FourDimensionalArrayAdeptarDeserialize | 3,347.97 ns |  56.004 ns |  68.778 ns | 3,359.19 ns | 0.3395 |      - |   2,152 B |
|      FourDimensionalArrayJsonDeserialize | 5,154.24 ns |  99.649 ns | 114.756 ns | 5,130.57 ns | 0.8469 | 0.0076 |   5,336 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|             NestedListAdeptarDeserialize | 1,416.34 ns |  27.085 ns |  32.243 ns | 1,413.41 ns | 0.1411 |      - |     896 B |
|                NestedListJsonDeserialize | 1,515.34 ns |  29.140 ns |  34.689 ns | 1,519.79 ns | 0.5035 | 0.0019 |   3,160 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                  ArrayAdeptarDeserialize |   998.21 ns |  19.746 ns |  35.605 ns |   994.92 ns | 0.1068 |      - |     672 B |
|                     ArrayJsonDeserialize | 1,279.18 ns |  25.007 ns |  41.088 ns | 1,274.53 ns | 0.4902 | 0.0019 |   3,080 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                 StringAdeptarDeserialize |   103.59 ns |   2.090 ns |   3.191 ns |   103.26 ns | 0.0318 |      - |     200 B |
|                    StringJsonDeserialize |   327.94 ns |   6.479 ns |  11.847 ns |   325.62 ns | 0.4191 | 0.0038 |   2,632 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                   LongAdeptarDeserialize |   166.82 ns |   3.252 ns |   4.112 ns |   167.07 ns | 0.0420 |      - |     264 B |
|                      LongJsonDeserialize |   378.57 ns |   7.133 ns |   7.326 ns |   379.96 ns | 0.4230 | 0.0005 |   2,656 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                   BoolAdeptarDeserialize |    75.12 ns |   1.517 ns |   2.406 ns |    74.91 ns | 0.0267 |      - |     168 B |
|                      BoolJsonDeserialize |   323.30 ns |   8.259 ns |  24.091 ns |   322.63 ns | 0.4148 |      - |   2,608 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                 DoubleAdeptarDeserialize |   392.49 ns |   7.353 ns |   7.551 ns |   391.88 ns | 0.0420 |      - |     264 B |
|                    DoubleJsonDeserialize |   660.07 ns |  12.978 ns |  28.213 ns |   659.94 ns | 0.4301 |      - |   2,704 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                   EnumAdeptarDeserialize |   241.69 ns |   4.733 ns |   7.777 ns |   241.11 ns | 0.0305 |      - |     192 B |
|                      EnumJsonDeserialize |   506.21 ns |   9.465 ns |  13.874 ns |   506.34 ns | 0.4225 |      - |   2,656 B |
|					   |  	 	 | 	      |		   |		 | 	  |	   |	       |
|                   ListAdeptarDeserialize |   629.53 ns |  12.604 ns |  28.450 ns |   623.21 ns | 0.0772 |      - |     488 B |
|                      ListJsonDeserialize |   766.87 ns |  12.087 ns |  10.715 ns |   765.76 ns | 0.4473 | 0.0038 |   2,808 B |
```

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
