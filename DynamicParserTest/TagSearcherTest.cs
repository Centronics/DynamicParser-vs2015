using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicParser;

namespace DynamicParserTest
{
    [TestClass]
    public class TagSearcherTest
    {
        [TestMethod]
        public void TagSearcherTest1()
        {
            //ДОБАВИТЬ БУКВЫ
            TagSearcher ts = new TagSearcher("000");
            Assert.AreEqual(true, ts.IsEqual("000"));
            ts = new TagSearcher("100");
            Assert.AreEqual(true, ts.IsEqual("100"));
            Assert.AreEqual(true, ts.IsEqual("010"));
            Assert.AreEqual(true, ts.IsEqual("001"));
            ts = new TagSearcher("10");
            Assert.AreEqual(true, ts.IsEqual("10"));
            Assert.AreEqual(true, ts.IsEqual("01"));
            Assert.AreEqual(false, ts.IsEqual("00"));
            Assert.AreEqual(false, ts.IsEqual("11"));
            ts = new TagSearcher("5");
            Assert.AreEqual(true, ts.IsEqual("5"));
            Assert.AreEqual(false, ts.IsEqual("0"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TagSearcherTest2()
        {
            // ReSharper disable once UnusedVariable
            TagSearcher ts = new TagSearcher("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TagSearcherTest3()
        {
            // ReSharper disable once UnusedVariable
            TagSearcher ts = new TagSearcher(null);
        }
    }
}