﻿using System;
using System.Collections.Generic;
using Adeptar;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
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



#else
            BenchmarkDotNet.Running.BenchmarkRunner.Run<MemoryBenchmarkerDemo>();
            Console.ReadLine();
#endif
        }
    }

    [MemoryDiagnoser]
    public class MemoryBenchmarkerDemo
    {
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
            AdeptarConverter.Serialize( new MyClass() );
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
            } );
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
            } );
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
            } );
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

        [Benchmark]
        public void TupleAdeptar()
        {
            AdeptarConverter.Serialize( (1, "Hello World", new MyClass(), new int[] { 1, 2, 3, 4 }) );
        }

        [Benchmark]
        public void TupleJson()
        {
            JsonConvert.SerializeObject( (1, "Hello World", new MyClass(), new int[] { 1, 2, 3, 4 }) );
        }

        [Benchmark]
        public void DictionaryWithArrayKeyAdeptar()
        {
            AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } } );
        }

        [Benchmark]
        public void DictionaryWithArrayKeyJson()
        {
            JsonConvert.SerializeObject( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } } );
        }

        [Benchmark]
        public void DictionaryAdeptar()
        {
            AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } } );
        }

        [Benchmark]
        public void DictionaryJson()
        {
            JsonConvert.SerializeObject( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } } );
        }

        [Benchmark]
        public void FourDimensionalArrayAdeptar()
        {
            AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
        }

        [Benchmark]
        public void FourDimensionalArrayJson()
        {
            JsonConvert.SerializeObject( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
        }

        [Benchmark]
        public void NestedListAdeptar()
        {
            AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
        }

        [Benchmark]
        public void NestedListJson()
        {
            JsonConvert.SerializeObject( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
        }

        [Benchmark]
        public void ArrayAdeptar()
        {
            AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
        }

        [Benchmark]
        public void ArrayJson()
        {
            JsonConvert.SerializeObject( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
        }

        [Benchmark]
        public void StringAdeptar()
        {
            AdeptarConverter.Serialize( @"hello world" );
        }

        [Benchmark]
        public void StringJson()
        {
            JsonConvert.SerializeObject( @"hello world" );
        }

        [Benchmark]
        public void LongAdeptar()
        {
            AdeptarConverter.Serialize( 12414124124 );
        }

        [Benchmark]
        public void LongJson()
        {
            JsonConvert.SerializeObject( 12414124124 );
        }

        [Benchmark]
        public void BoolAdeptar()
        {
            AdeptarConverter.Serialize( true );
        }

        [Benchmark]
        public void BoolJson()
        {
            JsonConvert.SerializeObject( true );
        }

        [Benchmark]
        public void DoubleAdeptar()
        {
            AdeptarConverter.Serialize( 5521355.124 );
        }

        [Benchmark]
        public void DoubleJson()
        {
            JsonConvert.SerializeObject( 5521355.124 );
        }

        [Benchmark]
        public void EnumAdeptar()
        {
            AdeptarConverter.Serialize( System.Xml.Formatting.Indented );
        }

        [Benchmark]
        public void EnumJson()
        {
            JsonConvert.SerializeObject( System.Xml.Formatting.Indented );
        }

        [Benchmark]
        public void ListAdeptar()
        {
            AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" } );
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