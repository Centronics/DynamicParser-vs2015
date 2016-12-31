using System;
using System.Collections.Generic;
using System.Linq;
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
            TagSearcher ts = new TagSearcher("AbCd");
            {
                List<FindString> lst1 = new List<FindString>(ts.IsEqual("abcd"));
                Assert.AreEqual(1, lst1.Count);
                Assert.AreEqual(0, lst1[0].Position);
                Assert.AreEqual(true, lst1[0].GetStringEqual("abdc"));
                Assert.AreEqual(true, lst1[0].GetStringEqual("cbac"));
                Assert.AreEqual(true, lst1[0].GetStringEqual("dabc"));
                Assert.AreEqual(true, lst1[0].GetStringEqual("bacd"));
                Assert.AreEqual(false, lst1[0].GetStringEqual(""));
                Assert.AreEqual(false, lst1[0].GetStringEqual("a"));
                Assert.AreEqual(false, lst1[0].GetStringEqual("abcdf"));
                Assert.AreEqual(false, lst1[0].GetStringEqual(null));
                Assert.AreEqual("ABCD", lst1[0].CurrentString);
            }
            {
                List<FindString> lst2 = new List<FindString>(ts.IsEqual("Abcd"));
                Assert.AreEqual(1, lst2.Count);
                Assert.AreEqual(0, lst2[0].Position);
                Assert.AreEqual(true, lst2[0].GetStringEqual("abdc"));
                Assert.AreEqual(true, lst2[0].GetStringEqual("cbac"));
                Assert.AreEqual(true, lst2[0].GetStringEqual("dabc"));
                Assert.AreEqual(true, lst2[0].GetStringEqual("bacd"));
                Assert.AreEqual(false, lst2[0].GetStringEqual(""));
                Assert.AreEqual(false, lst2[0].GetStringEqual("a"));
                Assert.AreEqual(false, lst2[0].GetStringEqual("abcdf"));
                Assert.AreEqual(false, lst2[0].GetStringEqual(null));
                Assert.AreEqual("ABCD", lst2[0].CurrentString);
            }
            {
                List<FindString> lst3 = new List<FindString>(ts.IsEqual("abCd"));
                Assert.AreEqual(1, lst3.Count);
                Assert.AreEqual(0, lst3[0].Position);
                Assert.AreEqual(true, lst3[0].GetStringEqual("acdb"));
                Assert.AreEqual(true, lst3[0].GetStringEqual("cbac"));
                Assert.AreEqual(true, lst3[0].GetStringEqual("dcba"));
                Assert.AreEqual(true, lst3[0].GetStringEqual("dcba"));
                Assert.AreEqual(false, lst3[0].GetStringEqual(""));
                Assert.AreEqual(false, lst3[0].GetStringEqual("a"));
                Assert.AreEqual(false, lst3[0].GetStringEqual("abcdf"));
                Assert.AreEqual(false, lst3[0].GetStringEqual(null));
                Assert.AreEqual("ABCD", lst3[0].CurrentString);
            }
            {
                List<FindString> lst4 = new List<FindString>(ts.IsEqual("abcD"));
                Assert.AreEqual(1, lst4.Count);
                Assert.AreEqual(0, lst4[0].Position);
                Assert.AreEqual(true, lst4[0].GetStringEqual("bcad"));
                Assert.AreEqual(true, lst4[0].GetStringEqual("cabd"));
                Assert.AreEqual(true, lst4[0].GetStringEqual("bdca"));
                Assert.AreEqual(true, lst4[0].GetStringEqual("dacb"));
                Assert.AreEqual(false, lst4[0].GetStringEqual(""));
                Assert.AreEqual(false, lst4[0].GetStringEqual("a"));
                Assert.AreEqual(false, lst4[0].GetStringEqual("abcdf"));
                Assert.AreEqual(false, lst4[0].GetStringEqual(null));
                Assert.AreEqual("ABCD", lst4[0].CurrentString);
            }
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TagSearcherTest4()
        {
            TagSearcher ts = new TagSearcher("ds");
            // ReSharper disable once UnusedVariable
            List<FindString> lst = new List<FindString>(ts.IsEqual(string.Empty));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TagSearcherTest5()
        {
            TagSearcher ts = new TagSearcher("fg");
            // ReSharper disable once UnusedVariable
            List<FindString> lst = new List<FindString>(ts.IsEqual(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TagSearcherTest6()
        {
            Assert.AreEqual(0, new TagSearcher("AbCd").IsEqual("abcdd").Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TagSearcherTest7()
        {
            Assert.AreEqual(0, new TagSearcher("AbCd").IsEqual("abc").Count());
        }
    }
}