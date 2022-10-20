# Adeptar
A small .NET file extension aimed to be fast and easy to use, aimed for modding purposes, with manual work in mind.

TO DO:
- Code optimization for certain cases.
- More features.
- Proper documentation

First some benchmarks, both serialization and deserialization.
Benchmakrs are compared to NewtonSoft.Json library
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1706 (21H2)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT
```
|                                   Method |        Mean |      Error |     StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|----------------------------------------- |------------:|-----------:|-----------:|------------:|-------:|-------:|----------:|
|                        ClassAdeptarEmpty | 1,155.91 ns |  22.976 ns |  39.633 ns | 1,154.54 ns | 0.0839 |      - |     528 B |
|                           ClassJsonEmpty | 1,021.27 ns |  24.237 ns |  71.463 ns | 1,002.13 ns | 0.2613 |      - |   1,640 B |
|                             ClassAdeptar | 2,177.01 ns |  43.295 ns |  98.604 ns | 2,192.15 ns | 0.2022 |      - |   1,272 B |
|                                ClassJson | 2,690.88 ns |  53.778 ns | 120.282 ns | 2,685.25 ns | 0.3777 |      - |   2,392 B |
|                ClassAdeptarNoIndentation | 2,309.16 ns |  63.364 ns | 182.821 ns | 2,248.01 ns | 0.1984 |      - |   1,264 B |
|                   ClassJsonNoIndentation | 2,548.47 ns |  49.513 ns |  72.575 ns | 2,552.33 ns | 0.3777 |      - |   2,392 B |
|                             TupleAdeptar | 2,448.88 ns |  48.630 ns | 130.642 ns | 2,407.91 ns | 0.1602 |      - |   1,024 B |
|                                TupleJson | 2,470.49 ns |  49.014 ns |  50.334 ns | 2,462.08 ns | 0.3395 | 0.0038 |   2,152 B |
|            DictionaryWithArrayKeyAdeptar | 1,259.66 ns |  22.670 ns |  56.456 ns | 1,259.14 ns | 0.1469 |      - |     928 B |
|               DictionaryWithArrayKeyJson | 1,796.04 ns |  31.312 ns |  27.757 ns | 1,791.50 ns | 0.3452 | 0.0019 |   2,176 B |
|                        DictionaryAdeptar |   401.85 ns |  11.895 ns |  33.743 ns |   401.92 ns | 0.0725 |      - |     456 B |
|                           DictionaryJson |   818.54 ns |  13.356 ns |  12.493 ns |   816.60 ns | 0.2661 | 0.0010 |   1,672 B |
|              FourDimensionalArrayAdeptar | 4,078.52 ns |  79.838 ns | 106.581 ns | 4,051.87 ns | 0.1984 |      - |   1,248 B |
|                 FourDimensionalArrayJson | 4,034.22 ns |  63.691 ns |  59.576 ns | 4,017.64 ns | 0.4044 |      - |   2,568 B |
|                        NestedListAdeptar | 1,134.62 ns |  34.736 ns |  99.665 ns | 1,089.85 ns | 0.1259 |      - |     800 B |
|                           NestedListJson | 1,931.97 ns |  48.191 ns | 138.270 ns | 1,928.31 ns | 0.3109 |      - |   1,952 B |
|                             ArrayAdeptar | 1,242.46 ns |  25.168 ns |  74.207 ns | 1,241.33 ns | 0.0706 |      - |     448 B |
|                                ArrayJson | 1,612.88 ns |  42.075 ns | 124.059 ns | 1,576.24 ns | 0.2499 |      - |   1,568 B |
|                            StringAdeptar |   110.10 ns |   2.950 ns |   8.465 ns |   108.60 ns | 0.0076 |      - |      48 B |
|                               StringJson |   452.19 ns |   8.766 ns |   9.744 ns |   450.07 ns | 0.2036 | 0.0005 |   1,280 B |
|                              LongAdeptar |   105.56 ns |   2.137 ns |   4.413 ns |   104.97 ns | 0.0191 |      - |     120 B |
|                                 LongJson |   466.32 ns |  13.309 ns |  37.755 ns |   455.41 ns | 0.2074 |      - |   1,304 B |
|                              BoolAdeptar |    59.78 ns |   1.187 ns |   1.625 ns |    59.76 ns | 0.0088 |      - |      56 B |
|                                 BoolJson |   377.37 ns |  10.258 ns |  30.245 ns |   370.87 ns | 0.1898 |      - |   1,192 B |
|                            DoubleAdeptar |   308.49 ns |   5.344 ns |   4.737 ns |   309.97 ns | 0.0191 |      - |     120 B |
|                               DoubleJson |   653.70 ns |  12.893 ns |  12.060 ns |   650.33 ns | 0.1993 |      - |   1,256 B |
|                              EnumAdeptar |    93.32 ns |   1.912 ns |   1.878 ns |    93.02 ns | 0.0101 |      - |      64 B |
|                                 EnumJson |   299.13 ns |  15.460 ns |  44.108 ns |   287.92 ns | 0.1884 | 0.0005 |   1,184 B |
|                              ListAdeptar |   442.66 ns |   7.522 ns |   7.388 ns |   440.13 ns | 0.0443 |      - |     280 B |
|                                 ListJson |   741.03 ns |  14.714 ns |  34.682 ns |   729.22 ns | 0.2394 | 0.0010 |   1,504 B |
|                  ClassAdeptarDeserialize | 2,943.36 ns |  54.942 ns |  53.961 ns | 2,926.05 ns | 0.2632 |      - |   1,672 B |
|                     ClassJsonDeserialize | 3,097.26 ns |  32.099 ns |  30.025 ns | 3,108.30 ns | 0.5684 | 0.0038 |   3,576 B |
|                  TupleAdeptarDeserialize | 5,169.22 ns |  78.938 ns |  65.917 ns | 5,173.25 ns | 0.3738 |      - |   2,392 B |
|                     TupleJsonDeserialize | 5,647.45 ns |  54.915 ns |  48.681 ns | 5,659.98 ns | 0.6180 |      - |   3,904 B |
| DictionaryWithArrayKeyAdeptarDeserialize | 1,675.42 ns |  23.036 ns |  21.548 ns | 1,673.94 ns | 0.1602 |      - |   1,016 B |
|    DictionaryWithArrayKeyJsonDeserialize | 2,218.06 ns |  25.502 ns |  22.607 ns | 2,219.06 ns | 0.5493 |      - |   3,464 B |
|             DictionaryAdeptarDeserialize |   653.11 ns |   6.153 ns |   5.455 ns |   651.35 ns | 0.0887 |      - |     560 B |
|                DictionaryJsonDeserialize |   870.95 ns |  11.084 ns |  10.368 ns |   868.71 ns | 0.4644 |      - |   2,920 B |
|   FourDimensionalArrayAdeptarDeserialize | 3,634.03 ns |  33.142 ns |  29.379 ns | 3,641.35 ns | 0.3395 |      - |   2,152 B |
|      FourDimensionalArrayJsonDeserialize | 5,833.85 ns | 114.107 ns | 184.262 ns | 5,796.32 ns | 0.8469 | 0.0076 |   5,336 B |
|             NestedListAdeptarDeserialize | 1,570.92 ns |  30.459 ns |  27.001 ns | 1,571.55 ns | 0.1411 |      - |     896 B |
|                NestedListJsonDeserialize | 1,777.42 ns |  35.106 ns |  91.867 ns | 1,778.49 ns | 0.5035 | 0.0019 |   3,160 B |
|                  ArrayAdeptarDeserialize | 1,076.41 ns |  21.173 ns |  30.366 ns | 1,066.60 ns | 0.1068 |      - |     672 B |
|                     ArrayJsonDeserialize | 1,391.37 ns |  27.588 ns |  76.448 ns | 1,403.05 ns | 0.4902 | 0.0019 |   3,080 B |
|                 StringAdeptarDeserialize |   112.56 ns |   2.162 ns |   3.730 ns |   112.68 ns | 0.0318 |      - |     200 B |
|                    StringJsonDeserialize |   346.88 ns |   6.036 ns |   5.646 ns |   345.08 ns | 0.4191 | 0.0038 |   2,632 B |
|                   LongAdeptarDeserialize |   178.68 ns |   3.602 ns |   6.940 ns |   180.05 ns | 0.0420 |      - |     264 B |
|                      LongJsonDeserialize |   424.41 ns |   8.419 ns |  19.678 ns |   430.76 ns | 0.4230 | 0.0005 |   2,656 B |
|                   BoolAdeptarDeserialize |    79.96 ns |   1.629 ns |   3.435 ns |    80.15 ns | 0.0267 |      - |     168 B |
|                      BoolJsonDeserialize |   306.13 ns |   6.102 ns |  13.898 ns |   306.82 ns | 0.4153 | 0.0005 |   2,608 B |
|                 DoubleAdeptarDeserialize |   398.57 ns |   7.917 ns |  17.379 ns |   400.37 ns | 0.0420 |      - |     264 B |
|                    DoubleJsonDeserialize |   701.15 ns |  13.956 ns |  28.191 ns |   712.74 ns | 0.4301 |      - |   2,704 B |
|                   EnumAdeptarDeserialize |   240.43 ns |   4.814 ns |  11.062 ns |   237.05 ns | 0.0305 |      - |     192 B |
|                      EnumJsonDeserialize |   509.22 ns |  10.111 ns |  19.722 ns |   512.38 ns | 0.4225 |      - |   2,656 B |
|                   ListAdeptarDeserialize |   592.10 ns |  11.828 ns |  24.427 ns |   586.42 ns | 0.0772 |      - |     488 B |
|                      ListJsonDeserialize |   817.88 ns |  16.153 ns |  37.756 ns |   819.23 ns | 0.4473 | 0.0038 |   2,808 B |


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

<h1>ID Feature</h1>
Index feature is used to serialize multiple objects into one.
For example in a file like this, there are 3 different objects serialized into one file. Each object that is appended to the file using the ID feature must
have a unique ID. The ID is used to find the necessary object. A file cant contain two objects with the same ID, if you try to serialize an object with an already existing ID, an exception will be thrown. The ID is taken as a string.

----------------------------------------------------------------
```cs
~odd_ints~
[1,3,5,7,9]
~even_ints~
[2,4,6,8,10]
~random~
[1,45,3,6,8,9,220,9]
```
