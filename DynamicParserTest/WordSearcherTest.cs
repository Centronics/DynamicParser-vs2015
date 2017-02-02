using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicParser;

namespace DynamicParserTest
{
    [TestClass]
    public class WordSearcherTest
    {
        [TestMethod]
        public void WordSearchTest()
        {
            {
                WordSearcher ws = new WordSearcher(new[] { "0", "0", "1", "0", "1", "2" });
                Assert.AreEqual(true, ws.IsEqual("000"));
                Assert.AreEqual(true, ws.IsEqual("001"));
                Assert.AreEqual(true, ws.IsEqual("002"));
                Assert.AreEqual(true, ws.IsEqual("010"));
                Assert.AreEqual(true, ws.IsEqual("011"));
                Assert.AreEqual(true, ws.IsEqual("012"));
                Assert.AreEqual(true, ws.IsEqual("012"));
                Assert.AreEqual(true, ws.IsEqual("11111222"));
                Assert.AreEqual(true, ws.IsEqual("111112220"));
                Assert.AreEqual(true, ws.IsEqual("1100111222"));
                Assert.AreEqual(true, ws.IsEqual("012"));
                Assert.AreEqual(true, ws.IsEqual("021"));
                Assert.AreEqual(true, ws.IsEqual("210"));
                Assert.AreEqual(true, ws.IsEqual("102"));
                Assert.AreEqual(true, ws.IsEqual("000"));
                Assert.AreEqual(true, ws.IsEqual("111"));
                Assert.AreEqual(true, ws.IsEqual("222"));
                Assert.AreEqual(true, ws.IsEqual("22"));
                Assert.AreEqual(true, ws.IsEqual("11"));
                Assert.AreEqual(true, ws.IsEqual("01"));
                Assert.AreEqual(true, ws.IsEqual("10"));
                Assert.AreEqual(true, ws.IsEqual("02"));
                Assert.AreEqual(true, ws.IsEqual("20"));
                Assert.AreEqual(true, ws.IsEqual("0"));
                Assert.AreEqual(true, ws.IsEqual("1"));
                Assert.AreEqual(true, ws.IsEqual("2"));

                Assert.AreEqual(false, ws.IsEqual("030"));
                Assert.AreEqual(false, ws.IsEqual("0141"));
                Assert.AreEqual(false, ws.IsEqual("3"));
                Assert.AreEqual(false, ws.IsEqual(string.Empty));
                Assert.AreEqual(false, ws.IsEqual(null));
            }

            {
                WordSearcher ws = new WordSearcher(new[] { "0", "0", "1", "0", "1", "2" });
                Assert.AreEqual(false, ws.IsEqual(string.Empty));
                Assert.AreEqual(false, ws.IsEqual(null));
            }

            {
                WordSearcher ws = new WordSearcher(new[] { string.Empty, null, string.Empty, null });
                Assert.AreEqual(false, ws.IsEqual(string.Empty));
                Assert.AreEqual(false, ws.IsEqual(null));
                Assert.AreEqual(false, ws.IsEqual("3"));
                Assert.AreEqual(false, ws.IsEqual("20"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WordSearchException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new WordSearcher(null);
        }
    }
}