using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicParser;
using Region = DynamicParser.Region;

namespace DynamicParserTest
{
    [TestClass]
    public class RegionTest
    {
        [TestMethod]
        public void RegisteredTest1()
        {
            Registered reg = new Registered { Region = new Rectangle(11, 12, 13, 14) };
            Assert.AreEqual(11, reg.X);
            Assert.AreEqual(12, reg.Y);
            Assert.AreEqual(24, reg.Right);
            Assert.AreEqual(26, reg.Bottom);
            Assert.AreEqual(true, reg.IsEmpty);
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(27, 28, 10, 10)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(3, 3, 5, 4)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(3, 3, 8, 8)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(3, 3, 7, 8)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(3, 3, 8, 7)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(3, 3, 8, 8)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(3, 3, 8, 8)));
            Assert.AreEqual(true, reg.IsConflict(new Rectangle(24, 10, 20, 8)));
            Assert.AreEqual(true, reg.IsConflict(new Rectangle(27, 26, 1, 1)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(24, 26, 0, 0)));
            Assert.AreEqual(true, reg.IsConflict(new Rectangle(11, 12, 20, 8)));
            Assert.AreEqual(true, reg.IsConflict(new Rectangle(11, 12, 1, 1)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(11, 12, 0, 0)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(30, 10, 20, 8)));
            Assert.AreEqual(true, reg.IsConflict(new Rectangle(5, 20, 1, 10)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(27, 28, 0, 0)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(12, 13, 5, 5)));
        }

        [TestMethod]
        public void RegisteredTest2()
        {
            Region region = new Region(5, 5);
            region.Add(0, 0, 2, 2);
            region.Add(2, 2, 1, 1);
            region.Add(3, 3, 1, 1);
            Assert.AreEqual(0, region[0, 0].X);
            Assert.AreEqual(0, region[0, 0].Y);
            Assert.AreEqual(2, region[0, 0].Right);
            Assert.AreEqual(2, region[0, 0].Bottom);
            Assert.AreEqual(0, region[0, 1].X);
            Assert.AreEqual(0, region[0, 1].Y);
            Assert.AreEqual(2, region[0, 1].Right);
            Assert.AreEqual(2, region[0, 1].Bottom);
            Assert.AreEqual(0, region[1, 0].X);
            Assert.AreEqual(0, region[1, 0].Y);
            Assert.AreEqual(2, region[1, 0].Right);
            Assert.AreEqual(2, region[1, 0].Bottom);
            Assert.AreEqual(0, region[1, 1].X);
            Assert.AreEqual(0, region[1, 1].Y);
            Assert.AreEqual(2, region[1, 1].Right);
            Assert.AreEqual(2, region[1, 1].Bottom);
            Assert.AreEqual(2, region[2, 2].X);
            Assert.AreEqual(2, region[2, 2].Y);
            Assert.AreEqual(3, region[2, 2].Right);
            Assert.AreEqual(3, region[2, 2].Bottom);
            Assert.AreEqual(2, region[3, 3].X);
            Assert.AreEqual(2, region[3, 3].Y);
            Assert.AreEqual(4, region[3, 3].Right);
            Assert.AreEqual(4, region[3, 3].Bottom);
            Assert.AreEqual(null, region[0, 2]);
            Assert.AreEqual(null, region[0, 3]);
            Assert.AreEqual(null, region[0, 4]);
            Assert.AreEqual(null, region[1, 2]);
            Assert.AreEqual(null, region[1, 3]);
            Assert.AreEqual(null, region[1, 4]);
            Assert.AreEqual(null, region[2, 0]);
            Assert.AreEqual(null, region[2, 1]);
            Assert.AreEqual(null, region[2, 3]);
            Assert.AreEqual(null, region[2, 4]);
            Assert.AreEqual(null, region[3, 0]);
            Assert.AreEqual(null, region[3, 1]);
            Assert.AreEqual(null, region[3, 2]);
            Assert.AreEqual(null, region[3, 4]);
            Assert.AreEqual(null, region[4, 0]);
            Assert.AreEqual(null, region[4, 1]);
            Assert.AreEqual(null, region[4, 2]);
            Assert.AreEqual(null, region[4, 3]);
            Assert.AreEqual(null, region[4, 4]);

            Assert.AreEqual(0, region[new Point(0, 0)].X);
            Assert.AreEqual(0, region[new Point(0, 0)].Y);
            Assert.AreEqual(2, region[new Point(0, 0)].Right);
            Assert.AreEqual(2, region[new Point(0, 0)].Bottom);
            Assert.AreEqual(0, region[new Point(0, 1)].X);
            Assert.AreEqual(0, region[new Point(0, 1)].Y);
            Assert.AreEqual(2, region[new Point(0, 1)].Right);
            Assert.AreEqual(2, region[new Point(0, 1)].Bottom);
            Assert.AreEqual(0, region[new Point(1, 0)].X);
            Assert.AreEqual(0, region[new Point(1, 0)].Y);
            Assert.AreEqual(2, region[new Point(1, 0)].Right);
            Assert.AreEqual(2, region[new Point(1, 0)].Bottom);
            Assert.AreEqual(0, region[new Point(1, 1)].X);
            Assert.AreEqual(0, region[new Point(1, 1)].Y);
            Assert.AreEqual(2, region[new Point(1, 1)].Right);
            Assert.AreEqual(2, region[new Point(1, 1)].Bottom);
            Assert.AreEqual(2, region[new Point(2, 2)].X);
            Assert.AreEqual(2, region[new Point(2, 2)].Y);
            Assert.AreEqual(3, region[new Point(2, 2)].Right);
            Assert.AreEqual(3, region[new Point(2, 2)].Bottom);
            Assert.AreEqual(2, region[new Point(3, 3)].X);
            Assert.AreEqual(2, region[new Point(3, 3)].Y);
            Assert.AreEqual(4, region[new Point(3, 3)].Right);
            Assert.AreEqual(4, region[new Point(3, 3)].Bottom);
            Assert.AreEqual(null, region[new Point(0, 2)]);
            Assert.AreEqual(null, region[new Point(0, 3)]);
            Assert.AreEqual(null, region[new Point(0, 4)]);
            Assert.AreEqual(null, region[new Point(1, 2)]);
            Assert.AreEqual(null, region[new Point(1, 3)]);
            Assert.AreEqual(null, region[new Point(1, 4)]);
            Assert.AreEqual(null, region[new Point(2, 0)]);
            Assert.AreEqual(null, region[new Point(2, 1)]);
            Assert.AreEqual(null, region[new Point(2, 3)]);
            Assert.AreEqual(null, region[new Point(2, 4)]);
            Assert.AreEqual(null, region[new Point(3, 0)]);
            Assert.AreEqual(null, region[new Point(3, 1)]);
            Assert.AreEqual(null, region[new Point(3, 2)]);
            Assert.AreEqual(null, region[new Point(3, 4)]);
            Assert.AreEqual(null, region[new Point(4, 0)]);
            Assert.AreEqual(null, region[new Point(4, 1)]);
            Assert.AreEqual(null, region[new Point(4, 2)]);
            Assert.AreEqual(null, region[new Point(4, 3)]);
            Assert.AreEqual(null, region[new Point(4, 4)]);

            Assert.AreEqual(true, region.Contains(0, 0));
            Assert.AreEqual(false, region.Contains(0, 1));
            Assert.AreEqual(false, region.Contains(0, 2));
            Assert.AreEqual(false, region.Contains(0, 3));
            Assert.AreEqual(false, region.Contains(0, 4));
            Assert.AreEqual(false, region.Contains(1, 0));
            Assert.AreEqual(false, region.Contains(1, 1));
            Assert.AreEqual(false, region.Contains(1, 2));
            Assert.AreEqual(false, region.Contains(1, 3));
            Assert.AreEqual(false, region.Contains(1, 4));
            Assert.AreEqual(false, region.Contains(2, 0));
            Assert.AreEqual(false, region.Contains(2, 1));
            Assert.AreEqual(true, region.Contains(2, 2));
            Assert.AreEqual(false, region.Contains(2, 3));
            Assert.AreEqual(false, region.Contains(2, 4));
            Assert.AreEqual(false, region.Contains(3, 0));
            Assert.AreEqual(false, region.Contains(3, 1));
            Assert.AreEqual(false, region.Contains(3, 2));
            Assert.AreEqual(true, region.Contains(3, 3));
            Assert.AreEqual(false, region.Contains(3, 4));
            Assert.AreEqual(false, region.Contains(4, 0));
            Assert.AreEqual(false, region.Contains(4, 1));
            Assert.AreEqual(false, region.Contains(4, 2));
            Assert.AreEqual(false, region.Contains(4, 3));
            Assert.AreEqual(false, region.Contains(4, 4));

            foreach (Registered reg1 in region.Elements)
            {
                bool bl1 = reg1.X == 0 && reg1.Y == 0 && reg1.Right == 2 && reg1.Bottom == 2;
                bool bl2 = reg1.X == 2 && reg1.Y == 2 && reg1.Right == 3 && reg1.Bottom == 3;
                bool bl3 = reg1.X == 3 && reg1.Y == 3 && reg1.Right == 4 && reg1.Bottom == 4;
                Assert.AreEqual(true, bl1 || bl2 || bl3);
            }

            foreach (Rectangle reg1 in region.Rectangles)
            {
                bool bl1 = reg1.X == 0 && reg1.Y == 0 && reg1.Right == 2 && reg1.Bottom == 2;
                bool bl2 = reg1.X == 2 && reg1.Y == 2 && reg1.Right == 3 && reg1.Bottom == 3;
                bool bl3 = reg1.X == 3 && reg1.Y == 3 && reg1.Right == 4 && reg1.Bottom == 4;
                Assert.AreEqual(true, bl1 || bl2 || bl3);
            }

            Attacher attacher = new Attacher(region.Width, region.Height);
            attacher.Add(0, 0);
            attacher.Add(2, 2);
            attacher.Add(3, 3);
            region.SetMask(attacher);
            attacher.SetMask(region);
            foreach (Attach.Proc proc in attacher.Attaches.Select(att => att.Unique))
            {
                bool bl = (proc.Place.X == 0 && proc.Place.Y == 0) ||
                          (proc.Place.X == 2 && proc.Place.Y == 2) ||
                          (proc.Place.X == 3 && proc.Place.Y == 3);
                Assert.AreEqual(true, bl);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredEx1Test()
        {
            // ReSharper disable once UnusedVariable
            Region reg = new Region(1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredEx2Test()
        {
            // ReSharper disable once UnusedVariable
            Region reg = new Region(1, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredEx3Test()
        {
            // ReSharper disable once UnusedVariable
            Region reg = new Region(0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredEx4Test()
        {
            // ReSharper disable once UnusedVariable
            Region reg = new Region(-12, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredTestEx5()
        {
            Region reg = new Region(10, 10);
            reg.Add(new Rectangle(0, 3, 10, 5));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredTestEx6()
        {
            Region reg = new Region(10, 10);
            reg.Add(new Rectangle(2, 3, 5, 10));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredTestEx7()
        {
            Region reg = new Region(10, 10);
            reg.Add(0, 3, 10, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredTestEx8()
        {
            Region reg = new Region(10, 10);
            reg.Add(2, 3, 5, 10);
        }
    }
}