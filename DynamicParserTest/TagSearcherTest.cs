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
            TagSearcher ts = new TagSearcher("000");
            Assert.AreEqual(false, ts.IsEqual(string.Empty));
            Assert.AreEqual(false, ts.IsEqual(null));
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
            ts = new TagSearcher("Aab1");
            Assert.AreEqual(true, ts.IsEqual("AaB1"));
            Assert.AreEqual(true, ts.IsEqual("aab1"));
            Assert.AreEqual(true, ts.IsEqual("b1aa"));
            Assert.AreEqual(true, ts.IsEqual("1aba"));
            Assert.AreEqual(true, ts.IsEqual("ab1a"));
            Assert.AreEqual(true, ts.IsEqual("AB1a"));
            Assert.AreEqual(true, ts.IsEqual("B1aa"));
            Assert.AreEqual(true, ts.IsEqual("1baa"));
            Assert.AreEqual(true, ts.IsEqual("1bAA"));
            Assert.AreEqual(true, ts.IsEqual("1bAa"));
            Assert.AreEqual(false, ts.IsEqual("aab1a"));
            Assert.AreEqual(false, ts.IsEqual("ba1c"));
            Assert.AreEqual(false, ts.IsEqual("hab1a"));
            Assert.AreEqual(false, ts.IsEqual("ba"));
            Assert.AreEqual(false, ts.IsEqual("bb1a"));
            Assert.AreEqual(false, ts.IsEqual("bb1aa"));
            Assert.AreEqual(ts.SourceString, "AAB1");
            Assert.AreNotEqual(ts.SourceString, "aab1");
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