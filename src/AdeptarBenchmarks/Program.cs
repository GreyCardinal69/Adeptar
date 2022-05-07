using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Adeptar;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using FastMember;
using System.Collections;
using System.Linq;

namespace AdeptarBenchmarks
{
    [MemoryDiagnoser]
    public class Program
    {
        static void Main ( string[] args )
        {
#if DEBUG
            string serializePath = AppDomain.CurrentDomain.BaseDirectory + @"seri.ader";
            string deserializePath = AppDomain.CurrentDomain.BaseDirectory + @"deser.ader";

            Console.WriteLine(AdeptarConverter.Serialize(new MemoryBenchmarkerDemo.MyClass(), Adeptar.Formatting.NoIndentation));
#else
            BenchmarkDotNet.Running.BenchmarkRunner.Run<MemoryBenchmarkerDemo>();
            Console.ReadLine();
#endif
        }
    }

    [MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class MemoryBenchmarkerDemo
    {
        public class MyClass
        {
            public int Number = 5;
            public int Number2 = 5;
            public int Number3 = 5;
            public int[] Odds = new int[] { 1, 3, 5, 7, 9 };
            public Dictionary<int, string> Maps = new()
            {
                { 1, "Hello" },
                { 2, "World" }
            };
        }


        [Benchmark]
        public void ClassAdeptar ()
        {
            AdeptarConverter.Serialize( new MyClass() );
        }

        [Benchmark]
        public void ClassJson ()
        {
            JsonConvert.SerializeObject( new MyClass() );
        }

        [Benchmark]
        public void TupleAdeptar ()
        {
            AdeptarConverter.Serialize( ( 1, "Hello World", new MyClass(), new int[] { 1,2,3,4 } ) );
        }

        [Benchmark]
        public void TupleJson ()
        {
            JsonConvert.SerializeObject( ( 1, "Hello World", new MyClass(), new int[] { 1, 2, 3, 4 } ) );
        }

        [Benchmark]
        public void DictionaryWithArrayKeyAdeptar ()
        {
            AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } } );
        }

        [Benchmark]
        public void DictionaryWithArrayKeyJson ()
        {
            JsonConvert.SerializeObject( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } } );
        }

        [Benchmark]
        public void DictionaryAdeptar ()
        {
            AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } } );
        }

        [Benchmark]
        public void DictionaryJson ()
        {
            JsonConvert.SerializeObject( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } } );
        }

        [Benchmark]
        public void FourDimensionalArrayAdeptar ()
        {
            AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
        }

        [Benchmark]
        public void FourDimensionalArrayJson ()
        {
            JsonConvert.SerializeObject( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
        }

        [Benchmark]
        public void NestedListAdeptar ()
        {
            AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
        }

        [Benchmark]
        public void NestedListJson ()
        {
            JsonConvert.SerializeObject( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
        }

        [Benchmark]
        public void ArrayAdeptar ()
        {
            AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
        }

        [Benchmark]
        public void ArrayJson ()
        {
            JsonConvert.SerializeObject( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
        }

        [Benchmark]
        public void StringAdeptar ()
        {
            AdeptarConverter.Serialize( @"hello world" );
        }

        [Benchmark]
        public void StringJson ()
        {
            JsonConvert.SerializeObject( @"hello world" );
        }

        [Benchmark]
        public void LongAdeptar ()
        {
            AdeptarConverter.Serialize( 12414124124 );
        }

        [Benchmark]
        public void LongJson ()
        {
            JsonConvert.SerializeObject( 12414124124 );
        }

        [Benchmark]
        public void BoolAdeptar ()
        {
            AdeptarConverter.Serialize( true );
        }

        [Benchmark]
        public void BoolJson ()
        {
            JsonConvert.SerializeObject( true );
        }

        [Benchmark]
        public void DoubleAdeptar ()
        {
            AdeptarConverter.Serialize( 5521355.124 );
        }

        [Benchmark]
        public void DoubleJson ()
        {
            JsonConvert.SerializeObject( 5521355.124 );
        }

        [Benchmark]
        public void EnumAdeptar ()
        {
            AdeptarConverter.Serialize( System.Xml.Formatting.Indented );
        }

        [Benchmark]
        public void EnumJson ()
        {
            JsonConvert.SerializeObject( System.Xml.Formatting.Indented );
        }

        [Benchmark]
        public void ListAdeptar ()
        {
            AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" } );
        }

        [Benchmark]
        public void ListJson ()
        {
            JsonConvert.SerializeObject( new List<string>() { "Some", "Random", "Words", "Words" } );
        }
    }
}