using System;
using DynamicParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicParserTest
{
    [TestClass]
    public class WordSearcherTest
    {
        [TestMethod]
        public void WordSearchTest()
        {
            {
                WordSearcher ws = new WordSearcher(new[] {"0", "0", "1", "0", "1", "2"});
                Assert.AreEqual(3, ws.Count);
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
                WordSearcher ws = new WordSearcher(new[] {"00", "09", "rr", "rrR", "RR", "09", "f"});
                Assert.AreEqual(5, ws.Count);
                Assert.AreEqual(true, ws.IsEqual("00"));
                Assert.AreEqual(false, ws.IsEqual("r9"));
                Assert.AreEqual(true, ws.IsEqual("rr09"));
                Assert.AreEqual(false, ws.IsEqual("09r0"));
                Assert.AreEqual(true, ws.IsEqual("rr0f"));
                Assert.AreEqual(true, ws.IsEqual("rrf"));
                Assert.AreEqual(true, ws.IsEqual("rf09"));
                Assert.AreEqual(true, ws.IsEqual("rrf09"));
                Assert.AreEqual(true, ws.IsEqual("rr09f"));
                Assert.AreEqual(true, ws.IsEqual("RR0f"));
                Assert.AreEqual(true, ws.IsEqual("rRf"));
                Assert.AreEqual(true, ws.IsEqual("Rf09"));
                Assert.AreEqual(true, ws.IsEqual("RrF09"));
                Assert.AreEqual(true, ws.IsEqual("rR09F"));
                Assert.AreEqual(true, ws.IsEqual("frR0f9Ff"));
            }

            {
                WordSearcher ws = new WordSearcher(new[] {"0", "0", "1", "0", "1", "2"});
                Assert.AreEqual(3, ws.Count);
                Assert.AreEqual(false, ws.IsEqual(string.Empty));
                Assert.AreEqual(false, ws.IsEqual(null));
            }

            {
                WordSearcher ws = new WordSearcher(new[] {string.Empty, null, string.Empty, null});
                Assert.AreEqual(0, ws.Count);
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