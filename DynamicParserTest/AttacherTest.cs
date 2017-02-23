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
            Attach attach = new Attach
            {
                Regs = new List<Reg>
                {
                    new Reg {SelectedProcessor = pr1},
                    new Reg {SelectedProcessor = pr2},
                    new Reg {SelectedProcessor = pr3},
                    new Reg {SelectedProcessor = pr4}
                }
            };

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
            attacher.Add(new Point(0, 0));
            attacher.Add(1, 0);
            attacher.Add(0, 1);
            attacher.Add(1, 1);
            attacher.Add(0, 2);
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
            Assert.AreEqual(true, attacher.Contains(new Point(1, 0)));
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
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(0, 3, 2, 1)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(4, 6, 2, 2)));
            Assert.AreEqual(false, attacher.IsConflict(new Rectangle(1, 2, 1, 2)));
            Assert.AreEqual(true, attacher.IsConflict(new Rectangle(0, 1, 5, 5)));

            Region region1 = new Region(10, 10);
            region1.Add(new Rectangle(0, 0, 1, 1));
            region1.Add(new Rectangle(1, 3, 2, 3));
            region1.Add(new Rectangle(5, 4, 2, 1));
            region1.Add(new Rectangle(7, 8, 1, 1));
            region1.Add(new Rectangle(1, 9, 2, 1));
            region1.Add(new Rectangle(8, 1, 2, 1));

            Assert.AreEqual(false, attacher.IsConflict(region1));

            Region region2 = new Region(10, 10);
            region2.Add(9, 9, 1, 1);
            region2.Add(9, 0, 1, 3);
            region2.Add(7, 7, 2, 2);
            region2.Add(3, 9, 2, 1);
            region2.Add(2, 0, 2, 2);
            region2.Add(3, 4, 1, 2);
            region2.Add(2, 6, 2, 3);

            Assert.AreEqual(false, attacher.IsConflict(region2));

            Region region3 = new Region(10, 10);
            region3.Add(0, 0, 4, 2);

            Assert.AreEqual(true, attacher.IsConflict(region3));

            Region region4 = new Region(10, 10);
            region4.Add(1, 1, 2, 2);

            Assert.AreEqual(true, attacher.IsConflict(region4));

            Region region5 = new Region(10, 10);
            region5.Add(1, 0, 3, 3);

            Assert.AreEqual(true, attacher.IsConflict(region5));

            Region region6 = new Region(10, 10);
            region6.Add(0, 1, 2, 2);

            Assert.AreEqual(true, attacher.IsConflict(region6));

            Region region7 = new Region(10, 10);
            region7.Add(0, 1, 5, 4);

            Assert.AreEqual(true, attacher.IsConflict(region7));

            Region region8 = new Region(10, 10);
            region8.Add(1, 3, 1, 2);

            Assert.AreEqual(false, attacher.IsConflict(region8));

            Region region9 = new Region(10, 10);
            region9.Add(0, 0, 1, 1);

            Assert.AreEqual(false, attacher.IsConflict(region9));

            Region region10 = new Region(10, 10);
            region10.Add(1, 0, 1, 1);

            Assert.AreEqual(false, attacher.IsConflict(region10));

            Region region11 = new Region(10, 10);
            region11.Add(2, 0, 1, 1);

            Assert.AreEqual(false, attacher.IsConflict(region11));

            Region region12 = new Region(10, 10);
            region12.Add(0, 1, 1, 1);

            Assert.AreEqual(false, attacher.IsConflict(region12));

            Region region13 = new Region(10, 10);
            region13.Add(1, 1, 1, 1);

            Assert.AreEqual(false, attacher.IsConflict(region13));

            Region region14 = new Region(10, 10);
            region14.Add(2, 1, 1, 1);

            Assert.AreEqual(false, attacher.IsConflict(region14));

            Region region15 = new Region(10, 10);
            region15.Add(3, 1, 1, 1);

            Assert.AreEqual(false, attacher.IsConflict(region15));

            Region region16 = new Region(10, 10);
            region16.Add(0, 2, 1, 2);

            Assert.AreEqual(false, attacher.IsConflict(region16));

            Region region17 = new Region(10, 10);
            region17.Add(0, 2, 1, 5);

            Assert.AreEqual(false, attacher.IsConflict(region17));

            Region region18 = new Region(10, 10);
            region18.Add(0, 3, 4, 3);

            Assert.AreEqual(false, attacher.IsConflict(region18));

            Region region19 = new Region(10, 10);
            region19.Add(1, 3, 4, 3);

            Assert.AreEqual(false, attacher.IsConflict(region19));

            Region region20 = new Region(10, 10);
            region20.Add(2, 3, 4, 3);

            Assert.AreEqual(false, attacher.IsConflict(region20));

            Region region21 = new Region(10, 10);
            region21.Add(1, 2, 1, 2);

            Assert.AreEqual(false, attacher.IsConflict(region21));

            Region region22 = new Region(10, 10);
            region22.Add(0, 0, 2, 1);

            Assert.AreEqual(true, attacher.IsConflict(region22));

            Region region23 = new Region(10, 10);
            region23.Add(0, 1, 2, 1);

            Assert.AreEqual(true, attacher.IsConflict(region23));

            Region region24 = new Region(10, 10);
            region24.Add(0, 2, 2, 1);

            Assert.AreEqual(true, attacher.IsConflict(region24));

            Region region25 = new Region(10, 10);
            region25.Add(0, 0, 1, 3);

            Assert.AreEqual(true, attacher.IsConflict(region25));

            Region region26 = new Region(10, 10);
            region26.Add(1, 0, 1, 3);

            Assert.AreEqual(true, attacher.IsConflict(region26));

            Region region27 = new Region(10, 10);
            region27.Add(0, 0, 1, 2);

            Assert.AreEqual(true, attacher.IsConflict(region27));

            Region region28 = new Region(10, 10);
            region28.Add(1, 0, 1, 2);

            Assert.AreEqual(true, attacher.IsConflict(region28));

            Region region29 = new Region(10, 10);
            region29.Add(0, 0, 1, 2);

            Assert.AreEqual(true, attacher.IsConflict(region29));

            Region region30 = new Region(10, 10);
            region30.Add(0, 0, 2, 1);

            Assert.AreEqual(true, attacher.IsConflict(region30));

            Region region31 = new Region(10, 10);
            region31.Add(1, 1, 6, 1);

            Assert.AreEqual(false, attacher.IsConflict(region31));

            Region region32 = new Region(10, 10);
            region32.Add(1, 2, 1, 5);

            Assert.AreEqual(false, attacher.IsConflict(region32));
        }

        [TestMethod]
        public void AttacherIsConflictTest()
        {
            Assert.AreEqual(true, Attacher.InRectangle(new Point(1, 1), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(true, Attacher.InRectangle(new Point(0, 1), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(true, Attacher.InRectangle(new Point(1, 0), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(1, 1), new Rectangle(2, 2, 2, 2)));
            Assert.AreEqual(true, Attacher.InRectangle(new Point(1, 1), new Rectangle(1, 1, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(1, 1), new Rectangle(0, 0, 0, 0)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(1, 3), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(2, 2), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(3, 2), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(3, 3), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(2, 5), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(5, 2), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(4, 4), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(3, 1), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(2, 0), new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, Attacher.InRectangle(new Point(0, 2), new Rectangle(0, 0, 2, 2)));
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
            attacher.Add(0, 0);
            attacher.Add(2, 2);
            attacher.Add(5, 5);
            Registered nn = region[0, 0];
            Registered nb = region[2, 2];
            Registered np = region[5, 5];
            Assert.AreEqual(true, nn.IsEmpty);
            Assert.AreEqual(true, nb.IsEmpty);
            Assert.AreNotEqual(null, nn);
            Assert.AreNotEqual(null, nb);
            Assert.AreEqual(null, np);
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