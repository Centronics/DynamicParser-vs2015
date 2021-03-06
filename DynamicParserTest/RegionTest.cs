﻿using System;
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
    public class RegionTest
    {
        [TestMethod]
        public void RegisteredTest1()
        {
            Registered reg = new Registered {Region = new Rectangle(11, 12, 13, 14)};
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
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(24, 10, 20, 8)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(27, 26, 1, 1)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(24, 26, 0, 0)));
            Assert.AreEqual(true, reg.IsConflict(new Rectangle(11, 12, 20, 8)));
            Assert.AreEqual(true, reg.IsConflict(new Rectangle(11, 12, 1, 1)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(11, 12, 0, 0)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(30, 10, 20, 8)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(5, 20, 1, 10)));
            Assert.AreEqual(false, reg.IsConflict(new Rectangle(27, 28, 0, 0)));
            Assert.AreEqual(true, reg.IsConflict(new Rectangle(12, 13, 5, 5)));
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
            Assert.AreEqual(3, region[3, 3].X);
            Assert.AreEqual(3, region[3, 3].Y);
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
            Assert.AreEqual(3, region[new Point(3, 3)].X);
            Assert.AreEqual(3, region[new Point(3, 3)].Y);
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
        }

        [TestMethod]
        public void RegionTest1()
        {
            Region region = new Region(2, 2);
            region.Add(new Rectangle(0, 0, 1, 1));
            region.Add(new Rectangle(1, 1, 1, 1));
            Assert.AreEqual(2, region.Rectangles.Count());
            Assert.AreEqual(2, region.Elements.Count());
            region.Clear();
            Assert.AreEqual(0, region.Rectangles.Count());
            Assert.AreEqual(0, region.Elements.Count());
            region.Add(new Rectangle(0, 0, 1, 1));
            region.Add(new Rectangle(1, 0, 1, 1));
            region.Add(new Rectangle(0, 1, 1, 1));
            region.Add(new Rectangle(1, 1, 1, 1));
            Assert.AreEqual(4, region.Rectangles.Count());
            Assert.AreEqual(4, region.Elements.Count());
            region.Clear();
            Assert.AreEqual(0, region.Rectangles.Count());
            Assert.AreEqual(0, region.Elements.Count());
        }

        [TestMethod]
        public void RegionTest2()
        {
            Region region = new Region(2, 2);
            region.Add(0, 0, 1, 1);
            region.Add(1, 1, 1, 1);
            region[0, 0].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MaxValue}, "0, 0")};
            region[1, 1].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MinValue}, "1, 1")};
            Assert.AreEqual(true, region.Contains("1, 1", 0));
            Assert.AreEqual(true, region.Contains("0, 0", 0));
            Assert.AreEqual(true, region.Contains(", 1", 1));
            Assert.AreEqual(true, region.Contains(", 0", 1));
            Assert.AreEqual(true, region.Contains(" 1", 2));
            Assert.AreEqual(true, region.Contains(" 0", 2));
            Assert.AreEqual(true, region.Contains("1", 3));
            Assert.AreEqual(true, region.Contains("0", 3));
            Assert.AreEqual(false, region.Contains("1", 4));
            Assert.AreEqual(false, region.Contains("a", 0));
            Assert.AreEqual(false, region.Contains("b", 4));
            Assert.AreEqual(false, region.Contains("c", 3));
        }

        [TestMethod]
        public void RegionTest3()
        {
            Region region = new Region(3, 3);
            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 0, 2, 2)));
            region.Add(0, 0, 2, 2);
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 3, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 4, 4)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 0, 0)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 2, 0, 0)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 0, 0)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 0, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 1, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 0, 0)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 0, 0)));

            Assert.AreEqual(false, region.IsConflict(new Rectangle(2, 0, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(2, 1, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(2, 2, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 2, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(1, 2, 1, 1)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(-1, 2, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, -2, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, -1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 1, -1)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(-1, -2, -1, -1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, -2, -1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(-1, -2, -1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, -2, 1, -1)));

            region.Clear();

            region.Add(1, 0, 1, 1);
            region.Add(1, 1, 1, 1);
            region.Add(2, 1, 1, 1);
            region.Add(1, 2, 1, 1);

            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 0, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 1, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 2, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(2, 0, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(2, 2, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 1, 1)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 2, 2)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 0, 1, 2)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 0, 1, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 1, 4)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 3, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 0, 4, 1)));

            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 1, 1, 1)));
            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 1, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 1, 3)));

            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 3, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 1, 4, 3)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 3, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 4, 3)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 3, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 4, 1)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 3)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 3, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 4, 3)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 4)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 2, 1)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(3, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(3, 1, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(3, 1, 3, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(3, 1, 4, 3)));

            Assert.AreEqual(false, region.IsConflict(new Rectangle(2, 0, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 0, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 0, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 0, 1, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 0, 1, 4)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 1, 4)));

            Assert.AreEqual(false, region.IsConflict(new Rectangle(2, 2, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(2, 1, 2, 2)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 3, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 2, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 3, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 2, 2, 2)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 1, 2, 2)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 2, 2)));

            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 1, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(1, 0, 1, 4)));

            Assert.AreEqual(false, region.IsConflict(new Rectangle(0, 2, 1, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 2, 1, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 2, 2, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 2, 2, 2)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 2, 3, 1)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 2, 1, 3)));
            Assert.AreEqual(true, region.IsConflict(new Rectangle(0, 2, 1, 4)));
        }

        [TestMethod]
        public void RegionTest4()
        {
            Region region = new Region(2, 2);
            region.Add(0, 0, 1, 1);
            region.Add(1, 0, 1, 2);
            Assert.AreNotEqual(null, region[0, 0]);
            Assert.AreNotEqual(null, region[1, 0]);
            Assert.AreEqual(null, region[0, 1]);
            Assert.AreNotEqual(null, region[1, 1]);
            Assert.AreEqual(2, region.Count);
            region.Remove(0, 0);
            Assert.AreEqual(null, region[0, 0]);
            Assert.AreNotEqual(null, region[1, 0]);
            Assert.AreEqual(null, region[0, 1]);
            Assert.AreNotEqual(null, region[1, 1]);
            Assert.AreEqual(1, region.Count);
            region.Remove(1, 0);
            Assert.AreEqual(null, region[0, 0]);
            Assert.AreEqual(null, region[1, 0]);
            Assert.AreEqual(null, region[0, 1]);
            Assert.AreEqual(null, region[1, 1]);
            Assert.AreEqual(0, region.Count);

            region.Add(0, 0, 1, 1);
            region.Add(1, 0, 1, 1);
            region.Add(0, 1, 1, 1);
            region.Add(1, 1, 1, 1);
            Assert.AreEqual(4, region.Count);
            region.Remove(0, 0);
            Assert.AreEqual(3, region.Count);
            region.Remove(0, 0);
            Assert.AreEqual(3, region.Count);
            region.Remove(1, 0);
            Assert.AreEqual(2, region.Count);
            region.Remove(0, 1);
            Assert.AreEqual(1, region.Count);
            region.Remove(1, 1);
            Assert.AreEqual(0, region.Count);
            region.Remove(1, 1);
            Assert.AreEqual(0, region.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RegisteredOutEx1Test()
        {
            Region region = new Region(2, 2);
            region.Add(0, 0, 1, 1);
            region.Add(1, 1, 1, 1);
            region[0, 0].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MaxValue}, "0, 0")};
            region[1, 1].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MinValue}, "1, 1")};
            Assert.AreEqual(true, region.Contains("1, 1", -1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RegisteredOutEx2Test()
        {
            Region region = new Region(2, 2);
            region.Add(0, 0, 1, 1);
            region.Add(1, 1, 1, 1);
            region[0, 0].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MaxValue}, "0, 0")};
            region[1, 1].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MinValue}, "1, 1")};
            Assert.AreEqual(true, region.Contains("0, 0", -1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RegisteredOutEx3Test()
        {
            Region region = new Region(2, 2);
            region.Add(0, 0, 1, 1);
            region.Add(1, 1, 1, 1);
            region[0, 0].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MaxValue}, "0, 0")};
            region[1, 1].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MinValue}, "1, 1")};
            Assert.AreEqual(true, region.Contains("1, 1", int.MinValue));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RegisteredOutEx4Test()
        {
            Region region = new Region(2, 2);
            region.Add(0, 0, 1, 1);
            region.Add(1, 1, 1, 1);
            region[0, 0].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MaxValue}, "0, 0")};
            region[1, 1].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MinValue}, "1, 1")};
            Assert.AreEqual(true, region.Contains("0, 0", int.MinValue));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredOutEx5Test()
        {
            Region region = new Region(2, 2);
            region.Add(0, 0, 1, 1);
            region.Add(1, 1, 1, 1);
            region[0, 0].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MaxValue}, "0, 0")};
            region[1, 1].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MinValue}, "1, 1")};
            Assert.AreEqual(true, region.Contains("", 0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisteredOutEx6Test()
        {
            Region region = new Region(2, 2);
            region.Add(0, 0, 1, 1);
            region.Add(1, 1, 1, 1);
            region[0, 0].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MaxValue}, "0, 0")};
            region[1, 1].Register = new Reg {SelectedProcessor = new Processor(new[] {SignValue.MinValue}, "1, 1")};
            Assert.AreEqual(true, region.Contains(null, 0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredOutEx7Test()
        {
            Region region = new Region(2, 2);
            // ReSharper disable once UnusedVariable
            Registered r = region[-1, 0];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredOutEx8Test()
        {
            Region region = new Region(2, 2);
            // ReSharper disable once UnusedVariable
            Registered r = region[0, -1];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredOutEx9Test()
        {
            Region region = new Region(2, 2);
            // ReSharper disable once UnusedVariable
            Registered r = region[2, 0];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredOutEx10Test()
        {
            Region region = new Region(2, 2);
            // ReSharper disable once UnusedVariable
            Registered r = region[0, 2];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredOutEx11Test()
        {
            Region region = new Region(2, 2);
            // ReSharper disable once UnusedVariable
            Registered r = region[3, 0];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredOutEx12Test()
        {
            Region region = new Region(2, 2);
            // ReSharper disable once UnusedVariable
            Registered r = region[0, 3];
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
            Region reg = new Region(9, 10);
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
            Region reg = new Region(10, 9);
            reg.Add(0, 2, 10, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredTestEx8()
        {
            Region reg = new Region(10, 10);
            reg.Add(2, 3, 5, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredTestEx9()
        {
            Region reg = new Region(10, 10);
            reg.Add(2, 3, 5, 6);
            reg.Add(2, 3, 4, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredTestEx10()
        {
            Region reg = new Region(10, 10);
            reg.Add(new Rectangle(2, 3, 5, 6));
            reg.Add(new Rectangle(2, 3, 4, 2));
        }
    }
}