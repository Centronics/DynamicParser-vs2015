using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicParser;
using DynamicProcessor;
using Processor = DynamicParser.Processor;

namespace DynamicParserTest
{
    [TestClass]
    public class ProcessorContainerTest
    {
        [TestMethod]
        public void ProcessorContainerConsTest()
        {
            ProcessorContainer pc = new ProcessorContainer(
                new Processor(new[] { new SignValue(1), SignValue.MaxValue }, "Одномерный1"),
                new Processor(new[] { new SignValue(2), SignValue.MinValue }, "Одномерный2"),
                null, //                                                       Одномерный3
                new Processor(new[] { new SignValue(3), SignValue.MaxValue }, "Одномерный4"));
            Assert.AreEqual(3, pc.Count);
            Assert.AreEqual(2, pc.Width);
            Assert.AreEqual(1, pc.Height);
            Assert.AreEqual("Одномерный1", pc[0].Tag);
            Assert.AreEqual("Одномерный2", pc[1].Tag);
            Assert.AreEqual("Одномерный4", pc[2].Tag);
        }

        [TestMethod]
        public void ProcessorContainerAddRange1Test()
        {
            Processor proc1 = new Processor(new[] { new SignValue(70), SignValue.MaxValue, new SignValue(10), new SignValue(200), new SignValue(20) }, "Одномерный1");
            ProcessorContainer pc = new ProcessorContainer(proc1);
            Processor proc2 = new Processor(new[] { SignValue.MaxValue, new SignValue(17), new SignValue(202), new SignValue(25), new SignValue(98) }, "Одномерный2");
            Processor proc3 = new Processor(new[] { SignValue.MaxValue, new SignValue(18), new SignValue(200), new SignValue(30) }, "Одномерный3");
            bool res = false;
            try
            {
                pc.AddRange(proc2, null, proc3);
            }
            catch (ArgumentException)
            {
                res = true;
            }
            pc.AddRange(null);
            Assert.AreEqual(res, true);
            Assert.AreEqual(1, pc.Count);
            Assert.AreEqual("Одномерный1", pc[0].Tag);
            Assert.AreEqual(5, pc.Width);
            Assert.AreEqual(1, pc.Height);
        }

        [TestMethod]
        public void ProcessorContainerAddRange2Test()
        {
            Processor proc1 = new Processor(new[] { new SignValue(70), SignValue.MaxValue, new SignValue(10), new SignValue(200), new SignValue(20) }, "Одномерный1");
            ProcessorContainer pc = new ProcessorContainer(proc1);
            Processor proc2 = new Processor(new[] { SignValue.MaxValue, new SignValue(17), new SignValue(202), new SignValue(25), new SignValue(98) }, "Одномерный2");
            Processor proc3 = new Processor(new[] { SignValue.MaxValue, new SignValue(18), new SignValue(200), new SignValue(30), new SignValue(99) }, "Одномерный3");
            pc.AddRange(proc2, null, proc3);
            pc.AddRange(null);
            Assert.AreEqual(3, pc.Count);
            Assert.AreEqual("Одномерный1", pc[0].Tag);
            Assert.AreEqual("Одномерный2", pc[1].Tag);
            Assert.AreEqual("Одномерный3", pc[2].Tag);
            Assert.AreEqual(5, pc.Width);
            Assert.AreEqual(1, pc.Height);
        }

        [TestMethod]
        public void ProcessorContainerAddRange3Test()
        {
            Processor proc1 = new Processor(new[] { new SignValue(70), SignValue.MaxValue, new SignValue(10), new SignValue(200), new SignValue(20) }, "Одномерный1");
            ProcessorContainer pc = new ProcessorContainer(proc1);
            Processor proc2 = new Processor(new[] { SignValue.MaxValue, new SignValue(17), new SignValue(202), new SignValue(25) }, "Одномерный2");
            int res = 0;
            try
            {
                pc.Add(proc2);
            }
            catch (ArgumentException)
            {
                res = 1;
            }
            try
            {
                pc.Add(null);
            }
            catch (ArgumentNullException)
            {
                res = 2;
            }
            pc.AddRange(null);
            pc.Add(proc1);
            Assert.AreEqual(res, 2);
            Assert.AreEqual(2, pc.Count);
            Assert.AreEqual("Одномерный1", pc[0].Tag);
            Assert.AreEqual("Одномерный1", pc[1].Tag);
            Assert.AreEqual(5, pc.Width);
            Assert.AreEqual(1, pc.Height);
        }

        [TestMethod]
        public void ProcessorContainerAddRange4Test()
        {
            Processor proc1 = new Processor(new[] { new SignValue(70), SignValue.MaxValue, new SignValue(10), new SignValue(200), new SignValue(20) }, "Одномерный1");
            ProcessorContainer pc = new ProcessorContainer(proc1);
            Processor proc2 = new Processor(new[] { SignValue.MaxValue, new SignValue(17), new SignValue(202), new SignValue(25), new SignValue(98) }, "Одномерный2");
            Processor proc3 = new Processor(new[] { SignValue.MaxValue, new SignValue(18), new SignValue(200), new SignValue(30), new SignValue(99) }, "Одномерный3");
            IEnumerable<Processor> prcs = new[] { proc1, proc2, null, proc3 };
            pc.AddRange(prcs);
            pc.AddRange(null);
            Assert.AreEqual(3, pc.Count);
            Assert.AreEqual("Одномерный1", pc[0].Tag);
            Assert.AreEqual("Одномерный2", pc[1].Tag);
            Assert.AreEqual("Одномерный3", pc[2].Tag);
            Assert.AreEqual(5, pc.Width);
            Assert.AreEqual(1, pc.Height);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx2Test()
        {
            Processor proc1 = new Processor(new[] { new SignValue(70), SignValue.MaxValue, new SignValue(10), new SignValue(200), new SignValue(20) }, "Одномерный1");
            Processor proc2 = new Processor(new[] { SignValue.MaxValue, new SignValue(17), new SignValue(202), new SignValue(25) }, "Одномерный2");
            // ReSharper disable once UnusedVariable
            ProcessorContainer pc = new ProcessorContainer(proc1, proc2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx3Test()
        {
            // ReSharper disable once UnusedVariable
            ProcessorContainer pc = new ProcessorContainer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx4Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable");
            // ReSharper disable once UnusedVariable
            ProcessorContainer pc = new ProcessorContainer(null, p);
        }
    }
}