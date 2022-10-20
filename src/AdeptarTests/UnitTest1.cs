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
            public DateTime date;
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
        public void EmptyClassTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new MyClass(), Formatting.NoIndentation ),
                "{date: \"1/1/0001 12:00:00 AM\",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore}" );
        }

        [TestMethod]
        public void ClassWithAdeptarConfiguration ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new MyClassWithConfig(), Formatting.NoIndentation ),
               @"{date: ""1/1/0001 12:00:00 AM"",Number: 0,Number3: 0,type: Ignore}" );
        }

        [TestMethod]
        public void HeavyClassTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new MyClass()
                {
                    date = DateTime.Now,
                    Maps = new Dictionary<int, string>() { { 1, "Hello" }, { 2, "World" } },
                    Number = 4,
                    Number3 = -444,
                    Odds = new int[] { 1, 3, 5, 7, 9 },
                    type = SerializableType.Tuple
                }, Formatting.NoIndentation ),
                @"{date: """ + $"{DateTime.Now}" + @""",Maps: [1:""Hello"",2:""World""],Number: 4,Number3: -444,Odds: [1,3,5,7,9],type: Tuple}" );
        }

        [TestMethod]
        public void HeavyTupleTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( (1, 4, new MyClass(), "hello world", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3, '3', true, 0.00001), Formatting.NoIndentation ),
                @"(Item1: 1,Item2: 4,Item3: {date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},Item4: ""hello world"",Item5: [1,2,3,4,5,6,7,8,9],Item6: 3,Item7: '3',Rest: (Item1: True,Item2: 1E-05))" );
        }

        [TestMethod]
        public void SimpleClassTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize(new SimpleClass(), Formatting.NoIndentation ),
                @"{Number: 5,Number2: 5,Number3: 5}" );
        }

        [TestMethod]
        public void NestedDictionaryTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } }, Formatting.NoIndentation),
                @"[1:[1,2,3,4],2:[3,4,5,6]]" );
        }

        [TestMethod]
        public void DictionaryTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } }, Formatting.NoIndentation ),
                @"[1:2,3:4]" );
        }

        [TestMethod]
        public void FourDimensionalArrayTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, Formatting.NoIndentation ),
                @"[<2,2,2,2>1,2,3,4,5,6,7,8,1,2,3,4,5,6,7,8]" );
        }

        [TestMethod]
        public void FourDimensionalArrayWithClassTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new MyClass[2, 2, 2, 2] { { { { new(), new() }, { new(), new() } }, { { new(), new() }, { new(), new() } } }, { { { new(), new() }, { new(), new() } }, { { new(), new() }, { new(), new()} } } }, Formatting.NoIndentation ),
                @"[<2,2,2,2>{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore}]" );
        }

        [TestMethod]
        public void NestedListTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } }, Formatting.NoIndentation ),
                @"[[1,2,3,4],[5,6,7,8]]" );
        }

        [TestMethod]
        public void NestedListWithClassTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new List<List<MyClass>>() { new List<MyClass>() { new(), new(), new(), new() }, new List<MyClass>() { new(), new(), new(), new() } }, Formatting.NoIndentation ),
                @"[[{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore}],[{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore},{date: ""1/1/0001 12:00:00 AM"",Maps: [],Number: 0,Number3: 0,Odds: [],type: Ignore}]]" );
        }

        [TestMethod]
        public void ArrayTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, }, Formatting.NoIndentation ),
                @"[1,2,3,4,5,6,7,8,9]" );
        }

        [TestMethod]
        public void ListTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" }, Formatting.NoIndentation ),
                @"[""Some"",""Random"",""Words"",""Words""]" );
        }

        [TestMethod]
        public void StringTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( @"hello world", Formatting.NoIndentation ),
                @"""hello world""" );
        }

        [TestMethod]
        public void LongTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( 12414124124 , Formatting.NoIndentation ),
                @"12414124124" );
        }

        [TestMethod]
        public void BoolTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( true , Formatting.NoIndentation ),
                @"True" );
        }

        [TestMethod]
        public void DoubleTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( 5521355.124 , Formatting.NoIndentation ),
                @"5521355.124" );
        }

        [TestMethod]
        public void EnumTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( Formatting.Indented, Formatting.NoIndentation ),
                @"Indented" );
        }

        [TestMethod]
        public void SimpleClassTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new SimpleClass(), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<SimpleClass>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void NestedDictionaryTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new Dictionary<int, int[]> { { 1, new int[] { 1, 2, 3, 4 } }, { 2, new int[] { 3, 4, 5, 6 } } }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<Dictionary<int, int[]>>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void DictionaryTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<Dictionary<int, int>>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void FourDimensionalArrayTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<int[,,,]>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void NestedListTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<List<List<int>>>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void ArrayTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<int[]>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void ListTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new List<string>() { "Some", "Random", "Words", "Words" }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<List<string>>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void StringTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( @"hello world", Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<string>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void LongTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( 12414124124, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<Int64>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void BoolTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( true, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<bool>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void DoubleTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( 5521355.124, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<double>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void EnumTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( Formatting.Indented, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<Formatting>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void EmptyClassTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new MyClass(), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClass>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void ClassWithAdeptarConfigurationDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new MyClassWithConfig(), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClassWithConfig>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void HeavyClassTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new MyClass()
            {
                date = DateTime.Now,
                Maps = new Dictionary<int, string>() { { 1, "Hello" }, { 2, "World" } },
                Number = 4,
                Number3 = -444,
                Odds = new int[] { 1, 3, 5, 7, 9 },
                type = SerializableType.Tuple
            }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClass>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void HeavyTupleTestDeserialize ()
        {
            string text = AdeptarConverter.Serialize( (1, 4, new MyClass(), "hello world", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3, '3', true, 0.00001), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<(int, int, MyClass, string, int[], int, char, bool, double)>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void NestedListWithClassDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new List<List<MyClass>>() { new List<MyClass>() { new(), new(), new(), new() }, new List<MyClass>() { new(), new(), new(), new() } }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<List<List<MyClass>>>( text ), Formatting.NoIndentation ) );
        }

        [TestMethod]
        public void FourDimensionalArrayWithClassDeserialize ()
        {
            string text = AdeptarConverter.Serialize( new MyClass[2, 2, 2, 2] { { { { new(), new() }, { new(), new() } }, { { new(), new() }, { new(), new() } } }, { { { new(), new() }, { new(), new() } }, { { new(), new() }, { new(), new() } } } }, Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClass[,,,]>( text ), Formatting.NoIndentation ) );
        }
    }
}