using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DynamicParser;
using DynamicProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Processor = DynamicParser.Processor;
using Region = DynamicParser.Region;

namespace DynamicParserTest
{
    [TestClass]
    public class AttacherTest
    {
        [TestMethod]
        public void AttachTest()
        {
            Assert.AreEqual(true, Attach.TagStringCompare("a", "a"));
            Assert.AreEqual(true, Attach.TagStringCompare("A", "a"));
            Assert.AreEqual(true, Attach.TagStringCompare("a", "A"));
            Assert.AreEqual(true, Attach.TagStringCompare(" a", "a"));
            Assert.AreEqual(true, Attach.TagStringCompare("a", " a"));
            Assert.AreEqual(true, Attach.TagStringCompare(" a", " A   "));
            Assert.AreEqual(true, Attach.TagStringCompare("   A", " a  "));
            Assert.AreEqual(true, Attach.TagStringCompare("A", "A"));
            Assert.AreEqual(true, Attach.TagStringCompare("  A   ", "  A  "));

            Processor pr1 = new Processor(new[] { SignValue.MaxValue }, " dd"),
                pr2 = new Processor(new[] { SignValue.MaxValue }, " DD  "),
                pr3 = new Processor(new[] { SignValue.MaxValue }, "dD"),
                pr4 = new Processor(new[] { SignValue.MaxValue }, " nn");
            Attach attach = new Attach();
            attach.Regs.Add(new Reg { Procs = new[] { pr1 } });
            attach.Regs.Add(new Reg { Procs = new[] { pr2 } });
            attach.Regs.Add(new Reg { Procs = new[] { pr3 } });
            attach.Regs.Add(new Reg { Procs = new[] { pr4 } });

            List<Processor> lst = new List<Processor>(attach.Processors);
            Assert.AreEqual(true, lst.Contains(pr1));
            Assert.AreEqual(true, lst.Contains(pr2));
            Assert.AreEqual(true, lst.Contains(pr3));
            Assert.AreEqual(true, lst.Contains(pr4));

            Attach.Proc proc = attach.Unique;
            Assert.AreEqual(true, proc.Procs.Contains(pr1));
            Assert.AreEqual(true, proc.Procs.Contains(pr4));
        }

