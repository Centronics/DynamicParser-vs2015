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
            WordSearcher ws = new WordSearcher(new[] { new List<string> { "a" }, new List<string> { "c", "d" }, new List<string> { "q", "w", "e" } });
            Assert.AreEqual(true, ws.IsEqual("acq"));
            Assert.AreEqual(true, ws.IsEqual("adq"));
            Assert.AreEqual(true, ws.IsEqual("acq"));
            Assert.AreEqual(true, ws.IsEqual("acw"));
            Assert.AreEqual(true, ws.IsEqual("ace"));
            Assert.AreEqual(true, ws.IsEqual("adq"));
            Assert.AreEqual(true, ws.IsEqual("adw"));
            Assert.AreEqual(true, ws.IsEqual("ade"));
            Assert.AreEqual(true, ws.IsEqual("qda"));
            Assert.AreEqual(true, ws.IsEqual("daq"));

            Assert.AreEqual(false, ws.IsEqual("akq"));
            Assert.AreEqual(false, ws.IsEqual("acr"));
            Assert.AreEqual(false, ws.IsEqual("qha"));
        }
    }
}