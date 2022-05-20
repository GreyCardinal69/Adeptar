# Adeptar
A small .NET file extension aimed to be fast and easy to use, aimed for modding purposes, with manual work in mind.

TO DO:
- Code optimization.
- More features.

First some benchmarks, both serialization and deserialization.

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1706 (21H2)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT


```
|                                   Method |        Mean |      Error |     StdDev |         Min |         Max |  Gen 0 |  Gen 1 | Allocated |
|----------------------------------------- |------------:|-----------:|-----------:|------------:|------------:|-------:|-------:|----------:|
|                             ClassAdeptar | 1,954.71 ns |  38.414 ns |  55.092 ns | 1,886.44 ns | 2,081.11 ns | 0.0572 |      - |     376 B |
|                                ClassJson |   946.80 ns |  18.374 ns |  28.060 ns |   912.54 ns | 1,017.75 ns | 0.2441 |      - |   1,536 B |
|                             TupleAdeptar | 3,107.37 ns |  57.068 ns |  53.382 ns | 3,054.00 ns | 3,204.65 ns | 0.1373 |      - |     872 B |
|                                TupleJson | 2,091.52 ns |  36.717 ns |  34.345 ns | 2,046.15 ns | 2,139.03 ns | 0.3319 |      - |   2,096 B |
|            DictionaryWithArrayKeyAdeptar | 1,279.96 ns |  19.842 ns |  18.561 ns | 1,242.89 ns | 1,304.85 ns | 0.1678 |      - |   1,064 B |
|               DictionaryWithArrayKeyJson | 1,753.01 ns |  32.002 ns |  66.799 ns | 1,611.63 ns | 1,940.54 ns | 0.3452 | 0.0019 |   2,176 B |
|                        DictionaryAdeptar |   400.15 ns |   7.777 ns |  11.399 ns |   380.11 ns |   422.97 ns | 0.0939 |      - |     592 B |
|                           DictionaryJson |   575.82 ns |  10.812 ns |   9.585 ns |   556.79 ns |   587.78 ns | 0.2661 | 0.0010 |   1,672 B |
|              FourDimensionalArrayAdeptar | 2,916.65 ns |  47.249 ns |  73.562 ns | 2,821.06 ns | 3,128.59 ns | 0.1907 |      - |   1,216 B |
|                 FourDimensionalArrayJson | 2,842.40 ns |  23.052 ns |  21.563 ns | 2,811.41 ns | 2,872.82 ns | 0.4082 |      - |   2,568 B |
|                        NestedListAdeptar |   687.97 ns |   6.051 ns |   5.364 ns |   678.53 ns |   695.23 ns | 0.1631 |      - |   1,024 B |
|                           NestedListJson | 1,230.64 ns |  24.502 ns |  35.140 ns | 1,163.22 ns | 1,303.71 ns | 0.3109 |      - |   1,952 B |
|                             ArrayAdeptar |   860.45 ns |  13.711 ns |  12.154 ns |   846.76 ns |   889.73 ns | 0.0706 |      - |     448 B |
|                                ArrayJson | 1,121.54 ns |  10.010 ns |   9.363 ns | 1,101.24 ns | 1,132.47 ns | 0.2499 |      - |   1,568 B |
|                            StringAdeptar |    79.78 ns |   0.895 ns |   0.837 ns |    77.99 ns |    80.66 ns | 0.0076 |      - |      48 B |
|                               StringJson |   246.41 ns |   2.948 ns |   2.757 ns |   242.18 ns |   249.48 ns | 0.2036 | 0.0005 |   1,280 B |
|                              LongAdeptar |    64.31 ns |   0.813 ns |   0.720 ns |    62.70 ns |    65.20 ns | 0.0191 |      - |     120 B |
|                                 LongJson |   244.69 ns |   4.227 ns |  10.127 ns |   235.03 ns |   276.30 ns | 0.2074 |      - |   1,304 B |
|                              BoolAdeptar |    38.02 ns |   0.655 ns |   0.701 ns |    37.22 ns |    39.86 ns | 0.0089 |      - |      56 B |
|                                 BoolJson |   213.52 ns |   4.014 ns |   4.122 ns |   203.83 ns |   220.91 ns | 0.1898 |      - |   1,192 B |
|                            DoubleAdeptar |   208.71 ns |   4.161 ns |   3.689 ns |   201.42 ns |   214.94 ns | 0.0191 |      - |     120 B |
|                               DoubleJson |   406.65 ns |   7.946 ns |  10.049 ns |   385.62 ns |   425.16 ns | 0.1998 | 0.0005 |   1,256 B |
|                              EnumAdeptar |    66.32 ns |   1.198 ns |   1.062 ns |    63.21 ns |    67.81 ns | 0.0101 |      - |      64 B |
|                                 EnumJson |   207.26 ns |   2.035 ns |   1.589 ns |   204.84 ns |   209.35 ns | 0.1886 | 0.0005 |   1,184 B |
|                              ListAdeptar |   447.99 ns |   4.240 ns |   3.966 ns |   443.25 ns |   456.20 ns | 0.0443 |      - |     280 B |
|                                 ListJson |   641.05 ns |   5.875 ns |   5.495 ns |   631.92 ns |   651.57 ns | 0.2394 | 0.0010 |   1,504 B |
|                  ClassAdeptarDeserialize | 3,506.27 ns |  33.660 ns |  31.485 ns | 3,437.87 ns | 3,555.52 ns | 0.3662 |      - |   2,313 B |
|                     ClassJsonDeserialize | 2,943.75 ns |  24.245 ns |  22.679 ns | 2,911.25 ns | 2,974.85 ns | 0.5684 | 0.0038 |   3,568 B |
|                  TupleAdeptarDeserialize | 5,966.79 ns |  36.635 ns |  34.269 ns | 5,916.12 ns | 6,023.82 ns | 0.4883 |      - |   3,097 B |
|                     TupleJsonDeserialize | 5,279.97 ns |  44.607 ns |  41.725 ns | 5,208.97 ns | 5,350.81 ns | 0.6180 |      - |   3,896 B |
| DictionaryWithArrayKeyAdeptarDeserialize | 2,469.37 ns |  22.194 ns |  20.760 ns | 2,431.17 ns | 2,496.10 ns | 0.2136 |      - |   1,353 B |
|    DictionaryWithArrayKeyJsonDeserialize | 2,060.23 ns |  13.766 ns |  12.877 ns | 2,024.89 ns | 2,075.88 ns | 0.5493 |      - |   3,464 B |
|             DictionaryAdeptarDeserialize |   810.67 ns |   4.915 ns |   4.598 ns |   801.40 ns |   816.51 ns | 0.0896 |      - |     568 B |
|                DictionaryJsonDeserialize |   861.82 ns |  13.383 ns |  11.863 ns |   846.18 ns |   887.06 ns | 0.4644 | 0.0029 |   2,920 B |
|   FourDimensionalArrayAdeptarDeserialize | 4,380.24 ns |  59.278 ns |  55.449 ns | 4,292.44 ns | 4,442.43 ns | 0.3891 |      - |   2,464 B |
|      FourDimensionalArrayJsonDeserialize | 5,527.61 ns | 101.192 ns | 163.406 ns | 5,351.76 ns | 5,942.05 ns | 0.8469 | 0.0076 |   5,336 B |
|             NestedListAdeptarDeserialize | 1,779.40 ns |  18.710 ns |  17.501 ns | 1,755.09 ns | 1,808.35 ns | 0.1564 |      - |     984 B |
|                NestedListJsonDeserialize | 1,572.79 ns |  15.357 ns |  14.365 ns | 1,547.08 ns | 1,591.44 ns | 0.5035 | 0.0019 |   3,160 B |
|                  ArrayAdeptarDeserialize | 1,569.10 ns |  15.886 ns |  14.860 ns | 1,550.81 ns | 1,592.23 ns | 0.1411 |      - |     896 B |
|                     ArrayJsonDeserialize | 1,311.23 ns |  14.976 ns |  14.008 ns | 1,290.31 ns | 1,338.95 ns | 0.4902 | 0.0019 |   3,080 B |
|                 StringAdeptarDeserialize |   145.87 ns |   1.706 ns |   1.596 ns |   143.30 ns |   147.90 ns | 0.0393 |      - |     248 B |
|                    StringJsonDeserialize |   333.32 ns |   3.546 ns |   2.961 ns |   329.83 ns |   340.17 ns | 0.4191 | 0.0038 |   2,632 B |
|                   LongAdeptarDeserialize |   201.75 ns |   1.512 ns |   1.414 ns |   199.30 ns |   203.47 ns | 0.0432 |      - |     272 B |
|                      LongJsonDeserialize |   389.86 ns |   3.145 ns |   2.942 ns |   385.66 ns |   395.36 ns | 0.4230 | 0.0005 |   2,656 B |
|                   BoolAdeptarDeserialize |   117.01 ns |   1.426 ns |   1.334 ns |   114.92 ns |   119.46 ns | 0.0305 |      - |     192 B |
|                      BoolJsonDeserialize |   285.03 ns |   3.551 ns |   3.322 ns |   279.54 ns |   288.97 ns | 0.4153 | 0.0005 |   2,608 B |
|                 DoubleAdeptarDeserialize |   421.69 ns |   5.305 ns |   4.430 ns |   410.95 ns |   427.30 ns | 0.0429 |      - |     272 B |
|                    DoubleJsonDeserialize |   670.32 ns |  15.838 ns |  45.696 ns |   607.90 ns |   800.67 ns | 0.4301 |      - |   2,704 B |
|                   EnumAdeptarDeserialize |   222.31 ns |   3.041 ns |   2.539 ns |   218.54 ns |   227.35 ns | 0.0331 |      - |     208 B |
|                      EnumJsonDeserialize |   503.60 ns |   9.970 ns |  11.082 ns |   492.80 ns |   533.53 ns | 0.4225 |      - |   2,656 B |
|                   ListAdeptarDeserialize |   841.73 ns |  13.865 ns |  12.970 ns |   799.48 ns |   855.24 ns | 0.1345 |      - |     848 B |
|                      ListJsonDeserialize |   809.97 ns |  15.701 ns |  23.015 ns |   773.00 ns |   857.08 ns | 0.4473 | 0.0038 |   2,808 B |


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
