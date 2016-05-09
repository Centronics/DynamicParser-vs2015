using DynamicParser;
using DynamicProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DynamicParserTest
{
    [TestClass]
    public class DynamicLogicTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            DynamicAssigment da = new DynamicAssigment();
            Assert.AreEqual(null, da.Convert());
            da.ResearchList = null;
            Assert.AreEqual(null, da.Convert());
            da.ResearchList = new List<Map>();
            Assert.AreEqual(null, da.Convert());
            da.ResearchList = new List<Map>();
            da.ResearchList.Add(null);
            da.ResearchList.Add(new Map());
            Assert.AreEqual(0, da.Convert().Count);
            Map map = new Map();
            map.Add(new MapObject { Sign = new SignValue(100) });
            map.Add(new MapObject { Sign = new SignValue(200) });
            Map map1 = new Map();
            map1.Add(new MapObject { Sign = new SignValue(500) });
            map1.Add(new MapObject { Sign = new SignValue(600) });
            da.ResearchList.Add(map);
            da.ResearchList.Add(map1);
            Map retMap = da.Convert();
            Assert.AreEqual(40, retMap.Count);
            Assert.AreEqual(0, retMap.CountDiscounted);
            Assert.AreEqual(100, map[0].Sign.Value);
            Assert.AreEqual(200, map[1].Sign.Value);
            Assert.AreEqual(500, map1[0].Sign.Value);
            Assert.AreEqual(600, map1[1].Sign.Value);
        }

        [TestMethod]
        public void AssignTest()
        {
            DynamicAssigment da = new DynamicAssigment();
            da.ResearchList = null;
            Assert.AreEqual(-1, da.Assign(new Map()));
            da.ResearchList = new List<Map>();
            da.ResearchList.Add(null);
            Assert.AreEqual(-1, da.Assign(null));
            Map tstMap = new Map();
            tstMap.Add(new MapObject());
            da.ResearchList.Add(tstMap);
            Assert.AreEqual(-1, da.Assign(null));
            Map map1 = new Map();
            map1.Add(new MapObject { Sign = new SignValue(2000) });
            map1.Add(new MapObject { Sign = new SignValue(3000) });
            Map map2 = new Map();
            map2.Add(new MapObject { Sign = new SignValue(1000) });
            map2.Add(new MapObject { Sign = new SignValue(500) });
            Map map3 = new Map();
            map3.Add(new MapObject { Sign = new SignValue(5000) });
            map3.Add(new MapObject { Sign = new SignValue(7000) });
            da = new DynamicAssigment();
            da.ResearchList.Add(map1);
            da.ResearchList.Add(map2);
            da.ResearchList.Add(map3);
            Map tst1 = new Map();
            tst1.Add(new MapObject { Sign = new SignValue(2000) });
            tst1.Add(new MapObject { Sign = new SignValue(3000) });
            Map tst2 = new Map();
            tst2.Add(new MapObject { Sign = new SignValue(1000) });
            tst2.Add(new MapObject { Sign = new SignValue(500) });
            Map tst3 = new Map();
            tst3.Add(new MapObject { Sign = new SignValue(5000) });
            tst3.Add(new MapObject { Sign = new SignValue(7000) });
            Map tst4 = new Map();
            tst4.Add(new MapObject { Sign = new SignValue(2000) });
            tst4.Add(new MapObject { Sign = new SignValue(500) });
            Map tst5 = new Map();
            tst5.Add(new MapObject { Sign = new SignValue(9000) });
            tst5.Add(new MapObject { Sign = new SignValue(500) });
            Map tst6 = new Map();
            tst6.Add(new MapObject { Sign = new SignValue(300) });
            tst6.Add(new MapObject { Sign = new SignValue(500) });
            int ar = da.Assign(tst1);
            Assert.AreEqual(0, ar);
            ar = da.Assign(tst2);
            Assert.AreEqual(1, ar);
            ar = da.Assign(tst3);
            Assert.AreEqual(2, ar);
            ar = da.Assign(tst4);
            Assert.AreEqual(1, ar);
            ar = da.Assign(tst5);
            Assert.AreEqual(1, ar);
            ar = da.Assign(tst6);
            Assert.AreEqual(1, ar);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignTest1()
        {
            Map testMap = new Map();
            testMap.Add(new MapObject { Sign = new SignValue(1000) });
            DynamicAssigment da = new DynamicAssigment();
            da.ResearchList.Add(testMap);
            Map assignMap = new Map();
            assignMap.Add(new MapObject { Sign = new SignValue(2000) });
            assignMap.Add(new MapObject { Sign = new SignValue(3000) });
            da.Assign(assignMap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignTest2()
        {
            Map testMap = new Map();
            testMap.Add(new MapObject { Sign = new SignValue(1000) });
            Map testMap1 = new Map();
            testMap1.Add(new MapObject { Sign = new SignValue(1000) });
            testMap1.Add(new MapObject { Sign = new SignValue(5000) });
            DynamicAssigment da = new DynamicAssigment();
            da.ResearchList.Add(testMap);
            da.ResearchList.Add(testMap1);
            Map assignMap = new Map();
            assignMap.Add(new MapObject { Sign = new SignValue(2000) });
            assignMap.Add(new MapObject { Sign = new SignValue(3000) });
            da.Assign(assignMap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignTest3()
        {
            Map testMap = new Map();
            testMap.Add(new MapObject { Sign = new SignValue(1000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            Map testMap1 = new Map();
            testMap1.Add(new MapObject { Sign = new SignValue(1000) });
            testMap1.Add(new MapObject { Sign = new SignValue(5000) });
            testMap1.Add(new MapObject { Sign = new SignValue(5000) });
            DynamicAssigment da = new DynamicAssigment();
            da.ResearchList.Add(testMap);
            da.ResearchList.Add(testMap1);
            Map assignMap = new Map();
            assignMap.Add(new MapObject { Sign = new SignValue(2000) });
            assignMap.Add(new MapObject { Sign = new SignValue(3000) });
            da.Assign(assignMap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignTest4()
        {
            Map testMap = new Map();
            testMap.Add(new MapObject { Sign = new SignValue(1000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            DynamicAssigment da = new DynamicAssigment();
            da.ResearchList.Add(testMap);
            Map tstMap = new Map();
            tstMap.Add(new MapObject { Sign = new SignValue(1000) });
            da.Assign(tstMap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignTest5()
        {
            DynamicAssigment da = new DynamicAssigment();
            da.ResearchList.Add(new Map());
            da.ResearchList.Add(new Map());
            Map testMap = new Map();
            testMap.Add(new MapObject { Sign = new SignValue(1000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            da.Assign(testMap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignTest6()
        {
            DynamicAssigment da = new DynamicAssigment();
            Map testMap = new Map();
            testMap.Add(new MapObject { Sign = new SignValue(1000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            da.ResearchList.Add(testMap);
            da.ResearchList.Add(new Map());
            testMap = new Map();
            testMap.Add(new MapObject { Sign = new SignValue(1000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            da.Assign(testMap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignTest7()
        {
            DynamicAssigment da = new DynamicAssigment();
            da.ResearchList.Add(null);
            Map testMap = new Map();
            testMap.Add(new MapObject { Sign = new SignValue(1000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            testMap.Add(new MapObject { Sign = new SignValue(5000) });
            da.Assign(testMap);
        }
    }
}