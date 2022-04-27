using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adeptar;
using System.Collections.Generic;

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
                @"[1:[1,2,3,4],2:[3,4,5,6]] " );
        }

        [TestMethod]
        public void DictionaryTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new Dictionary<int, int> { { 1, 2 }, { 3, 4 } }, Formatting.NoIndentation ),
                @"[1:2,3:4] " );
        }

        [TestMethod]
        public void FourDimensionalArrayTest ()
        {
            Assert.AreEqual(
                AdeptarConverter.Serialize( new int[2, 2, 2, 2] { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, Formatting.NoIndentation ),
                @"[	<2,2,2,2>1,2,3,4,5,6,7,8,1,2,3,4,5,6,7,8]" );
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
    }
}