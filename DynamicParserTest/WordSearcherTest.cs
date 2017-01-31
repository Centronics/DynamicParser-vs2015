using System.Collections.Generic;
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
                WordSearcher ws = new WordSearcher(new[] { new List<string> { "0" }, new List<string> { "0", "1" }, new List<string> { "0", "1", "2" } });
                Assert.AreEqual(true, ws.IsEqual("000"));
                Assert.AreEqual(true, ws.IsEqual("001"));
                Assert.AreEqual(true, ws.IsEqual("002"));
                Assert.AreEqual(true, ws.IsEqual("010"));
                Assert.AreEqual(true, ws.IsEqual("011"));
                Assert.AreEqual(true, ws.IsEqual("012"));

                Assert.AreEqual(false, ws.IsEqual("030"));
                Assert.AreEqual(false, ws.IsEqual("014"));
                Assert.AreEqual(true, ws.IsEqual("112"));
            }

            {
                WordSearcher ws = new WordSearcher(new[] { new List<string> { "0", "4" }, new List<string> { "3", "5", "9" } });
                Assert.AreEqual(true, ws.IsEqual("03"));
                Assert.AreEqual(true, ws.IsEqual("05"));
                Assert.AreEqual(true, ws.IsEqual("09"));
                Assert.AreEqual(true, ws.IsEqual("43"));
                Assert.AreEqual(true, ws.IsEqual("45"));
                Assert.AreEqual(true, ws.IsEqual("49"));

                Assert.AreEqual(true, ws.IsEqual("30"));
                Assert.AreEqual(true, ws.IsEqual("50"));
                Assert.AreEqual(true, ws.IsEqual("90"));
                Assert.AreEqual(true, ws.IsEqual("34"));
                Assert.AreEqual(true, ws.IsEqual("54"));
                Assert.AreEqual(true, ws.IsEqual("94"));

                Assert.AreEqual(true, ws.IsEqual("99"));
                Assert.AreEqual(false, ws.IsEqual("88"));
                Assert.AreEqual(false, ws.IsEqual("74"));
            }

            {
                WordSearcher ws = new WordSearcher(new[] { new List<string> { "0", "5", "9", "7", "8" } });
                Assert.AreEqual(true, ws.IsEqual("0"));
                Assert.AreEqual(true, ws.IsEqual("5"));
                Assert.AreEqual(true, ws.IsEqual("9"));
                Assert.AreEqual(true, ws.IsEqual("8"));
                Assert.AreEqual(true, ws.IsEqual("7"));

                Assert.AreEqual(false, ws.IsEqual("4"));
                Assert.AreEqual(false, ws.IsEqual("3"));
                Assert.AreEqual(false, ws.IsEqual("2"));
            }
        }
    }
}