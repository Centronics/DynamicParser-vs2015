using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using DynamicParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Region = DynamicParser.Region;

namespace DynamicParserTest
{
    [TestClass]
    public class SearchResultsTest
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public void SearchTest()
        {
            {
                SearchResults s = new SearchResults(2, 3, 4, 6);
                Assert.AreEqual(2, s.Width);
                Assert.AreEqual(3, s.Height);
                Assert.AreEqual(2, s.ResultSize.Width);
                Assert.AreEqual(3, s.ResultSize.Height);
                Assert.AreEqual(2, s.MapWidth);
                Assert.AreEqual(3, s.MapHeight);
                Assert.AreEqual(4, s.MapSize.Width);
                Assert.AreEqual(6, s.MapSize.Height);
            }

            SearchResults sr = new SearchResults(5, 5, 10, 15)
            {
                [0, 0] = new ProcPerc { Percent = 1.4800 },
                [1, 0] = new ProcPerc { Percent = 1.4709 },
                [2, 0] = new ProcPerc { Percent = 2.4 },
                [3, 0] = new ProcPerc { Percent = 3.4 },
                [4, 0] = new ProcPerc { Percent = 4.4, Procs = new Processor[0] },
                [0, 1] = new ProcPerc { Percent = 1.5 },
                [1, 1] = new ProcPerc { Percent = 1.6 },
                [2, 1] = new ProcPerc { Percent = 7.4 },
                [3, 1] = new ProcPerc { Percent = 8.4 },
                [4, 1] = new ProcPerc { Percent = 9.4 },
                [0, 2] = new ProcPerc { Percent = 10.4 },
                [1, 2] = new ProcPerc { Percent = 11.4 },
                [2, 2] = new ProcPerc { Percent = 12.4 },
                [3, 2] = new ProcPerc { Percent = 13.4 },
                [4, 2] = new ProcPerc { Percent = 14.4 },
                [0, 3] = new ProcPerc { Percent = 15.4 },
                [1, 3] = new ProcPerc { Percent = 16.4 },
                [2, 3] = new ProcPerc { Percent = 17.4 },
                [3, 3] = new ProcPerc { Percent = 18.4, Procs = new Processor[0] },
                [4, 3] = new ProcPerc { Percent = 18.4 },
                [0, 4] = new ProcPerc { Percent = 19.4 },
                [1, 4] = new ProcPerc { Percent = 20.4 },
                [2, 4] = new ProcPerc { Percent = 21.4 },
                [3, 4] = new ProcPerc { Percent = 22.4 },
                [4, 4] = new ProcPerc { Percent = 23.4 }
            };

            Assert.AreEqual(5, sr.Width);
            Assert.AreEqual(5, sr.Height);
            Assert.AreEqual(10, sr.MapWidth);
            Assert.AreEqual(15, sr.MapHeight);
            Assert.AreEqual(10, sr.MapSize.Width);
            Assert.AreEqual(15, sr.MapSize.Height);
            Assert.AreEqual(5, sr.ResultSize.Width);
            Assert.AreEqual(5, sr.ResultSize.Height);

            Assert.AreEqual(1.4800, sr[0, 0].Percent);
            Assert.AreEqual(1.4709, sr[1, 0].Percent);
            Assert.AreEqual(2.4, sr[2, 0].Percent);
            Assert.AreEqual(3.4, sr[3, 0].Percent);
            Assert.AreEqual(4.4, sr[4, 0].Percent);
            Assert.AreEqual(1.5, sr[0, 1].Percent);
            Assert.AreEqual(1.6, sr[1, 1].Percent);
            Assert.AreEqual(7.4, sr[2, 1].Percent);
            Assert.AreEqual(8.4, sr[3, 1].Percent);
            Assert.AreEqual(9.4, sr[4, 1].Percent);
            Assert.AreEqual(10.4, sr[0, 2].Percent);
            Assert.AreEqual(11.4, sr[1, 2].Percent);
            Assert.AreEqual(12.4, sr[2, 2].Percent);
            Assert.AreEqual(13.4, sr[3, 2].Percent);
            Assert.AreEqual(14.4, sr[4, 2].Percent);
            Assert.AreEqual(15.4, sr[0, 3].Percent);
            Assert.AreEqual(16.4, sr[1, 3].Percent);
            Assert.AreEqual(17.4, sr[2, 3].Percent);
            Assert.AreEqual(18.4, sr[3, 3].Percent);
            Assert.AreEqual(18.4, sr[4, 3].Percent);
            Assert.AreEqual(19.4, sr[0, 4].Percent);
            Assert.AreEqual(20.4, sr[1, 4].Percent);
            Assert.AreEqual(21.4, sr[2, 4].Percent);
            Assert.AreEqual(22.4, sr[3, 4].Percent);
            Assert.AreEqual(23.4, sr[4, 4].Percent);

            Region region = new Region(4, 4);
            region.Add(0, 0, 2, 2);
            region.Add(2, 2, 1, 1);
            region.Add(3, 0, 1, 1);
            region.Add(0, 3, 1, 1);

            Assert.AreEqual(RegionStatus.Ok, sr.RegionCorrect(region));
            Assert.AreEqual(RegionStatus.Ok, sr.FindRegion(region));

            Assert.AreEqual(false, region[0, 0].IsEmpty);
            foreach (Reg reg in region[0, 0].Register)
                Assert.AreEqual(true, reg.Percent == 1.48 || reg.Percent == 1.4709);
            Assert.AreEqual(2, region[0, 0].Register.Count);
            Assert.AreEqual(new Rectangle(0, 0, 2, 2), region[0, 0].Region);
            Assert.AreEqual(false, region[0, 1].IsEmpty);
            foreach (Reg reg in region[0, 1].Register)
                Assert.AreEqual(true, reg.Percent == 1.48 || reg.Percent == 1.4709);
            Assert.AreEqual(2, region[0, 1].Register.Count);
            Assert.AreEqual(new Rectangle(0, 0, 2, 2), region[0, 1].Region);
            Assert.AreEqual(false, region[0, 3].IsEmpty);
            foreach (Reg reg in region[0, 3].Register)
                Assert.AreEqual(15.4, reg.Percent);
            Assert.AreEqual(new Rectangle(0, 3, 1, 1), region[0, 3].Region);
            Assert.AreEqual(false, region[0, 3].IsEmpty);
            foreach (Reg reg in region[0, 3].Register)
                Assert.AreEqual(15.4, reg.Percent);
            Assert.AreEqual(new Rectangle(0, 3, 1, 1), region[0, 3].Region);
            Assert.AreEqual(false, region[1, 0].IsEmpty);
            Assert.AreEqual(2, region[1, 0].Register.Count);
            foreach (Reg reg in region[1, 0].Register)
                Assert.AreEqual(true, reg.Percent == 1.48 || reg.Percent == 1.4709);
            Assert.AreEqual(new Rectangle(0, 0, 2, 2), region[1, 0].Region);
            Assert.AreEqual(false, region[1, 1].IsEmpty);
            Assert.AreEqual(2, region[1, 1].Register.Count);
            foreach (Reg reg in region[1, 1].Register)
                Assert.AreEqual(true, reg.Percent == 1.48 || reg.Percent == 1.4709);
            Assert.AreEqual(new Rectangle(0, 0, 2, 2), region[1, 1].Region);
            foreach (Reg reg in region[2, 2].Register)
                Assert.AreEqual(12.4, reg.Percent);
            Assert.AreEqual(false, region[2, 2].IsEmpty);
            Assert.AreEqual(new Rectangle(2, 2, 1, 1), region[2, 2].Region);
            Assert.AreEqual(1, region[3, 0].Register.Count);
            foreach (Reg reg in region[3, 0].Register)
                Assert.AreEqual(3.4, reg.Percent);
            Assert.AreEqual(false, region[3, 0].IsEmpty);
            Assert.AreEqual(new Rectangle(3, 0, 1, 1), region[3, 0].Region);
            Assert.AreEqual(false, region[3, 0].IsEmpty);
            foreach (Reg reg in region[3, 0].Register)
                Assert.AreEqual(3.4, reg.Percent);
            Assert.AreEqual(false, region[3, 0].IsEmpty);
            Assert.AreEqual(new Rectangle(3, 0, 1, 1), region[3, 0].Region);
            Assert.AreEqual(null, region[0, 2]);
            Assert.AreEqual(null, region[1, 2]);
            Assert.AreEqual(null, region[1, 3]);
            Assert.AreEqual(null, region[2, 0]);
            Assert.AreEqual(null, region[2, 1]);
            Assert.AreEqual(null, region[2, 3]);
            Assert.AreEqual(null, region[3, 1]);
            Assert.AreEqual(null, region[3, 2]);
            Assert.AreEqual(null, region[3, 3]);

            Assert.AreEqual(RegionStatus.Ok, sr.RegionCorrect(new Region(4, 4)));
            Assert.AreEqual(RegionStatus.Ok, sr.FindRegion(new Region(4, 4)));
            Assert.AreEqual(RegionStatus.Null, sr.RegionCorrect(null));
            Assert.AreEqual(RegionStatus.Null, sr.FindRegion(null));
            Assert.AreEqual(RegionStatus.WidthBig, sr.RegionCorrect(new Region(6, 4)));
            Assert.AreEqual(RegionStatus.WidthBig, sr.FindRegion(new Region(6, 4)));
            Assert.AreEqual(RegionStatus.HeightBig, sr.RegionCorrect(new Region(4, 6)));
            Assert.AreEqual(RegionStatus.HeightBig, sr.FindRegion(new Region(4, 6)));
            Assert.AreEqual(RegionStatus.Ok, sr.RegionCorrect(new Region(4, 5)));
            Assert.AreEqual(RegionStatus.Ok, sr.FindRegion(new Region(4, 5)));
            Assert.AreEqual(RegionStatus.Ok, sr.RegionCorrect(new Region(5, 4)));
            Assert.AreEqual(RegionStatus.Ok, sr.FindRegion(new Region(5, 4)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx1Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(-5, 5, 7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx2Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(5, -5, 7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx3Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(-5, -5, 7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx4Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(0, 5, 7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx5Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(5, 0, 7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx6Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(0, 0, 7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx7Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 2, -7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx8Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 2, 7, -8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx9Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 2, 0, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx10Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 2, 7, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx11Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 2, -7, -8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx12Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 2, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx13Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(-1, 2, -7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx14Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, -2, -7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx15Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(-1, -2, -7, -8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx16Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 2, -7, -8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx17Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, -2, -7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx18Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 0, 0, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx19Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(0, 2, -7, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx20Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 2, -7, 8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchEx21Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SearchResults(1, 0, -7, 0);
        }
    }
}