        [TestMethod]
        public void AttacherTest1()
        {
            Attacher attacher = new Attacher(2, 3);
            Assert.AreEqual(2, attacher.Width);
            Assert.AreEqual(3, attacher.Height);
            attacher.Add(new Point(1, 1));
            attacher.Add(1, 2);
            bool ex1 = false, ex2 = false;
            foreach (Attach at in attacher.Attaches)
            {
                if (at.Point == new Point(1, 1))
                    ex1 = true;
                if (at.Point == new Point(1, 2))
                    ex2 = true;
            }
            Assert.AreEqual(true, ex1);
            Assert.AreEqual(true, ex2);
            ex1 = false; ex2 = false;
            foreach (Point at in attacher.Points)
            {
                if (at == new Point(1, 1))
                    ex1 = true;
                if (at == new Point(1, 2))
                    ex2 = true;
            }
            Assert.AreEqual(true, ex1);
            Assert.AreEqual(true, ex2);
            Assert.AreEqual(true, attacher.Contains(new Point(1, 1)));
            Assert.AreEqual(true, attacher.Contains(new Point(1, 2)));
            Assert.AreEqual(false, attacher.Contains(new Point(1, 0)));
            Assert.AreEqual(false, attacher.Contains(new Point(0, 10)));

            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(0, 0, 1, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(1, 1, 1, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(1, 2, 1, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(2, 3, 15, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(1, 1, 5, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(1, 1, 1, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(1, 2, 12, 10)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(7, 8, 1, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(1, 0, 2, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(0, 1, 1, 1)));
            Assert.AreEqual(true, attacher.IsConflict(new Rectangle(1, 1, 1, 2)));
            Assert.AreEqual(true, attacher.IsConflict(new Rectangle(0, 0, 3, 3)));
            Assert.AreEqual(true, attacher.IsConflict(new Rectangle(1, 1, 2, 2)));
            Assert.AreEqual(true, attacher.IsConflict(new Rectangle(0, 2, 2, 1)));
            Assert.AreEqual(true, attacher.IsConflict(new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(true, attacher.IsConflict(new Rectangle(1, 0, 1, 2)));
            Assert.AreEqual(true, attacher.IsConflict(new Rectangle(0, 1, 5, 5)));

            Region region1 = new Region(10, 10);
            region1.Add(new Rectangle(0, 0, 1, 1));
            region1.Add(new Rectangle(1, 1, 1, 1));
            region1.Add(new Rectangle(1, 2, 1, 1));
            region1.Add(new Rectangle(2, 3, 15, 1));
            region1.Add(new Rectangle(1, 1, 5, 1));
            region1.Add(new Rectangle(1, 1, 1, 1));
            region1.Add(new Rectangle(1, 2, 12, 10));
            region1.Add(new Rectangle(7, 8, 1, 1));
            region1.Add(new Rectangle(1, 0, 2, 1));
            region1.Add(new Rectangle(0, 1, 1, 1));

            Assert.AreEqual(false, attacher.IsConflict(region1));

            Region region2 = new Region(10, 10);
            region2.Add(new Rectangle(1, 1, 1, 2));
            region2.Add(new Rectangle(0, 0, 3, 3));
            region2.Add(new Rectangle(1, 1, 2, 2));
            region2.Add(new Rectangle(0, 2, 2, 1));
            region2.Add(new Rectangle(0, 0, 2, 2));
            region2.Add(new Rectangle(1, 0, 1, 2));
            region2.Add(new Rectangle(0, 1, 5, 5));

            Assert.AreEqual(true, attacher.IsConflict(region2));
        }

        [TestMethod]
        public void AttacherIsConflictTest()
        {
            Assert.AreEqual(true, Attacher.IsConflict(new Point(1, 1), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(true, Attacher.IsConflict(new Point(0, 1), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(true, Attacher.IsConflict(new Point(1, 0), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(true, Attacher.IsConflict(new Point(1, 1), new Rectangle(2, 2, 2, 2)));
            Assert.AreEqual(true, Attacher.IsConflict(new Point(1, 1), new Rectangle(1, 1, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(1, 1), new Rectangle(0, 0, 0, 0)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(1, 3), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(2, 2), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(3, 2), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(3, 3), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(2, 5), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(5, 2), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(4, 4), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(3, 1), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(2, 0), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.IsConflict(new Point(0, 2), new Rectangle(0, 0, 2, 2)));
        }

        [TestMethod]
        public void AttacherSetMaskTest()
        {
            Region region = new Region(10, 10);
            region.Add(2, 2, 3, 3);
            region.Add(0, 0, 1, 1);
            region.Add(9, 9, 1, 1);
            region.Add(9, 6, 1, 2);
            Attacher attacher = new Attacher(10, 10);
            attacher.Add(5, 5);
            attacher.Add(0, 0);
            attacher.Add(2, 2);
            attacher.SetMask(region);
            List<Attach> lst = attacher.Attaches.ToList();
            bool b1 = false, b2 = false, b3 = false;
            foreach (Attach att in lst)
            {
                if (att.Point == new Point(0, 0))
                    b1 = true;
                if (att.Point == new Point(2, 2))
                    b2 = true;
                if (att.Point == new Point(5, 5))
                    b3 = true;
            }
            Assert.AreEqual(true, b1 && b2 && !b3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AttacherSetMaskEx1Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(2, 2).SetMask(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherSetMaskEx2Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(2, 2).SetMask(new Region(2, 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherSetMaskEx3Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(2, 2).SetMask(new Region(1, 2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherSetMaskEx4Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(2, 2).SetMask(new Region(5, 2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherSetMaskEx5Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(2, 2).SetMask(new Region(1, 7));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx1Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(0, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx2Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(2, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx3Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(0, -2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx4Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(-10, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx5Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Attacher(2, -2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx6Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(2, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx7Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx8Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(5, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx9Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(1, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx10Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(new Point(2, 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx11Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(new Point(1, 2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx12Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(new Point(5, 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx13Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(new Point(1, 5));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx14Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(1, 1);
            attacher.Add(1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx15Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(-1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx16Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(1, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AttacherEx17Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Attacher attacher = new Attacher(2, 2);
            attacher.Add(-1, -1);
        }
    }
}