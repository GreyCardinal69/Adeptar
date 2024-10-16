using System;
using System.Collections.Generic;
using System.Collections;
using BenchmarkDotNet.Attributes;
using Adeptar;
using Newtonsoft.Json;
using System.Reflection;
using static AdeptarBenchmarks.MemoryBenchmarkerDemo;

namespace AdeptarBenchmarks
{
    [MemoryDiagnoser]
    public class Program
    {
        static void Main( string[] args )
        {
#if DEBUG
            string serializePath = AppDomain.CurrentDomain.BaseDirectory + @"seri.ader";
            string deserializePath = AppDomain.CurrentDomain.BaseDirectory + @"deser.ader";

            var type = typeof( MemoryBenchmarkerDemo.MyClassWithNested );
            var obj = new MemoryBenchmarkerDemo.MyClassWithNested()
            {
                date = DateTime.Now,
                Maps = new Dictionary<int, string>() { { 1, "hello" }, { 2, "world" } },
                Number = 1,
                Number3 = 4,
                Odds = new int[] { 1, 3, 5, 7, 9 },
                type = SerializableType.Dictionary,
                NestedObject = new()
                {
                    Number = 1,
                    Type = SerializableType.Dictionary,
                }
            };

            AdeptarBaker.BakeClassStruct( type, obj );

            Console.WriteLine(  AdeptarConverter.SerializeUsingBaked( obj, type ) );
#else
            AdeptarBaker.BakeClassStruct( typeof( MemoryBenchmarkerDemo.MyClass ), new MemoryBenchmarkerDemo.MyClass() );
            AdeptarBaker.BakeClassStruct( typeof( MyClassWithNested ), new MyClassWithNested() );

            BenchmarkDotNet.Running.BenchmarkRunner.Run<MemoryBenchmarkerDemo>();
            Console.ReadLine();
#endif
        }
    }

    [MemoryDiagnoser]
    public class MemoryBenchmarkerDemo
    {
        public static Type Type = typeof( MemoryBenchmarkerDemo.MyClass );
        public static Type Type2 = typeof( MemoryBenchmarkerDemo.MyClassWithNested );
        public static int A1 = AdeptarBaker.IntBakeClassStruct( Type, new MyClass() );
        public static int A2 = AdeptarBaker.IntBakeClassStruct( Type2, new MyClassWithNested() );

        public class NestedClass
        {
            public int Number;
            public SerializableType Type;
        }

        public class MyClassWithNested
        {
            public SerializableType type = SerializableType.Ignore;
            public int Number;
            public int Number3;
            public int[] Odds;
            public Dictionary<int, string> Maps;
            public DateTime date;
            public NestedClass NestedObject;
        }

        public class MyClass
        {
            public SerializableType type = SerializableType.Ignore;
            public int Number;
            public int Number3;
            public int[] Odds;
            public Dictionary<int, string> Maps;
            public DateTime date;
        }

        public class MyClassWithConfig
        {
            [AdeptarIgnore]
            public AdeptarConfiguration config = new()
            {
                ToIgnore = new string[] { "Odds", "Maps" }
            };
            public SerializableType type = SerializableType.Ignore;
            public int Number;
            public int Number3;
            public int[] Odds;
            public Dictionary<int, string> Maps;
            public DateTime date;
        }

        [Benchmark]
        public void ClassAdeptarEmpty()
        {
            AdeptarConverter.Serialize( new MyClass(), Type );
        }

        [Benchmark]
        public void ClassJsonEmpty()
        {
            JsonConvert.SerializeObject( new MyClass() );
        }

        [Benchmark]
        public void ClassAdeptar()
        {
            AdeptarConverter.Serialize( new MyClass()
            {
                date = DateTime.Now,
                Maps = new Dictionary<int, string>() { { 1, "hello" }, { 2, "world" } },
                Number = 1,
                Number3 = 4,
                Odds = new int[] { 1, 3, 5, 7, 9 },
                type = SerializableType.Dictionary
            }, Type );
        }

        [Benchmark]
        public void ClassJson()
        {
            for ( int i = 0; i < 10000; i++ )
            {
                JsonConvert.SerializeObject( new MyClass()
                {
                    date = DateTime.Now,
                    Maps = new Dictionary<int, string>() { { 1, "hello" }, { 2, "world" } },
                    Number = 1,
                    Number3 = 4,
                    Odds = new int[] { 1, 3, 5, 7, 9 },
                    type = SerializableType.Dictionary
                } );
            }
        }

        [Benchmark]
        public void ClassAdeptarNested()
        {
            AdeptarConverter.Serialize( new MemoryBenchmarkerDemo.MyClassWithNested()
            {
                date = DateTime.Now,
                Maps = new Dictionary<int, string>() { { 1, "hello" }, { 2, "world" } },
                Number = 1,
                Number3 = 4,
                Odds = new int[] { 1, 3, 5, 7, 9 },
                type = SerializableType.Dictionary,
                NestedObject = new()
                {
                    Number = 1,
                    Type = SerializableType.Dictionary,
                }
            }, Type2 );
        }

