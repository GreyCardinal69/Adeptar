# Adeptar
A small .NET file extension aimed to be fast and easy to use, aimed for modding purposes, with manual work in mind.


#Adeptar

A small .NET file extension aimed to be fast and easy to use, aimed for modding purposes, with manual work in mind. 

TO DO:
- Code optimization.

First some benchmarks, both serialization and deserialization.

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 5.0.14 (5.0.1422.5710), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET 5.0.14 (5.0.1422.5710), X64 RyuJIT


```
|                                   Method |        Mean |      Error |     StdDev |      Median |         Min |         Max |  Gen 0 |  Gen 1 | Allocated |
|----------------------------------------- |------------:|-----------:|-----------:|------------:|------------:|------------:|-------:|-------:|----------:|
|                             ClassAdeptar | 1,445.69 ns |  27.978 ns |  33.306 ns | 1,435.54 ns | 1,400.19 ns | 1,522.03 ns | 0.0515 |      - |     328 B |
|                                ClassJson |   853.89 ns |  20.682 ns |  59.008 ns |   838.99 ns |   758.03 ns | 1,027.51 ns | 0.2317 |      - |   1,456 B |
|                             TupleAdeptar | 2,815.08 ns |  55.307 ns |  77.533 ns | 2,801.91 ns | 2,688.42 ns | 2,980.45 ns | 0.1297 |      - |     824 B |
|                                TupleJson | 2,173.18 ns |  42.840 ns |  60.056 ns | 2,157.65 ns | 2,076.25 ns | 2,302.46 ns | 0.3281 |      - |   2,064 B |
|            DictionaryWithArrayKeyAdeptar | 1,370.24 ns |  24.432 ns |  28.136 ns | 1,371.74 ns | 1,324.47 ns | 1,430.90 ns | 0.1678 |      - |   1,064 B |
|               DictionaryWithArrayKeyJson | 1,731.29 ns |  33.099 ns |  32.507 ns | 1,738.16 ns | 1,668.38 ns | 1,790.12 ns | 0.3452 | 0.0019 |   2,176 B |
|                        DictionaryAdeptar |   508.78 ns |  17.133 ns |  49.159 ns |   508.60 ns |   407.69 ns |   629.74 ns | 0.0939 |      - |     592 B |
|                           DictionaryJson |   826.09 ns |  32.536 ns |  92.827 ns |   822.37 ns |   629.19 ns | 1,064.03 ns | 0.2661 | 0.0010 |   1,672 B |
|              FourDimensionalArrayAdeptar | 3,979.26 ns | 115.356 ns | 340.130 ns | 3,897.92 ns | 3,333.23 ns | 4,856.10 ns | 0.1907 |      - |   1,216 B |
|                 FourDimensionalArrayJson | 3,733.36 ns |  73.986 ns | 106.108 ns | 3,743.91 ns | 3,555.04 ns | 3,899.45 ns | 0.4044 |      - |   2,568 B |
|                        NestedListAdeptar |   861.49 ns |  33.706 ns |  98.323 ns |   835.53 ns |   729.78 ns | 1,154.32 ns | 0.1631 |      - |   1,024 B |
|                           NestedListJson | 1,709.79 ns |  42.614 ns | 121.582 ns | 1,705.26 ns | 1,406.96 ns | 1,993.04 ns | 0.3109 |      - |   1,952 B |
|                             ArrayAdeptar | 1,131.46 ns |  30.466 ns |  87.902 ns | 1,117.85 ns | 1,007.72 ns | 1,324.52 ns | 0.0706 |      - |     448 B |
|                                ArrayJson | 1,211.39 ns |  24.046 ns |  56.678 ns | 1,197.30 ns | 1,141.69 ns | 1,371.69 ns | 0.2499 |      - |   1,568 B |
|                            StringAdeptar |    71.80 ns |   1.433 ns |   1.534 ns |    71.92 ns |    69.11 ns |    74.24 ns | 0.0076 |      - |      48 B |
|                               StringJson |   349.70 ns |  12.690 ns |  37.018 ns |   360.29 ns |   260.65 ns |   401.41 ns | 0.2036 | 0.0005 |   1,280 B |
|                              LongAdeptar |    68.52 ns |   1.410 ns |   2.356 ns |    68.35 ns |    65.43 ns |    73.42 ns | 0.0191 |      - |     120 B |
|                                 LongJson |   245.08 ns |   4.764 ns |   6.982 ns |   244.12 ns |   234.71 ns |   258.69 ns | 0.2074 |      - |   1,304 B |
|                              BoolAdeptar |    30.24 ns |   0.496 ns |   0.464 ns |    30.34 ns |    29.45 ns |    31.15 ns | 0.0089 |      - |      56 B |
|                                 BoolJson |   211.39 ns |   4.216 ns |  10.182 ns |   208.43 ns |   194.52 ns |   239.45 ns | 0.1898 |      - |   1,192 B |
|                            DoubleAdeptar |   211.46 ns |   4.019 ns |   3.947 ns |   211.28 ns |   203.29 ns |   220.33 ns | 0.0191 |      - |     120 B |
|                               DoubleJson |   427.99 ns |  10.594 ns |  31.236 ns |   416.30 ns |   385.49 ns |   494.76 ns | 0.1998 | 0.0005 |   1,256 B |
|                              EnumAdeptar |    70.33 ns |   1.417 ns |   1.326 ns |    70.60 ns |    68.13 ns |    72.34 ns | 0.0101 |      - |      64 B |
|                                 EnumJson |   299.50 ns |  26.363 ns |  77.733 ns |   279.44 ns |   200.99 ns |   482.98 ns | 0.1886 | 0.0005 |   1,184 B |
|                              ListAdeptar |   475.95 ns |   9.280 ns |   8.681 ns |   473.39 ns |   464.76 ns |   493.28 ns | 0.0443 |      - |     280 B |
|                                 ListJson |   857.33 ns |  23.638 ns |  67.442 ns |   872.56 ns |   678.18 ns |   997.50 ns | 0.2394 | 0.0010 |   1,504 B |
|                  ClassAdeptarDeserialize | 3,966.70 ns | 128.813 ns | 359.080 ns | 3,962.17 ns | 3,397.64 ns | 4,931.75 ns | 0.3357 |      - |   2,137 B |
|                     ClassJsonDeserialize | 3,840.31 ns |  73.047 ns | 113.725 ns | 3,814.18 ns | 3,665.38 ns | 4,079.56 ns | 0.5608 | 0.0038 |   3,528 B |
|                  TupleAdeptarDeserialize | 7,420.22 ns | 130.806 ns | 134.328 ns | 7,454.94 ns | 7,103.16 ns | 7,578.08 ns | 0.4654 |      - |   2,921 B |
|                     TupleJsonDeserialize | 5,415.14 ns |  78.290 ns | 189.079 ns | 5,384.42 ns | 5,104.82 ns | 6,211.04 ns | 0.6104 |      - |   3,856 B |
| DictionaryWithArrayKeyAdeptarDeserialize | 3,086.39 ns |  61.493 ns | 173.441 ns | 3,119.28 ns | 2,695.84 ns | 3,452.57 ns | 0.2136 |      - |   1,353 B |
|    DictionaryWithArrayKeyJsonDeserialize | 2,746.96 ns |  53.259 ns |  82.918 ns | 2,761.36 ns | 2,566.47 ns | 2,927.15 ns | 0.5493 |      - |   3,464 B |
|             DictionaryAdeptarDeserialize |   975.86 ns |  15.151 ns |  14.880 ns |   973.22 ns |   951.13 ns | 1,005.03 ns | 0.0896 |      - |     568 B |
|                DictionaryJsonDeserialize | 1,333.82 ns |  95.399 ns | 281.286 ns | 1,450.49 ns |   825.12 ns | 1,775.87 ns | 0.4644 |      - |   2,920 B |
|   FourDimensionalArrayAdeptarDeserialize | 6,400.73 ns | 131.267 ns | 378.735 ns | 6,316.89 ns | 5,465.99 ns | 7,342.66 ns | 0.3891 |      - |   2,464 B |
|      FourDimensionalArrayJsonDeserialize | 7,653.46 ns | 152.551 ns | 304.661 ns | 7,671.28 ns | 7,019.70 ns | 8,346.69 ns | 0.8392 |      - |   5,336 B |
|             NestedListAdeptarDeserialize | 2,492.31 ns |  49.555 ns |  74.171 ns | 2,490.76 ns | 2,343.12 ns | 2,623.03 ns | 0.1564 |      - |     984 B |
|                NestedListJsonDeserialize | 2,052.23 ns |  98.244 ns | 289.675 ns | 2,076.00 ns | 1,611.16 ns | 2,542.15 ns | 0.5035 |      - |   3,160 B |
|                  ArrayAdeptarDeserialize | 1,698.41 ns |  33.527 ns |  56.015 ns | 1,684.66 ns | 1,628.54 ns | 1,833.20 ns | 0.1411 |      - |     896 B |
|                     ArrayJsonDeserialize | 1,399.22 ns |  20.968 ns |  19.613 ns | 1,399.44 ns | 1,358.60 ns | 1,423.11 ns | 0.4902 | 0.0019 |   3,080 B |
|                 StringAdeptarDeserialize |   152.10 ns |   2.677 ns |   2.504 ns |   151.30 ns |   148.34 ns |   156.35 ns | 0.0393 |      - |     248 B |
|                    StringJsonDeserialize |   382.55 ns |   6.389 ns |   5.664 ns |   380.38 ns |   375.69 ns |   394.61 ns | 0.4191 | 0.0038 |   2,632 B |
|                   LongAdeptarDeserialize |   207.92 ns |   2.235 ns |   2.091 ns |   207.49 ns |   203.59 ns |   210.86 ns | 0.0432 |      - |     272 B |
|                      LongJsonDeserialize |   442.59 ns |   8.250 ns |   8.103 ns |   442.15 ns |   430.93 ns |   457.94 ns | 0.4230 | 0.0005 |   2,656 B |
|                   BoolAdeptarDeserialize |   129.70 ns |   2.617 ns |   7.467 ns |   129.12 ns |   113.40 ns |   146.63 ns | 0.0305 |      - |     192 B |
|                      BoolJsonDeserialize |   365.60 ns |  15.478 ns |  44.158 ns |   357.57 ns |   304.89 ns |   480.03 ns | 0.4153 | 0.0005 |   2,608 B |
|                 DoubleAdeptarDeserialize |   437.20 ns |   8.344 ns |   7.396 ns |   437.23 ns |   419.56 ns |   448.00 ns | 0.0429 |      - |     272 B |
|                    DoubleJsonDeserialize |   721.38 ns |  17.887 ns |  51.894 ns |   703.71 ns |   657.32 ns |   861.54 ns | 0.4301 |      - |   2,704 B |
|                   EnumAdeptarDeserialize |   233.89 ns |   4.709 ns |   7.995 ns |   233.37 ns |   215.79 ns |   254.85 ns | 0.0331 |      - |     208 B |
|                      EnumJsonDeserialize |   578.31 ns |  11.587 ns |  33.059 ns |   572.95 ns |   522.80 ns |   664.57 ns | 0.4225 |      - |   2,656 B |
|                   ListAdeptarDeserialize |   873.26 ns |  14.854 ns |  13.894 ns |   870.80 ns |   854.66 ns |   903.64 ns | 0.1345 |      - |     848 B |
|                      ListJsonDeserialize |   857.78 ns |  15.744 ns |  21.018 ns |   856.69 ns |   827.02 ns |   898.00 ns | 0.4473 | 0.0038 |   2,808 B |












































