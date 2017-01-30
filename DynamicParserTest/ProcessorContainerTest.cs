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
                pc.AddRange(proc2, proc3);
            }
            catch (ArgumentException)
            {
                res = true;
            }
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
                res++;
            }
            try
            {
                pc.Add(null);
            }
            catch (ArgumentNullException)
            {
                res++;
            }
            Assert.AreEqual(res, 2);
            Assert.AreEqual(1, pc.Count);
            Assert.AreEqual("Одномерный1", pc[0].Tag);
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
            IList<Processor> prcs = new[] { proc2, null, proc3 };
            pc.AddRange(prcs);
            Assert.AreEqual(3, pc.Count);
            Assert.AreEqual("Одномерный1", pc[0].Tag);
            Assert.AreEqual("Одномерный2", pc[1].Tag);
            Assert.AreEqual("Одномерный3", pc[2].Tag);
            Assert.AreEqual(5, pc.Width);
            Assert.AreEqual(1, pc.Height);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx1Test()
        {
            Processor proc1 = new Processor(new[] { new SignValue(70), SignValue.MaxValue, new SignValue(10), new SignValue(200), new SignValue(20) }, "Одномерный1");
            Processor proc2 = new Processor(new[] { SignValue.MaxValue, new SignValue(17), new SignValue(202), new SignValue(25) }, "Одномерный2");
            // ReSharper disable once UnusedVariable
            ProcessorContainer pc = new ProcessorContainer(proc1, proc2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx2Test()
        {
            // ReSharper disable once UnusedVariable
            ProcessorContainer pc = new ProcessorContainer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx3Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable");
            // ReSharper disable once UnusedVariable
            ProcessorContainer pc = new ProcessorContainer(null, p);
        }

        [TestMethod]
        public void ProcessorContainerEx4Test()
        {
            SignValue[,] pr1 = new SignValue[2, 3];
            pr1[0, 0] = SignValue.MaxValue;
            pr1[1, 0] = SignValue.MinValue;

            pr1[0, 1] = SignValue.MaxValue;
            pr1[1, 1] = SignValue.MinValue;

            pr1[0, 2] = SignValue.MaxValue;
            pr1[1, 2] = SignValue.MinValue;

            SignValue[,] pr2 = new SignValue[2, 3];
            pr2[0, 0] = new SignValue(10);
            pr2[1, 0] = new SignValue(109);

            pr2[0, 1] = new SignValue(14);
            pr2[1, 1] = SignValue.MinValue;

            pr2[0, 2] = new SignValue(20);
            pr2[1, 2] = new SignValue(90);

            SignValue[,] pr3 = new SignValue[3, 3];
            pr3[0, 0] = new SignValue(10);
            pr3[1, 0] = new SignValue(109);
            pr3[2, 0] = new SignValue(14);

            pr3[0, 1] = new SignValue(14);
            pr3[1, 1] = SignValue.MinValue;
            pr3[2, 1] = new SignValue(15);

            pr3[0, 2] = new SignValue(20);
            pr3[1, 2] = new SignValue(90);
            pr3[2, 2] = new SignValue(25);

            Processor proc1 = new Processor(pr1, "Двумерный1");
            ProcessorContainer pc = new ProcessorContainer(proc1);
            Processor proc2 = new Processor(pr2, "Двумерный2");
            Processor proc3 = new Processor(pr3, "Двумерный3");
            int count = 0;
            try
            {
                IList<Processor> prcs = new[] { proc1, proc2, null, proc3 };
                pc.AddRange(prcs);
            }
            catch (ArgumentException)
            {
                count++;
            }
            try
            {
                pc.Add(proc1);
            }
            catch (ArgumentException)
            {
                count++;
            }
            proc1 = new Processor(pr1, "Двумерный1");
            proc2 = new Processor(pr2, "Двумерный2");
            proc3 = new Processor(pr3, "Двумерный3");
            try
            {
                pc.AddRange(proc1, proc2, null, proc3);
            }
            catch (ArgumentException)
            {
                count++;
            }
            pc.Add(proc2);
            Assert.AreEqual(3, count);
            Assert.AreEqual(2, pc.Count);
            Assert.AreEqual("Двумерный1", pc[0].Tag);
            Assert.AreEqual("Двумерный2", pc[1].Tag);
            Assert.AreEqual(2, pc.Width);
            Assert.AreEqual(3, pc.Height);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx5Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " Unusedvariable ");
            // ReSharper disable once UnusedVariable
            ProcessorContainer pc = new ProcessorContainer(null, p, p1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx6Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable");
            // ReSharper disable once UnusedVariable
            ProcessorContainer pc = new ProcessorContainer(p, p);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx7Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(p, p1);
            pc.Add(p1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx8Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(p, p1);
            pc.Add(new Processor(new[] { SignValue.MaxValue }, " unusedvAriable1 "));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx9Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(p, p1);
            pc.AddRange(new Processor(new[] { SignValue.MaxValue }, " unusedvAriable1 "));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx10Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(null, p, p1);
            pc.AddRange(new Processor(new[] { SignValue.MaxValue }, " unusedvAriable "),
                new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 "));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx11Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(null, p, p1);
            Processor[] procs =
            {
                new Processor(new[] {SignValue.MaxValue}, " UnusedVariable "),
                new Processor(new[] {SignValue.MaxValue}, " UnusedVariable4 ")
            };
            pc.AddRange(procs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx12Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(p, p1);
            IList<Processor> procs = new[]
            {
                new Processor(new[] {SignValue.MaxValue}, " UnusedVariable "),
                new Processor(new[] {SignValue.MaxValue}, " UnusedVariable4 ")
            };
            pc.AddRange(procs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx13Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(p, p1);
            Processor[] procs =
            {
                p,
                new Processor(new[] {SignValue.MaxValue}, " UnusedVariable4 ")
            };
            pc.AddRange(procs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx14Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(p, p1);
            Processor[] procs =
            {
                new Processor(new[] {SignValue.MaxValue}, " UnusedVariable "),
                p1
            };
            pc.AddRange(procs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx15Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(p, p1);
            IList<Processor> procs = new[]
            {
                p,
                new Processor(new[] {SignValue.MaxValue}, " UnusedVariable4 ")
            };
            pc.AddRange(procs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx16Test()
        {
            Processor p = new Processor(new[] { SignValue.MaxValue }, "UnusedVariable"),
                p1 = new Processor(new[] { SignValue.MaxValue }, " UnusedVariable1 ");
            ProcessorContainer pc = new ProcessorContainer(p, p1);
            IList<Processor> procs = new[]
            {
                new Processor(new[] {SignValue.MaxValue}, " UnusedVariable "),
                p1
            };
            pc.AddRange(procs);
        }

        [TestMethod]
        public void ProcessorContainerInOneSizeTestIsEquals()
        {
            SignValue[,] pr1 = new SignValue[2, 3];
            pr1[0, 0] = SignValue.MaxValue;
            pr1[1, 0] = SignValue.MinValue;

            pr1[0, 1] = SignValue.MaxValue;
            pr1[1, 1] = SignValue.MinValue;

            pr1[0, 2] = SignValue.MaxValue;
            pr1[1, 2] = SignValue.MinValue;

            SignValue[,] pr2 = new SignValue[2, 3];
            pr2[0, 0] = new SignValue(10);
            pr2[1, 0] = new SignValue(109);

            pr2[0, 1] = new SignValue(14);
            pr2[1, 1] = SignValue.MinValue;

            pr2[0, 2] = new SignValue(20);
            pr2[1, 2] = new SignValue(90);

            SignValue[,] pr3 = new SignValue[3, 3];
            pr3[0, 0] = new SignValue(10);
            pr3[1, 0] = new SignValue(109);
            pr3[2, 0] = new SignValue(14);

            pr3[0, 1] = new SignValue(14);
            pr3[1, 1] = SignValue.MinValue;
            pr3[2, 1] = new SignValue(15);

            pr3[0, 2] = new SignValue(20);
            pr3[1, 2] = new SignValue(90);
            pr3[2, 2] = new SignValue(25);

            Processor proc1 = new Processor(pr1, "pr1");
            Processor proc2 = new Processor(pr2, "pr2");
            Processor proc3 = new Processor(pr3, "pr3");

            Assert.AreEqual(false, ProcessorContainer.InOneSize(new[] { proc1, proc2, proc3 }));
            Assert.AreEqual(true, ProcessorContainer.InOneSize(new[] { proc1, proc2 }));
            Assert.AreEqual(false, ProcessorContainer.InOneSize(new[] { proc1, proc2 }));
            Assert.AreEqual(false, ProcessorContainer.InOneSize(null));
            Assert.AreEqual(false, ProcessorContainer.InOneSize(new[] { proc1, proc1, proc2, proc3 }));
            Assert.AreEqual(true, ProcessorContainer.InOneSize(new[] { proc1 }));
            Assert.AreEqual(false, ProcessorContainer.InOneSize(new[] { proc1 }));
            Assert.AreEqual(false, ProcessorContainer.InOneSize(new[] { proc1 }));

            Assert.AreEqual(false, ProcessorContainer.IsEquals(new[] { proc1, proc2 }));
            Assert.AreEqual(true, ProcessorContainer.IsEquals(new[] { proc1, proc1 }));
            Assert.AreEqual(false, ProcessorContainer.IsEquals(null));
            Assert.AreEqual(true, ProcessorContainer.IsEquals(new[] { proc3, proc2, proc3, proc2 }));
        }

        [TestMethod]
        public void ProcessorContainerContainsTagInOneTag()
        {
            SignValue[,] pr1 = new SignValue[2, 3];
            pr1[0, 0] = SignValue.MaxValue;
            pr1[1, 0] = SignValue.MinValue;

            pr1[0, 1] = SignValue.MaxValue;
            pr1[1, 1] = SignValue.MinValue;

            pr1[0, 2] = SignValue.MaxValue;
            pr1[1, 2] = SignValue.MinValue;

            SignValue[,] pr2 = new SignValue[2, 3];
            pr2[0, 0] = new SignValue(10);
            pr2[1, 0] = new SignValue(109);

            pr2[0, 1] = new SignValue(14);
            pr2[1, 1] = SignValue.MinValue;

            pr2[0, 2] = new SignValue(20);
            pr2[1, 2] = new SignValue(90);

            SignValue[,] pr3 = new SignValue[3, 3];
            pr3[0, 0] = new SignValue(10);
            pr3[1, 0] = new SignValue(109);
            pr3[2, 0] = new SignValue(14);

            pr3[0, 1] = new SignValue(14);
            pr3[1, 1] = SignValue.MinValue;
            pr3[2, 1] = new SignValue(15);

            pr3[0, 2] = new SignValue(20);
            pr3[1, 2] = new SignValue(90);
            pr3[2, 2] = new SignValue(25);

            Processor proc1 = new Processor(pr1, "pr1");
            Processor proc2 = new Processor(pr2, "pr2");

            ProcessorContainer pc = new ProcessorContainer(proc1, proc2);
            Assert.AreEqual(true, pc.ContainsTag("pr1"));
            Assert.AreEqual(true, pc.ContainsTag("pr1 "));
            Assert.AreEqual(true, pc.ContainsTag("pr2"));
            Assert.AreEqual(true, pc.ContainsTag(" pr2"));
            Assert.AreEqual(true, pc.ContainsTag("pR2"));
            Assert.AreEqual(false, pc.ContainsTag("pr3"));
            Assert.AreEqual(false, pc.ContainsTag(" Pr3 "));
            Assert.AreEqual(false, pc.ContainsTag("     "));
            Assert.AreEqual(false, pc.ContainsTag(string.Empty));

            Assert.AreEqual(true, ProcessorContainer.InOneTag(null));
            Assert.AreEqual(false, ProcessorContainer.InOneTag(new[] { proc1, proc1, proc2 }));
            proc2 = new Processor(pr2, " pr1");
            Processor proc3 = new Processor(pr3, "pr3");
            Assert.AreEqual(false, ProcessorContainer.InOneTag(new[] { proc1, proc2, proc3 }));
            proc1 = new Processor(pr2, " pR1 ");
            Assert.AreEqual(false, ProcessorContainer.InOneTag(new[] { proc1, proc2, proc3 }));
            Assert.AreEqual(true, ProcessorContainer.InOneTag(new[] { proc1 }));
            proc2 = new Processor(pr2, " pt1");
            Assert.AreEqual(true, ProcessorContainer.InOneTag(new[] { proc1, proc2, proc3 }));
        }
    }
}