        [Benchmark]
        public void ClassJsonNested()
        {
            JsonConvert.SerializeObject( new MemoryBenchmarkerDemo.MyClassWithNested()
            {
                date = DateTime.Now,
                Maps = new Dictionary<int, string>() { { 1, "hello" }, { 2, "world" } },
                Number = 1,
                Number3 = 4,
                Odds = new int[] { 1, 3, 5, 7, 9 },
                type = SerializableType.Dictionary,
                NestedObject = new()
                {
                    Number = 1,
                    Type = SerializableType.Dictionary,
                }
            } );
        }

        [Benchmark]
        public void ClassAdeptarNoIndentation()
        {
            AdeptarConverter.Serialize( new MyClass()
            {
                date = DateTime.Now,
                Maps = new Dictionary<int, string>() { { 1, "hello" }, { 2, "world" } },
                Number = 1,
                Number3 = 4,
                Odds = new int[] { 1, 3, 5, 7, 9 },
                type = SerializableType.Dictionary
            }, Type );
        }

        [Benchmark]
        public void ClassJsonNoIndentation()
        {
            JsonConvert.SerializeObject( new MyClass()
            {
                date = DateTime.Now,
                Maps = new Dictionary<int, string>() { { 1, "hello" }, { 2, "world" } },
                Number = 1,
                Number3 = 4,
                Odds = new int[] { 1, 3, 5, 7, 9 },
                type = SerializableType.Dictionary
            }, Newtonsoft.Json.Formatting.None );
        }

        public static Type Type3 = typeof( (int, string, MyClass, int[]) );

        [Benchmark]
        public void TupleAdeptar()
        {
            AdeptarConverter.Serialize( (1, "Hello World", new MyClass(), new int[] { 1, 2, 3, 4 }), Type3 );
        }

        [Benchmark]
        public void TupleJson()
        {
            JsonConvert.SerializeObject( (1, "Hello World", new MyClass(), new int[] { 1, 2, 3, 4 }) );
        }

        public static Type Type4 = typeof( Dictionary<int, int[]> );

