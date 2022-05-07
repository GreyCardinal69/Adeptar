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





    }
}