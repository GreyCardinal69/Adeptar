# Adeptar
A small .NET file extension aimed to be fast and easy to use, aimed for modding purposes, with manual work in mind.

TO DO:
- Code optimization.
- More features.

First some benchmarks, both serialization and deserialization.
Benchmakrs are compared to NewtonSoft.Json library
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1706 (21H2)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT


```
|                                   Method |        Mean |      Error |     StdDev |  Gen 0 |  Gen 1 | Allocated |
|----------------------------------------- |------------:|-----------:|-----------:|-------:|-------:|----------:|
|                        ClassAdeptarEmpty | 1,075.31 ns |   6.764 ns |   6.327 ns | 0.0820 |      - |     520 B |
|                           ClassJsonEmpty |   960.31 ns |   4.345 ns |   4.064 ns | 0.2613 |      - |   1,640 B |
|                             ClassAdeptar | 2,100.83 ns |   9.211 ns |   8.616 ns | 0.1984 |      - |   1,256 B |
|                                ClassJson | 2,412.54 ns |  47.558 ns |  78.138 ns | 0.3777 |      - |   2,392 B |
|                             TupleAdeptar | 2,114.77 ns |  15.750 ns |  13.962 ns | 0.1602 |      - |   1,024 B |
|                                TupleJson | 2,040.00 ns |  27.509 ns |  24.386 ns | 0.3395 |      - |   2,152 B |
|            DictionaryWithArrayKeyAdeptar | 1,044.10 ns |   9.193 ns |   8.599 ns | 0.1469 |      - |     928 B |
|               DictionaryWithArrayKeyJson | 1,508.54 ns |  11.848 ns |  11.082 ns | 0.3452 |      - |   2,176 B |
|                        DictionaryAdeptar |   288.92 ns |   2.446 ns |   2.168 ns | 0.0725 |      - |     456 B |
|                           DictionaryJson |   525.39 ns |  10.375 ns |  15.843 ns | 0.2661 | 0.0010 |   1,672 B |
|              FourDimensionalArrayAdeptar | 2,815.23 ns |  53.767 ns |  64.006 ns | 0.1945 |      - |   1,240 B |
|                 FourDimensionalArrayJson | 2,626.05 ns |  11.113 ns |   9.280 ns | 0.4082 |      - |   2,568 B |
|                        NestedListAdeptar |   715.21 ns |   3.073 ns |   2.874 ns | 0.1268 |      - |     800 B |
|                           NestedListJson | 1,117.63 ns |   8.765 ns |   8.198 ns | 0.3109 |      - |   1,952 B |
|                             ArrayAdeptar |   744.22 ns |   7.549 ns |   5.894 ns | 0.0706 |      - |     448 B |
|                                ArrayJson |   999.44 ns |  19.789 ns |  18.510 ns | 0.2499 |      - |   1,568 B |
|                            StringAdeptar |    65.47 ns |   0.405 ns |   0.338 ns | 0.0076 |      - |      48 B |
|                               StringJson |   218.26 ns |   4.081 ns |   3.618 ns | 0.2038 | 0.0005 |   1,280 B |
|                              LongAdeptar |    62.27 ns |   0.679 ns |   0.602 ns | 0.0191 |      - |     120 B |
|                                 LongJson |   221.50 ns |   4.318 ns |   6.464 ns | 0.2077 | 0.0005 |   1,304 B |
|                              BoolAdeptar |    33.16 ns |   0.547 ns |   0.427 ns | 0.0089 |      - |      56 B |
|                                 BoolJson |   190.77 ns |   2.444 ns |   2.167 ns | 0.1898 |      - |   1,192 B |
|                            DoubleAdeptar |   196.83 ns |   1.949 ns |   1.628 ns | 0.0191 |      - |     120 B |
|                               DoubleJson |   351.17 ns |   1.805 ns |   1.600 ns | 0.1998 | 0.0005 |   1,256 B |
|                              EnumAdeptar |    55.04 ns |   0.317 ns |   0.281 ns | 0.0102 |      - |      64 B |
|                                 EnumJson |   193.82 ns |   3.790 ns |   5.900 ns | 0.1886 | 0.0005 |   1,184 B |
|                              ListAdeptar |   371.11 ns |   2.378 ns |   2.225 ns | 0.0443 |      - |     280 B |
|                                 ListJson |   604.48 ns |  12.087 ns |  19.859 ns | 0.2394 | 0.0010 |   1,504 B |
|                  ClassAdeptarDeserialize | 3,319.33 ns |  19.065 ns |  17.834 ns | 0.3662 |      - |   2,321 B |
|                     ClassJsonDeserialize | 2,734.14 ns |  44.033 ns |  39.034 ns | 0.5684 | 0.0038 |   3,576 B |
|                  TupleAdeptarDeserialize | 5,672.89 ns | 113.188 ns | 212.594 ns | 0.4883 |      - |   3,105 B |
|                     TupleJsonDeserialize | 4,787.21 ns |  93.348 ns | 155.964 ns | 0.6180 |      - |   3,904 B |
| DictionaryWithArrayKeyAdeptarDeserialize | 2,251.82 ns |   9.249 ns |   8.199 ns | 0.2136 |      - |   1,353 B |
|    DictionaryWithArrayKeyJsonDeserialize | 1,867.18 ns |  16.446 ns |  15.384 ns | 0.5512 | 0.0019 |   3,464 B |
|             DictionaryAdeptarDeserialize |   754.18 ns |  10.479 ns |   9.802 ns | 0.0896 |      - |     568 B |
|                DictionaryJsonDeserialize |   796.45 ns |  15.867 ns |  23.258 ns | 0.4644 |      - |   2,920 B |
|   FourDimensionalArrayAdeptarDeserialize | 3,931.19 ns |  56.733 ns |  53.068 ns | 0.3891 |      - |   2,464 B |
|      FourDimensionalArrayJsonDeserialize | 5,197.52 ns | 100.989 ns | 108.057 ns | 0.8469 | 0.0076 |   5,336 B |
|             NestedListAdeptarDeserialize | 1,655.74 ns |  10.162 ns |   9.506 ns | 0.1564 |      - |     984 B |
|                NestedListJsonDeserialize | 1,535.03 ns |  28.314 ns |  27.808 ns | 0.5035 | 0.0019 |   3,160 B |
|                  ArrayAdeptarDeserialize | 1,478.07 ns |  25.153 ns |  23.528 ns | 0.1411 |      - |     896 B |
|                     ArrayJsonDeserialize | 1,218.43 ns |  19.026 ns |  17.797 ns | 0.4902 | 0.0019 |   3,080 B |
|                 StringAdeptarDeserialize |   137.34 ns |   2.618 ns |   2.321 ns | 0.0393 |      - |     248 B |
|                    StringJsonDeserialize |   317.93 ns |   6.249 ns |   8.342 ns | 0.4191 | 0.0038 |   2,632 B |
|                   LongAdeptarDeserialize |   189.25 ns |   1.511 ns |   1.413 ns | 0.0432 |      - |     272 B |
|                      LongJsonDeserialize |   370.47 ns |   6.716 ns |   6.282 ns | 0.4230 | 0.0005 |   2,656 B |
|                   BoolAdeptarDeserialize |   106.47 ns |   2.102 ns |   2.421 ns | 0.0305 |      - |     192 B |
|                      BoolJsonDeserialize |   271.61 ns |   5.100 ns |   4.521 ns | 0.4153 | 0.0005 |   2,608 B |
|                 DoubleAdeptarDeserialize |   402.52 ns |   7.606 ns |   7.114 ns | 0.0429 |      - |     272 B |
|                    DoubleJsonDeserialize |   627.26 ns |  12.227 ns |  20.763 ns | 0.4301 |      - |   2,704 B |
|                   EnumAdeptarDeserialize |   200.75 ns |   3.046 ns |   2.849 ns | 0.0331 |      - |     208 B |
|                      EnumJsonDeserialize |   459.57 ns |   9.126 ns |  16.455 ns | 0.4230 | 0.0005 |   2,656 B |
|                   ListAdeptarDeserialize |   792.82 ns |  15.628 ns |  22.908 ns | 0.1345 |      - |     848 B |
|                      ListJsonDeserialize |   743.67 ns |  14.099 ns |  13.189 ns | 0.4473 | 0.0038 |   2,808 B |
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
