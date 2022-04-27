using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Adeptar;
using Newtonsoft.Json;
using System.IO;
using Fasterflect;
using FastMember;

namespace AdeptarTests
{
    [MemoryDiagnoser]
    class Program
    {
        public class MyClass
        {
            [AdeptarIgnore]
            public int Number = 5;
            public int Number2 = 5;
            public int Number3 = 5;
        }

        static void Main ( string[] args )
        {
#if DEBUG
            string serializePath = AppDomain.CurrentDomain.BaseDirectory + @"seri.ader";
            string deserializePath = AppDomain.CurrentDomain.BaseDirectory + @"deser.ader";


            AdeptarConverter.SerializeWrite( serializePath, new MyClass() );
            File.WriteAllText( deserializePath, JsonConvert.SerializeObject( new MyClass() ) );
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
        int IterationCount =  10000000;

        [AdeptarIgnore("Number2")]
        public class MyClass
        {
            [JsonIgnore]
            public int Number = 5;
            public int Number2 = 5;
            public int Number3 = 5;
        }

        public enum TestEnum
        {
            Test,
            Enum
        }

        [Benchmark]
        public void SimpleClassAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( new MyClass() );
            }
        }

        [Benchmark]
        public void SimpleClassJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( new MyClass() );
            }
        }

        [Benchmark]
        public void ComplexDictionaryAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } } );
            }
        }

        [Benchmark]
        public void ComplexDictionaryJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } }, Newtonsoft.Json.Formatting.Indented );
            }
        }

        [Benchmark]
        public void SimpleDictionaryAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 },{ 3, 4 } } );
            }
        }

        [Benchmark]
        public void SimpleDictionaryJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } } );
            }
        }

        [Benchmark]
        public void FourDimensionalArrayAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
            }
        }

        [Benchmark]
        public void FourDimensionalArrayJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } );
            }
        }

        [Benchmark]
        public void NestedListAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
            }
        }

        [Benchmark]
        public void NestedListJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } } );
            }
        }

        [Benchmark]
        public void SimpleArrayAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
            }
        }

        [Benchmark]
        public void SimpleArrayJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, } );
            }
        }

        [Benchmark]
        public void SimpleListAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" } );
            }
        }

        [Benchmark]
        public void SimpleListJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( new List<string>() { "Some", "Random", "Words", "Words" } );
            }
        }

        [Benchmark]
        public void WriteStringAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( "hello world" );
            }
        }

        [Benchmark]
        public void WriteStringJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject("hello world");
            }
        }

        [Benchmark]
        public void WriteNumberAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( 12414124124 );
            }
        }

        [Benchmark]
        public void WriteNumberJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( 12414124124 );
            }
        }

        [Benchmark]
        public void WriteBoolAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( true );
            }
        }

        [Benchmark]
        public void WriteBoolJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( true );
            }
        }


        [Benchmark]
        public void WriteDoubleAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( 5521355.124 );
            }
        }

        [Benchmark]
        public void WriteDoubleJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( 5521355.124 );
            }
        }

        [Benchmark]
        public void WriteEnumAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( TestEnum.Test );
            }
        }

        [Benchmark]
        public void WriteEnumJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( TestEnum.Test );
            }
        }

        [Benchmark]
        public void WriteDateTimeAder ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                AdeptarConverter.Serialize( DateTime.Now );
            }
        }

        [Benchmark]
        public void WriteDateTimeJson ()
        {
            for (int i = 0; i < IterationCount; i++)
            {
                JsonConvert.SerializeObject( DateTime.Now );
            }
        }
    }
}