        [Benchmark]
        public void DictionaryWithArrayKeyAdeptar()
        {
            AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } }, Type4 );
        }

        [Benchmark]
        public void DictionaryWithArrayKeyJson()
        {
            JsonConvert.SerializeObject( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } } );
        }

        public static Type Type5 = typeof( Dictionary<int, int> );

        [Benchmark]
        public void DictionaryAdeptar()
        {
            AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } }, Type5 );
        }

        [Benchmark]
        public void DictionaryJson()
        {
            JsonConvert.SerializeObject( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } } );
        }

        public static Type Type6 = typeof( int[,,,] );

        [Benchmark]
        public void FourDimensionalArrayAdeptar()
        {
            AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, Type6 );
        }

        [Benchmark]
        public void FourDimensionalArrayJson()
        {
            JsonConvert.SerializeObject( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
        }

        public static Type Type7 = typeof( List<List<int>> );

        [Benchmark]
        public void NestedListAdeptar()
        {
            AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } }, Type7 );
        }

        [Benchmark]
        public void NestedListJson()
        {
            JsonConvert.SerializeObject( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
        }

        public static Type Type8 = typeof( int[] );

        [Benchmark]
        public void ArrayAdeptar()
        {
            AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, }, Type8 );
        }

        [Benchmark]
        public void ArrayJson()
        {
            JsonConvert.SerializeObject( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
        }

        public static Type Type9 = typeof( string );

        [Benchmark]
        public void StringAdeptar()
        {
            AdeptarConverter.Serialize( @"hello world", Type9 );
        }

        [Benchmark]
        public void StringJson()
        {
            JsonConvert.SerializeObject( @"hello world" );
        }

        public static Type Type10 = typeof( long );

        [Benchmark]
        public void LongAdeptar()
        {
            AdeptarConverter.Serialize( 12414124124, Type10 );
        }

        [Benchmark]
        public void LongJson()
        {
            JsonConvert.SerializeObject( 12414124124 );
        }

        public static Type Type11 = typeof( bool );

        [Benchmark]
        public void BoolAdeptar()
        {
            AdeptarConverter.Serialize( true, Type11 );
        }

        [Benchmark]
        public void BoolJson()
        {
            JsonConvert.SerializeObject( true );
        }

        public static Type Type12 = typeof( double );

        [Benchmark]
        public void DoubleAdeptar()
        {
            AdeptarConverter.Serialize( 5521355.124, Type12 );
        }

        [Benchmark]
        public void DoubleJson()
        {
            JsonConvert.SerializeObject( 5521355.124 );
        }

        public static Type Type13 = typeof( Enum );

        [Benchmark]
        public void EnumAdeptar()
        {
            AdeptarConverter.Serialize( System.Xml.Formatting.Indented, Type13 );
        }

        [Benchmark]
        public void EnumJson()
        {
            JsonConvert.SerializeObject( System.Xml.Formatting.Indented );
        }

        public static Type Type14 = typeof( List<string> );

        [Benchmark]
        public void ListAdeptar()
        {
            AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" }, Type14 );
        }

        [Benchmark]
        public void ListJson()
        {
            JsonConvert.SerializeObject( new List<string>() { "Some", "Random", "Words", "Words" } );
        }

        [Benchmark]
        public void ClassAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<MyClass>( @"{Maps: [1:""Hello"",2:""World""],Number: 5,Number2: 5,Number3: 5,Odds: [1,3,5,7,9]}" );
        }

        [Benchmark]
        public void ClassJsonDeserialize()
        {
            JsonConvert.DeserializeObject<MyClass>( @"{""Number"":5,""Number2"":5,""Number3"":5,""Odds"":[1,3,5,7,9],""Maps"":{""1"":""Hello"",""2"":""World""}}" );
        }

        [Benchmark]
        public void TupleAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<(int, string, MyClass, int[])>( @"(Item1: 1,Item2: ""Hello World"",Item3: {Maps: [1:""Hello"",2:""World""],Number: 5,Number2: 5,Number3: 5,Odds: [1,3,5,7,9]},Item4: [1,2,3,4])" );
        }

        [Benchmark]
        public void TupleJsonDeserialize()
        {
            JsonConvert.DeserializeObject<(int, string, MyClass, int[])>( @"{""Item1"":1,""Item2"":""Hello World"",""Item3"":{""Number"":5,""Number2"":5,""Number3"":5,""Odds"":[1,3,5,7,9],""Maps"":{""1"":""Hello"",""2"":""World""}},""Item4"":[1,2,3,4]}" );
        }

        [Benchmark]
        public void DictionaryWithArrayKeyAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<Dictionary<int, int[]>>( @"[1:[1,2,3,4],2:[3,4,5,6]]" );
        }

        [Benchmark]
        public void DictionaryWithArrayKeyJsonDeserialize()
        {
            JsonConvert.DeserializeObject<Dictionary<int, int[]>>( @"{""1"":[1,2,3,4],""2"":[3,4,5,6]}" );
        }

        [Benchmark]
        public void DictionaryAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<Dictionary<int, int>>( @"[1:2,3:4]" );
        }

        [Benchmark]
        public void DictionaryJsonDeserialize()
        {
            JsonConvert.DeserializeObject<Dictionary<int, int>>( @"{""1"":2,""3"":4}" );
        }

        [Benchmark]
        public void FourDimensionalArrayAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<int[,,,]>( @"[<2,2,2,2>1,2,3,4,5,6,7,8,1,2,3,4,5,6,7,8]" );
        }

        [Benchmark]
        public void FourDimensionalArrayJsonDeserialize()
        {
            JsonConvert.DeserializeObject<int[,,,]>( @"[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[1,2],[3,4]],[[5,6],[7,8]]]]" );
        }

        [Benchmark]
        public void NestedListAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<List<List<int>>>( @"[[1,2,3,4],[5,6,7,8]]" );
        }

        [Benchmark]
        public void NestedListJsonDeserialize()
        {
            JsonConvert.DeserializeObject<List<List<int>>>( @"[[1,2,3,4],[5,6,7,8]]" );
        }

        [Benchmark]
        public void ArrayAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<int[]>( @"[1,2,3,4,5,6,7,8,9]" );
        }

        [Benchmark]
        public void ArrayJsonDeserialize()
        {
            JsonConvert.DeserializeObject<int[]>( @"[1,2,3,4,5,6,7,8,9]" );
        }

        [Benchmark]
        public void StringAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<string>( @"""hello world""" );
        }

        [Benchmark]
        public void StringJsonDeserialize()
        {
            JsonConvert.DeserializeObject<string>( @"""hello world""" );
        }

        [Benchmark]
        public void LongAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<long>( 12414124124.ToString() );
        }

        [Benchmark]
        public void LongJsonDeserialize()
        {
            JsonConvert.DeserializeObject<long>( 12414124124.ToString() );
        }

        [Benchmark]
        public void BoolAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<bool>( "True" );
        }

        [Benchmark]
        public void BoolJsonDeserialize()
        {
            JsonConvert.DeserializeObject<bool>( "true" );
        }

        [Benchmark]
        public void DoubleAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<double>( 5521355.124.ToString() );
        }

        [Benchmark]
        public void DoubleJsonDeserialize()
        {
            JsonConvert.DeserializeObject<double>( 5521355.124.ToString() );
        }

        [Benchmark]
        public void EnumAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<System.Xml.Formatting>( "Indented" );
        }

        [Benchmark]
        public void EnumJsonDeserialize()
        {
            JsonConvert.DeserializeObject<System.Xml.Formatting>( "1" );
        }

        [Benchmark]
        public void ListAdeptarDeserialize()
        {
            AdeptarConverter.DeserializeString<List<string>>( @"[""Some"",""Random"",""Words"",""Words""]" );
        }

        [Benchmark]
        public void ListJsonDeserialize()
        {
            JsonConvert.DeserializeObject<List<string>>( @"[""Some"",""Random"",""Words"",""Words""]" );
        }
    }
}