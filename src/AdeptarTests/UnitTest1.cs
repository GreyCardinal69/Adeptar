using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adeptar;
using System.Collections.Generic;
using System;

namespace AdeptarTests
{
    [TestClass]
    public class UnitTest1
    {
        private class SimpleClass
        {
            public int Number = 5;
            public int Number2 = 5;
            public int Number3 = 5;
        }

        private class MyClass
        {
            public SerializableType type = SerializableType.Ignore;
            public int Number;
            public int Number3;
            public int[] Odds;
            public Dictionary<int, string> Maps;
            public DateTime date = DateTime.MinValue;
        }

        private class MyClassWithConfig
        {
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

        [TestMethod]
        public void EmptyClassTest()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            Assert.AreEqual(
                AdeptarConverter.Serialize( new MyClass(), typeof( MyClass ), Formatting.NoIndentation ),
                "{date: \"01-Jan-01 12:00:00 AM\",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore}" );
        }

        [TestMethod]
        public void ClassWithAdeptarConfiguration()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClassWithConfig ), new MyClassWithConfig() );
            Assert.AreEqual(
                AdeptarConverter.Serialize( new MyClassWithConfig(), typeof( MyClassWithConfig ), Formatting.NoIndentation ),
               @"{date: ""01-Jan-01 12:00:00 AM"",Number: 0,Number3: 0,type: Ignore}" );
        }

        [TestMethod]
        public void HeavyClassTest()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            Assert.AreEqual(
                AdeptarConverter.Serialize( new MyClass()
                {
                    date = DateTime.Now,
                    Maps = new Dictionary<int, string>() { { 1, "Hello" }, { 2, "World" } },
                    Number = 4,
                    Number3 = -444,
                    Odds = new int[] { 1, 3, 5, 7, 9 },
                    type = SerializableType.Tuple
                }, typeof( MyClass ), Formatting.NoIndentation ),
                @"{date: """ + $"{DateTime.Now}" + @""",Maps: [1:""Hello"",2:""World""],Number: 4,Number3: -444,Odds: [1,3,5,7,9],type: Tuple}" );
        }

        [TestMethod]
        public void HeavyTupleTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( (1, 4, new MyClass(), "hello world", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3, '3', true, 0.00001), typeof( (int, int, MyClass, string, int[], int, char, bool, double) ), Formatting.NoIndentation ),
                @"(Item1: 1,Item2: 4,Item3: {date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},Item4: ""hello world"",Item5: [1,2,3,4,5,6,7,8,9],Item6: 3,Item7: '3',Rest: (Item1: True,Item2: 1E-05))" );
        }

        [TestMethod]
        public void SimpleClassTest()
        {
            AdeptarBaker.BakeClassStruct( typeof( SimpleClass ), new SimpleClass() );
            Assert.AreEqual(
                AdeptarConverter.Serialize( new SimpleClass(), typeof( SimpleClass ), Formatting.NoIndentation ),
                @"{Number: 5,Number2: 5,Number3: 5}" );
        }

        [TestMethod]
        public void NestedDictionaryTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } }, typeof( Dictionary<int, int[]> ), Formatting.NoIndentation ),
                @"[1:[1,2,3,4],2:[3,4,5,6]]" );
        }

        [TestMethod]
        public void DictionaryTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } }, typeof( Dictionary<int, int> ), Formatting.NoIndentation ),
                @"[1:2,3:4]" );
        }

        [TestMethod]
        public void FourDimensionalArrayTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, typeof( int[,,,] ), Formatting.NoIndentation ),
                @"[<2,2,2,2>1,2,3,4,5,6,7,8,1,2,3,4,5,6,7,8]" );
        }

        [TestMethod]
        public void FourDimensionalArrayWithClassTest()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            Assert.AreEqual(
                AdeptarConverter.Serialize( new MyClass[2, 2, 2, 2] { { { { new(), new() }, { new(), new() } }, { { new(), new() }, { new(), new() } } }, { { { new(), new() }, { new(), new() } }, { { new(), new() }, { new(), new() } } } }, typeof( MyClass[,,,] ), Formatting.NoIndentation ),
                @"[<2,2,2,2>{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore}]" );
        }

        [TestMethod]
        public void NestedListTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } }, typeof( List<List<int>> ), Formatting.NoIndentation ),
                @"[[1,2,3,4],[5,6,7,8]]" );
        }

        [TestMethod]
        public void NestedListWithClassTest()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            Assert.AreEqual(
                AdeptarConverter.Serialize( new List<List<MyClass>>() { new List<MyClass>() { new(), new(), new(), new() }, new List<MyClass>() { new(), new(), new(), new() } }, typeof( List<List<MyClass>> ), Formatting.NoIndentation ),
                @"[[{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore}],[{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""01-Jan-01 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore}]]" );
        }

        [TestMethod]
        public void ArrayTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, }, typeof( int[] ), Formatting.NoIndentation ),
                @"[1,2,3,4,5,6,7,8,9]" );
        }

        [TestMethod]
        public void ListTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" }, typeof( List<string> ), Formatting.NoIndentation ),
                @"[""Some"",""Random"",""Words"",""Words""]" );
        }

        [TestMethod]
        public void StringTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( @"hello world", typeof( string ), Formatting.NoIndentation ),
                @"""hello world""" );
        }

        [TestMethod]
        public void LongTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( 12414124124, typeof( long ), Formatting.NoIndentation ),
                @"12414124124" );
        }

        [TestMethod]
        public void BoolTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( true, typeof( bool ), Formatting.NoIndentation ),
                @"True" );
        }

        [TestMethod]
        public void DoubleTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( 5521355.124, typeof( double ), Formatting.NoIndentation ),
                @"5521355.124" );
        }

        [TestMethod]
        public void EnumTest()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( Formatting.Indented, typeof( Enum ), Formatting.NoIndentation ),
                @"Indented" );
        }

        [TestMethod]
        public void SimpleClassTestDeserialize()
        {
            AdeptarBaker.BakeClassStruct( typeof( SimpleClass ), new SimpleClass() );
            string text = AdeptarConverter.Serialize( new SimpleClass(), typeof( SimpleClass ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<SimpleClass>( text ), typeof( SimpleClass ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void NestedDictionaryTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } }, typeof( Dictionary<int, int[]> ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<Dictionary<int, int[]>>( text ), typeof( Dictionary<int, int[]> ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void DictionaryTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } }, typeof( Dictionary<int, int> ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<Dictionary<int, int>>( text ), typeof( Dictionary<int, int> ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void FourDimensionalArrayTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, typeof( int[,,,] ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<int[,,,]>( text ), typeof( int[,,,] ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void NestedListTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } }, typeof( List<List<int>> ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<List<List<int>>>( text ), typeof( List<List<int>> ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void ArrayTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, }, typeof( int[] ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<int[]>( text ), typeof( int[] ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void ListTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" }, typeof( List<string> ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<List<string>>( text ), typeof( List<string> ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void StringTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( @"hello world", typeof( string ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<string>( text ), typeof( string ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void LongTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( 12414124124, typeof( long ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<Int64>( text ), typeof( long ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void BoolTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( true, typeof( bool ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<bool>( text ), typeof( bool ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void DoubleTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( 5521355.124, typeof( double ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<double>( text ), typeof( double ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void EnumTestDeserialize()
        {
            string text = AdeptarConverter.Serialize( Formatting.Indented, typeof( Enum ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<Formatting>( text ), typeof( Enum ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void EmptyClassTestDeserialize()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            string text = AdeptarConverter.Serialize( new MyClass(), typeof( MyClass ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClass>( text ), typeof( MyClass ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void ClassWithAdeptarConfigurationDeserialize()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClassWithConfig ), new MyClassWithConfig() );
            string text = AdeptarConverter.Serialize( new MyClassWithConfig(), typeof( MyClassWithConfig ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClassWithConfig>( text ), typeof( MyClassWithConfig ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void HeavyClassTestDeserialize()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            string text = AdeptarConverter.Serialize( new MyClass()
            {
                date = DateTime.Now,
                Maps = new Dictionary<int, string>() { { 1, "Hello" }, { 2, "World" } },
                Number = 4,
                Number3 = -444,
                Odds = new int[] { 1, 3, 5, 7, 9 },
                type = SerializableType.Tuple
            }, typeof( MyClass ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClass>( text ), typeof( MyClass ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void HeavyTupleTestDeserialize()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            string text = AdeptarConverter.Serialize( (1, 4, new MyClass(), "hello world", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3, '3', true, 0.00001), typeof( (int, int, MyClass, string, int[], int, char, bool, double) ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<(int, int, MyClass, string, int[], int, char, bool, double)>( text ), typeof( (int, int, MyClass, string, int[], int, char, bool, double) ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void NestedListWithClassDeserialize()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            string text = AdeptarConverter.Serialize( new List<List<MyClass>>() { new List<MyClass>() { new(), new(), new(), new() }, new List<MyClass>() { new(), new(), new(), new() } }, typeof( List<MyClass> ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<List<List<MyClass>>>( text ), typeof( List<MyClass> ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void FourDimensionalArrayWithClassDeserialize()
        {
            AdeptarBaker.BakeClassStruct( typeof( MyClass ), new MyClass() );
            string text = AdeptarConverter.Serialize( new MyClass[2, 2, 2, 2] { { { { new(), new() }, { new(), new() } }, { { new(), new() }, { new(), new() } } }, { { { new(), new() }, { new(), new() } }, { { new(), new() }, { new(), new() } } } }, typeof( MyClass[,,,] ), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClass[,,,]>( text ), typeof( MyClass[,,,] ), Formatting.NoIndentation ) );
        }
    }
}