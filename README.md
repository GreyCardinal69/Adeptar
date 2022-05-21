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
|                                   Method |        Mean |      Error |     StdDev |      Median |         Min |         Max |  Gen 0 |  Gen 1 | Allocated |
|----------------------------------------- |------------:|-----------:|-----------:|------------:|------------:|------------:|-------:|-------:|----------:|
|                             ClassAdeptar | 1,226.30 ns |  23.899 ns |  21.186 ns | 1,224.48 ns | 1,180.99 ns | 1,261.42 ns | 0.0839 |      - |     528 B |
|                                ClassJson | 1,081.38 ns |  21.060 ns |  21.627 ns | 1,086.94 ns | 1,041.53 ns | 1,106.52 ns | 0.2613 |      - |   1,640 B |
|                             TupleAdeptar | 2,318.99 ns |  45.056 ns |  55.332 ns | 2,312.21 ns | 2,241.22 ns | 2,433.45 ns | 0.1564 |      - |     984 B |
|                                TupleJson | 2,233.23 ns |  29.668 ns |  24.774 ns | 2,233.60 ns | 2,190.03 ns | 2,272.23 ns | 0.3395 |      - |   2,152 B |
|            DictionaryWithArrayKeyAdeptar | 1,287.75 ns |  25.495 ns |  34.898 ns | 1,292.21 ns | 1,214.72 ns | 1,358.85 ns | 0.1678 |      - |   1,064 B |
|               DictionaryWithArrayKeyJson | 1,650.58 ns |  32.104 ns |  45.006 ns | 1,642.40 ns | 1,581.08 ns | 1,747.54 ns | 0.3452 | 0.0019 |   2,176 B |
|                        DictionaryAdeptar |   380.36 ns |   7.637 ns |   7.501 ns |   381.33 ns |   366.98 ns |   391.62 ns | 0.0939 |      - |     592 B |
|                           DictionaryJson |   557.72 ns |  11.107 ns |  21.400 ns |   554.19 ns |   524.61 ns |   606.84 ns | 0.2661 | 0.0010 |   1,672 B |
|              FourDimensionalArrayAdeptar | 2,925.98 ns |  44.410 ns |  41.541 ns | 2,927.90 ns | 2,832.67 ns | 2,989.32 ns | 0.1907 |      - |   1,216 B |
|                 FourDimensionalArrayJson | 2,839.79 ns |  40.889 ns |  36.247 ns | 2,842.42 ns | 2,778.94 ns | 2,895.87 ns | 0.4082 |      - |   2,568 B |
|                        NestedListAdeptar |   706.81 ns |  13.375 ns |  14.311 ns |   709.61 ns |   681.31 ns |   730.93 ns | 0.1631 |      - |   1,024 B |
|                           NestedListJson | 1,183.94 ns |  18.654 ns |  17.449 ns | 1,183.63 ns | 1,149.23 ns | 1,214.34 ns | 0.3109 |      - |   1,952 B |
|                             ArrayAdeptar |   873.70 ns |  17.247 ns |  19.170 ns |   870.16 ns |   848.00 ns |   928.38 ns | 0.0706 |      - |     448 B |
|                                ArrayJson | 1,126.07 ns |  21.829 ns |  30.601 ns | 1,128.26 ns | 1,064.15 ns | 1,191.73 ns | 0.2499 |      - |   1,568 B |
|                            StringAdeptar |    81.46 ns |   1.611 ns |   2.205 ns |    81.04 ns |    78.08 ns |    85.45 ns | 0.0076 |      - |      48 B |
|                               StringJson |   233.51 ns |   3.223 ns |   2.691 ns |   233.88 ns |   229.19 ns |   238.07 ns | 0.2036 | 0.0005 |   1,280 B |
|                              LongAdeptar |    65.50 ns |   1.331 ns |   1.533 ns |    65.36 ns |    63.18 ns |    69.01 ns | 0.0191 |      - |     120 B |
|                                 LongJson |   239.98 ns |   4.683 ns |   7.428 ns |   239.20 ns |   227.58 ns |   259.59 ns | 0.2077 |      - |   1,304 B |
|                              BoolAdeptar |    41.36 ns |   0.689 ns |   0.644 ns |    41.47 ns |    39.90 ns |    42.08 ns | 0.0089 |      - |      56 B |
|                                 BoolJson |   235.97 ns |   7.853 ns |  22.783 ns |   238.22 ns |   196.57 ns |   297.51 ns | 0.1898 |      - |   1,192 B |
|                            DoubleAdeptar |   217.46 ns |   4.168 ns |   8.513 ns |   213.72 ns |   206.61 ns |   239.91 ns | 0.0191 |      - |     120 B |
|                               DoubleJson |   401.41 ns |   7.997 ns |  15.407 ns |   400.00 ns |   377.09 ns |   432.72 ns | 0.1998 | 0.0005 |   1,256 B |
|                              EnumAdeptar |    77.20 ns |   3.151 ns |   9.039 ns |    74.24 ns |    64.69 ns |   102.10 ns | 0.0101 |      - |      64 B |
|                                 EnumJson |   208.37 ns |   3.078 ns |   2.879 ns |   207.89 ns |   203.83 ns |   213.87 ns | 0.1886 | 0.0005 |   1,184 B |
|                              ListAdeptar |   475.18 ns |   3.446 ns |   3.223 ns |   476.19 ns |   469.42 ns |   478.90 ns | 0.0439 |      - |     280 B |
|                                 ListJson |   647.73 ns |  10.907 ns |  10.202 ns |   649.10 ns |   632.93 ns |   662.92 ns | 0.2394 | 0.0010 |   1,504 B |
|                  ClassAdeptarDeserialize | 3,619.72 ns |  58.365 ns |  54.594 ns | 3,613.66 ns | 3,547.66 ns | 3,752.00 ns | 0.3662 |      - |   2,321 B |
|                     ClassJsonDeserialize | 2,929.06 ns |  29.183 ns |  25.870 ns | 2,922.59 ns | 2,900.78 ns | 2,986.42 ns | 0.5684 | 0.0038 |   3,576 B |
|                  TupleAdeptarDeserialize | 6,144.17 ns |  95.189 ns |  89.040 ns | 6,164.57 ns | 6,028.93 ns | 6,324.40 ns | 0.4883 |      - |   3,105 B |
|                     TupleJsonDeserialize | 5,664.92 ns | 112.606 ns | 284.571 ns | 5,601.35 ns | 5,218.77 ns | 6,336.41 ns | 0.6180 |      - |   3,904 B |
| DictionaryWithArrayKeyAdeptarDeserialize | 2,674.21 ns |  53.246 ns | 106.339 ns | 2,675.04 ns | 2,512.22 ns | 2,914.56 ns | 0.2136 |      - |   1,353 B |
|    DictionaryWithArrayKeyJsonDeserialize | 2,365.32 ns |  44.331 ns |  64.980 ns | 2,343.73 ns | 2,277.35 ns | 2,526.83 ns | 0.5493 |      - |   3,464 B |
|             DictionaryAdeptarDeserialize |   867.94 ns |  15.559 ns |  32.819 ns |   861.63 ns |   822.11 ns |   962.19 ns | 0.0896 |      - |     568 B |
|                DictionaryJsonDeserialize |   897.92 ns |  18.156 ns |  52.963 ns |   872.79 ns |   842.82 ns | 1,022.61 ns | 0.4644 |      - |   2,920 B |
|   FourDimensionalArrayAdeptarDeserialize | 4,530.47 ns |  63.577 ns |  56.359 ns | 4,545.21 ns | 4,417.23 ns | 4,619.21 ns | 0.3891 |      - |   2,464 B |
|      FourDimensionalArrayJsonDeserialize | 5,536.87 ns |  91.369 ns | 118.806 ns | 5,514.77 ns | 5,377.96 ns | 5,888.27 ns | 0.8469 | 0.0076 |   5,336 B |
|             NestedListAdeptarDeserialize | 1,915.28 ns |  36.917 ns |  36.257 ns | 1,910.60 ns | 1,863.41 ns | 1,981.21 ns | 0.1564 |      - |     984 B |
|                NestedListJsonDeserialize | 1,640.83 ns |  23.111 ns |  19.299 ns | 1,636.65 ns | 1,617.70 ns | 1,679.33 ns | 0.5035 | 0.0019 |   3,160 B |
|                  ArrayAdeptarDeserialize | 1,697.28 ns |  32.871 ns |  52.137 ns | 1,702.28 ns | 1,621.09 ns | 1,822.29 ns | 0.1411 |      - |     896 B |
|                     ArrayJsonDeserialize | 1,364.21 ns |  24.741 ns |  21.932 ns | 1,357.04 ns | 1,340.49 ns | 1,410.19 ns | 0.4902 | 0.0019 |   3,080 B |
|                 StringAdeptarDeserialize |   154.25 ns |   1.892 ns |   1.580 ns |   154.58 ns |   150.37 ns |   155.74 ns | 0.0393 |      - |     248 B |
|                    StringJsonDeserialize |   397.44 ns |   9.967 ns |  28.598 ns |   388.34 ns |   357.64 ns |   477.30 ns | 0.4191 | 0.0038 |   2,632 B |
|                   LongAdeptarDeserialize |   221.47 ns |   3.290 ns |   3.077 ns |   220.34 ns |   217.00 ns |   228.10 ns | 0.0432 |      - |     272 B |
|                      LongJsonDeserialize |   456.17 ns |  18.132 ns |  53.462 ns |   439.03 ns |   387.81 ns |   598.19 ns | 0.4230 | 0.0005 |   2,656 B |
|                   BoolAdeptarDeserialize |   128.79 ns |   2.614 ns |   7.416 ns |   127.90 ns |   114.32 ns |   148.65 ns | 0.0305 |      - |     192 B |
|                      BoolJsonDeserialize |   359.24 ns |  16.618 ns |  47.948 ns |   341.01 ns |   305.82 ns |   478.13 ns | 0.4153 | 0.0005 |   2,608 B |
|                 DoubleAdeptarDeserialize |   525.99 ns |  12.296 ns |  34.277 ns |   522.97 ns |   445.12 ns |   628.96 ns | 0.0429 |      - |     272 B |
|                    DoubleJsonDeserialize |   851.51 ns |  14.170 ns |  13.255 ns |   852.63 ns |   834.32 ns |   881.26 ns | 0.4301 |      - |   2,704 B |
|                   EnumAdeptarDeserialize |   255.04 ns |   3.062 ns |   2.557 ns |   254.54 ns |   250.83 ns |   258.55 ns | 0.0329 |      - |     208 B |
|                      EnumJsonDeserialize |   661.02 ns |  11.594 ns |  13.802 ns |   659.54 ns |   637.26 ns |   690.11 ns | 0.4225 |      - |   2,656 B |
|                   ListAdeptarDeserialize |   999.70 ns |  15.346 ns |  12.815 ns | 1,004.27 ns |   970.33 ns | 1,015.62 ns | 0.1335 |      - |     848 B |
|                      ListJsonDeserialize | 1,039.65 ns |  20.204 ns |  23.267 ns | 1,031.16 ns |   996.71 ns | 1,086.88 ns | 0.4463 | 0.0038 |   2,808 B |



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
