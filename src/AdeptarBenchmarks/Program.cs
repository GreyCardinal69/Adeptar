using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Adeptar;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using FastMember;

namespace AdeptarBenchmarks
{
    [MemoryDiagnoser]
    public class Program
    {
        public struct TestStruct
        {
            public AssemblyNameFlags y;
            public string name;
            public List<float> floaties;
        }

        public class MyClass
        {
            public int Number;
            public int Number2;
            public int Number3;
            public int[] odds;
            public string[] swords;
            public Dictionary<int, int> nums;
            public Dictionary<int, bool[]> numsbools;
            public TestStruct structy;
        }

        static void Main ( string[] args )
        {
#if DEBUG
            string serializePath = AppDomain.CurrentDomain.BaseDirectory + @"seri.ader";
            string deserializePath = AppDomain.CurrentDomain.BaseDirectory + @"deser.ader";


            var t = (1, (1,2,3), true, "string", "oh\"", new int[] { 6, 7, 8, 9 }, new MemoryBenchmarkerDemo.MyClass(), System.Xml.Formatting.Indented);

            AdeptarConverter.SerializeWrite( serializePath, t, Adeptar.Formatting.NoIndentation );
            Console.WriteLine(JsonConvert.SerializeObject(t));
            var x = AdeptarConverter.DeserializeString( AdeptarConverter.Serialize( t ), t.GetType() );
            AdeptarConverter.SerializeWrite( deserializePath, x );
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
            public int Number = 1;
            public int Number2 = 2;
            public int Number3 = 3;
        }

        public enum TestEnum
        {
            Test,
            Enum
        }

        public static object x = (1, (1, 2, 3), true, "string", "oh\"", new int[] { 6, 7, 8, 9 }, new MemoryBenchmarkerDemo.MyClass(), System.Xml.Formatting.Indented);

        public static string xx = JsonConvert.SerializeObject( x );
        public static string yy = AdeptarConverter.Serialize( x, Adeptar.Formatting.NoIndentation );

        [Benchmark]
        public void ComplexTupleAder ()
        {
            AdeptarConverter.DeserializeString<(int, (int,int,int), bool, string, string, int[], MemoryBenchmarkerDemo.MyClass, System.Xml.Formatting)>( yy );
        }

        [Benchmark]
        public void ComplexTupleJson ()
        {
            JsonConvert.DeserializeObject<(int, (int, int, int), bool, string, string, int[], MemoryBenchmarkerDemo.MyClass, System.Xml.Formatting)>(xx );
        }

        /*
        [Benchmark]
        public void SimpleClassAder ()
        {
            AdeptarConverter.Serialize( new MyClass() );
        }

        [Benchmark]
        public void SimpleClassJson ()
        {
            JsonConvert.SerializeObject( new MyClass() );
        }

        [Benchmark]
        public void ComplexDictionaryAder ()
        {
            AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } } );
        }

        [Benchmark]
        public void ComplexDictionaryJson ()
        {
            JsonConvert.SerializeObject( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } }, Newtonsoft.Json.Formatting.Indented );
        }

        [Benchmark]
        public void SimpleDictionaryAder ()
        {
            AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } } );
        }

        [Benchmark]
        public void SimpleDictionaryJson ()
        {
            JsonConvert.SerializeObject( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } } );
        }

        [Benchmark]
        public void FourDimensionalArrayAder ()
        {
            AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
        }

        [Benchmark]
        public void FourDimensionalArrayJson ()
        {
            JsonConvert.SerializeObject( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
        }

        [Benchmark]
        public void NestedListAder ()
        {
            AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
        }

        [Benchmark]
        public void NestedListJson ()
        {
            JsonConvert.SerializeObject( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
        }

        [Benchmark]
        public void SimpleArrayAder ()
        {
            AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
        }

        [Benchmark]
        public void SimpleArrayJson ()
        {
            JsonConvert.SerializeObject( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
        }

        [Benchmark]
        public void SimpleListAder ()
        {
            AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" } );
        }

        [Benchmark]
        public void SimpleListJson ()
        {
            JsonConvert.SerializeObject( new List<string>() { "Some", "Random", "Words", "Words" } );
        }

        [Benchmark]
        public void WriteStringAder ()
        {
            AdeptarConverter.Serialize( "hello world" );
        }

        [Benchmark]
        public void WriteStringJson ()
        {
            JsonConvert.SerializeObject( "hello world" );
        }

        [Benchmark]
        public void WriteNumberAder ()
        {
            AdeptarConverter.Serialize( 12414124124 );
        }

        [Benchmark]
        public void WriteNumberJson ()
        {
            JsonConvert.SerializeObject( 12414124124 );
        }

        [Benchmark]
        public void WriteBoolAder ()
        {
            AdeptarConverter.Serialize( true );
        }

        [Benchmark]
        public void WriteBoolJson ()
        {
            JsonConvert.SerializeObject( true );
        }


        [Benchmark]
        public void WriteDoubleAder ()
        {
            AdeptarConverter.Serialize( 5521355.124 );
        }

        [Benchmark]
        public void WriteDoubleJson ()
        {
            JsonConvert.SerializeObject( 5521355.124 );
        }

        [Benchmark]
        public void WriteEnumAder ()
        {
            AdeptarConverter.Serialize( TestEnum.Test );
        }

        [Benchmark]
        public void WriteEnumJson ()
        {
            JsonConvert.SerializeObject( TestEnum.Test );
        }

        [Benchmark]
        public void WriteDateTimeAder ()
        {
            AdeptarConverter.Serialize( DateTime.Now );
        }

        [Benchmark]
        public void WriteDateTimeJson ()
        {
            JsonConvert.SerializeObject( DateTime.Now );
        }*/
    }
}