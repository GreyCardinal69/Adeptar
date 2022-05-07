using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adeptar;
using System.Collections.Generic;
using System;

namespace AdeptarTests
{
    [TestClass]
    public class UnitTest1
    {
        private class MyClass
        {
            public int Number = 5;
            public int Number2 = 5;
            public int Number3 = 5;
        }

        [TestMethod]
        public void SimpleClassTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize(new MyClass(), Formatting.NoIndentation ),
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
        public void NestedListTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 6, 7, 8 } }, Formatting.NoIndentation ),
                @"[[1,2,3,4],[5,6,7,8]]" );
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
            string text = AdeptarConverter.Serialize( new MyClass(), Formatting.NoIndentation );
            Assert.AreEqual(
                text,
                AdeptarConverter.Serialize(
                AdeptarConverter.DeserializeString<MyClass>( text ), Formatting.NoIndentation ) );
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
    }
}