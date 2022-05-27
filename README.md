# Adeptar
A small .NET file extension aimed to be fast and easy to use, aimed for modding purposes, with manual work in mind.

TO DO:
- Code optimization.
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
|                        ClassAdeptarEmpty | 1,187.07 ns |  23.253 ns |  28.557 ns | 1,185.48 ns | 0.0820 |      - |     520 B |
|                           ClassJsonEmpty | 1,082.13 ns |  20.454 ns |  39.408 ns | 1,075.21 ns | 0.2613 |      - |   1,640 B |
|                             ClassAdeptar | 2,287.27 ns |  43.388 ns |  48.226 ns | 2,280.86 ns | 0.1984 |      - |   1,256 B |
|                                ClassJson | 2,567.31 ns |  51.257 ns |  70.161 ns | 2,562.42 ns | 0.3777 |      - |   2,392 B |
|                             TupleAdeptar | 2,391.85 ns |  42.535 ns |  50.635 ns | 2,397.67 ns | 0.1602 |      - |   1,024 B |
|                                TupleJson | 2,191.24 ns |  25.654 ns |  23.996 ns | 2,186.47 ns | 0.3395 |      - |   2,152 B |
|            DictionaryWithArrayKeyAdeptar | 1,185.54 ns |  22.378 ns |  24.873 ns | 1,182.13 ns | 0.1469 |      - |     928 B |
|               DictionaryWithArrayKeyJson | 1,713.67 ns |  32.666 ns |  27.278 ns | 1,714.70 ns | 0.3452 | 0.0019 |   2,176 B |
|                        DictionaryAdeptar |   327.79 ns |   6.444 ns |   7.421 ns |   326.63 ns | 0.0725 |      - |     456 B |
|                           DictionaryJson |   588.29 ns |  11.505 ns |  11.300 ns |   585.61 ns | 0.2661 | 0.0010 |   1,672 B |
|              FourDimensionalArrayAdeptar | 3,176.62 ns |  63.353 ns | 125.052 ns | 3,182.49 ns | 0.1945 |      - |   1,240 B |
|                 FourDimensionalArrayJson | 3,059.41 ns |  60.426 ns | 122.064 ns | 3,038.93 ns | 0.4082 |      - |   2,568 B |
|                        NestedListAdeptar |   783.37 ns |  12.185 ns |  10.802 ns |   784.06 ns | 0.1268 |      - |     800 B |
|                           NestedListJson | 1,299.93 ns |  25.521 ns |  64.960 ns | 1,278.18 ns | 0.3109 |      - |   1,952 B |
|                             ArrayAdeptar |   837.56 ns |  16.594 ns |  17.756 ns |   833.69 ns | 0.0706 |      - |     448 B |
|                                ArrayJson | 1,123.36 ns |  22.125 ns |  27.171 ns | 1,118.96 ns | 0.2499 |      - |   1,568 B |
|                            StringAdeptar |    74.22 ns |   1.487 ns |   1.826 ns |    74.73 ns | 0.0076 |      - |      48 B |
|                               StringJson |   252.37 ns |   4.987 ns |   8.194 ns |   252.78 ns | 0.2038 | 0.0005 |   1,280 B |
|                              LongAdeptar |    73.67 ns |   1.507 ns |   3.997 ns |    72.51 ns | 0.0191 |      - |     120 B |
|                                 LongJson |   247.07 ns |   4.665 ns |   8.047 ns |   248.11 ns | 0.2074 |      - |   1,304 B |
|                              BoolAdeptar |    37.86 ns |   0.659 ns |   0.617 ns |    37.94 ns | 0.0089 |      - |      56 B |
|                                 BoolJson |   208.71 ns |   3.946 ns |   3.691 ns |   209.05 ns | 0.1898 | 0.0005 |   1,192 B |
|                            DoubleAdeptar |   215.27 ns |   4.244 ns |   3.969 ns |   214.90 ns | 0.0191 |      - |     120 B |
|                               DoubleJson |   407.87 ns |   7.939 ns |  12.123 ns |   406.94 ns | 0.1998 | 0.0005 |   1,256 B |
|                              EnumAdeptar |    63.33 ns |   1.264 ns |   1.772 ns |    63.16 ns | 0.0101 |      - |      64 B |
|                                 EnumJson |   212.51 ns |   2.920 ns |   2.588 ns |   211.87 ns | 0.1884 | 0.0005 |   1,184 B |
|                              ListAdeptar |   427.11 ns |   8.436 ns |  10.360 ns |   423.78 ns | 0.0443 |      - |     280 B |
|                                 ListJson |   682.09 ns |  12.314 ns |  21.241 ns |   676.43 ns | 0.2394 | 0.0010 |   1,504 B |
|                  ClassAdeptarDeserialize | 2,763.92 ns |  50.111 ns |  46.874 ns | 2,760.92 ns | 0.2632 |      - |   1,672 B |
|                     ClassJsonDeserialize | 3,032.86 ns |  60.576 ns | 113.777 ns | 3,010.02 ns | 0.5684 | 0.0038 |   3,576 B |
|                  TupleAdeptarDeserialize | 4,762.98 ns |  95.035 ns | 212.559 ns | 4,804.37 ns | 0.3738 |      - |   2,392 B |
|                     TupleJsonDeserialize | 5,639.85 ns |  86.769 ns |  81.164 ns | 5,665.70 ns | 0.6180 |      - |   3,904 B |
| DictionaryWithArrayKeyAdeptarDeserialize | 1,679.46 ns |  18.651 ns |  16.534 ns | 1,682.09 ns | 0.1602 |      - |   1,016 B |
|    DictionaryWithArrayKeyJsonDeserialize | 2,328.99 ns |  40.322 ns |  52.430 ns | 2,333.67 ns | 0.5493 |      - |   3,464 B |
|             DictionaryAdeptarDeserialize |   677.74 ns |  12.888 ns |  15.342 ns |   676.15 ns | 0.0887 |      - |     560 B |
|                DictionaryJsonDeserialize | 1,031.74 ns |  17.853 ns |  24.437 ns | 1,021.51 ns | 0.4635 |      - |   2,920 B |
|   FourDimensionalArrayAdeptarDeserialize | 3,848.92 ns |  47.733 ns |  44.650 ns | 3,858.01 ns | 0.3357 |      - |   2,152 B |
|      FourDimensionalArrayJsonDeserialize | 6,285.12 ns | 123.576 ns | 216.433 ns | 6,314.55 ns | 0.8469 | 0.0076 |   5,336 B |
|             NestedListAdeptarDeserialize | 1,643.35 ns |  31.951 ns |  28.324 ns | 1,646.82 ns | 0.1411 |      - |     896 B |
|                NestedListJsonDeserialize | 1,870.87 ns |  36.240 ns |  54.243 ns | 1,869.94 ns | 0.5035 | 0.0019 |   3,160 B |
|                  ArrayAdeptarDeserialize | 1,101.31 ns |  16.937 ns |  15.842 ns | 1,098.94 ns | 0.1068 |      - |     672 B |
|                     ArrayJsonDeserialize | 1,489.45 ns |  22.724 ns |  20.145 ns | 1,488.03 ns | 0.4902 | 0.0019 |   3,080 B |
|                 StringAdeptarDeserialize |   114.52 ns |   1.223 ns |   1.144 ns |   114.58 ns | 0.0318 |      - |     200 B |
|                    StringJsonDeserialize |   405.02 ns |   8.020 ns |  10.429 ns |   402.86 ns | 0.4191 | 0.0038 |   2,632 B |
|                   LongAdeptarDeserialize |   194.70 ns |   3.245 ns |   2.877 ns |   194.42 ns | 0.0420 |      - |     264 B |
|                      LongJsonDeserialize |   459.83 ns |   9.175 ns |  13.158 ns |   460.69 ns | 0.4230 | 0.0005 |   2,656 B |
|                   BoolAdeptarDeserialize |    77.98 ns |   0.803 ns |   0.711 ns |    78.04 ns | 0.0267 |      - |     168 B |
|                      BoolJsonDeserialize |   318.75 ns |   3.639 ns |   3.404 ns |   318.48 ns | 0.4153 | 0.0005 |   2,608 B |
|                 DoubleAdeptarDeserialize |   411.90 ns |   2.974 ns |   2.782 ns |   410.90 ns | 0.0420 |      - |     264 B |
|                    DoubleJsonDeserialize |   695.66 ns |   7.739 ns |   7.239 ns |   694.89 ns | 0.4301 |      - |   2,704 B |
|                   EnumAdeptarDeserialize |   265.52 ns |   5.221 ns |   9.145 ns |   266.74 ns | 0.0305 |      - |     192 B |
|                      EnumJsonDeserialize |   590.94 ns |  27.603 ns |  80.518 ns |   578.07 ns | 0.4225 |      - |   2,656 B |
|                   ListAdeptarDeserialize |   675.58 ns |  14.804 ns |  43.416 ns |   671.24 ns | 0.0772 |      - |     488 B |
|                      ListJsonDeserialize |   860.67 ns |   8.395 ns |   7.852 ns |   860.22 ns | 0.4473 | 0.0038 |   2,808 B |


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
