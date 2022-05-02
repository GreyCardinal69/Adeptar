﻿using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Adeptar;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace AdeptarBenchmarks
{
    [MemoryDiagnoser]
    class Program
    {
        private class TestClass
        {
            public int x;
            public string z;
            public int[] odds;
            public string[] words;
            public Dictionary<int, int> nums;
            public Dictionary<int, bool[]> numsbools;

        }

        static void Main ( string[] args )
        {
#if DEBUG
            string serializePath = AppDomain.CurrentDomain.BaseDirectory + @"seri.ader";
            string deserializePath = AppDomain.CurrentDomain.BaseDirectory + @"deser.ader";

            var y = new Dictionary<Dictionary<int, int>, Dictionary<int, int>>()
            {
                { new Dictionary<int,int>() { { 1,2 }, { 3, 4} }, new Dictionary<int, int>() { {5,6 }, { 7, 8 } } },
                { new Dictionary<int,int>() { { 1,2 }, { 3, 4} }, new Dictionary<int, int>() { {5,6 }, { 7, 8 } } },
                { new Dictionary<int,int>() { { 1,2 }, { 3, 4} }, new Dictionary<int, int>() { {5,6 }, { 7, 8 } } }
            };

            AdeptarConverter.SerializeWrite( serializePath, y  );
            var x = AdeptarConverter.DeserializeString<Dictionary<Dictionary<int, int>, Dictionary<int, int>>>( AdeptarConverter.Serialize( y ) );
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
            public int Number = 5;
            public int Number2 = 5;
            public int Number3 = 5;
        }

        public enum TestEnum
        {
            Test,
            Enum
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