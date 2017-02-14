using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class SearchResultsTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindRelationException1()
        {
            new SearchResults(3, 3, 1, 1).FindRelation(0, 1, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindRelationException2()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindRelationException3()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((IList<string>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindRelationException4()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((ICollection)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException5()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((ICollection)new List<string> { "test" }, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException6()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((ICollection)new List<string> { "test" }, 1, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException7()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((IList<string>)new List<string> { "test" }, 1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException8()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((IList<string>)new List<string> { "test" }, 1, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException9()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((IList<string>)new List<string> { "test" }, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException10()
        {
            new SearchResults(3, 3, 1, 1).FindRelation("test", -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException11()
        {
            new SearchResults(3, 3, 1, 1).FindRelation("test", 1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException12()
        {
            new SearchResults(3, 3, 1, 1).FindRelation("test", 1, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException13()
        {
            new SearchResults(3, 3, 1, 1).FindRelation(0, -1, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException14()
        {
            new SearchResults(3, 3, 1, 1).FindRelation(0, 0, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindRelationException15()
        {
            new SearchResults(3, 3, 1, 1).FindRelation(-1, 1, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindRelationException16()
        {
            new SearchResults(3, 3, 1, 1).FindRelation(-1, 3, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindRelationException17()
        {
            new SearchResults(3, 3, 1, 1).FindRelation(-1, 5, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindRelationException18()
        {
            new SearchResults(3, 3, 1, 1).FindRelation("test", 1, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindRelationException19()
        {
            new SearchResults(3, 3, 1, 1).FindRelation("test", 1, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindRelationException20()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((IList<string>)new List<string> { "test" }, 0, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindRelationException21()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((IList<string>)new List<string> { "test" }, 0, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindRelationException22()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((ICollection)new List<string> { "test" }, 0, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindRelationException23()
        {
            new SearchResults(3, 3, 1, 1).FindRelation((ICollection)new List<string> { "test" }, 0, 5);
        }

        [TestMethod]
        public void FindRelationTest1()
        {
            Processor pr1 = new Processor(new[] { SignValue.MaxValue }, "121");
            Processor pr2 = new Processor(new[] { SignValue.MinValue }, "552hjfgh");
            Processor pr3 = new Processor(new[] { SignValue.MinValue }, "683A");
            SearchResults sr = new SearchResults(1, 1, 1, 1)
            {
                [0, 0] = new ProcPerc { Percent = 0.45, Procs = new[] { pr1, pr3, pr2 } }
            };
            Assert.AreEqual(true, sr.FindRelation("1"));
            Assert.AreEqual(true, sr.FindRelation("5"));
            Assert.AreEqual(true, sr.FindRelation("6"));
            Assert.AreEqual(false, sr.FindRelation("7"));
            Assert.AreEqual(true, sr.FindRelation("1", 2));
            Assert.AreEqual(true, sr.FindRelation("2", 2));
            Assert.AreEqual(true, sr.FindRelation("3", 2));
            Assert.AreEqual(false, sr.FindRelation("7", 2));
            Assert.AreEqual(true, sr.FindRelation("121212", 0, 2));
            Assert.AreEqual(true, sr.FindRelation("55555555", 0, 2));
            Assert.AreEqual(false, sr.FindRelation("55555555", 0, 8));
            Assert.AreEqual(false, sr.FindRelation("55555555", 0, 4));
            Assert.AreEqual(true, sr.FindRelation("686868", 0, 2));
            Assert.AreEqual(false, sr.FindRelation("696868", 0, 2));
            Assert.AreEqual(true, sr.FindRelation("6868", 0, 2));
            Assert.AreEqual(true, sr.FindRelation("68", 0, 2));
            //добавить параллельный
        }

        [TestMethod]
        public void FindRelationTest()
        {
            Processor pr1 = new Processor(new[] { SignValue.MaxValue }, "121");
            Processor pr2 = new Processor(new[] { SignValue.MinValue }, "552hjfgh");
            Processor pr3 = new Processor(new[] { SignValue.MinValue }, "683A");
            SearchResults sr = new SearchResults(3, 3, 1, 1)
            {
                [0, 0] = new ProcPerc { Percent = 0.45, Procs = new[] { pr1, pr3 } },
                [1, 0] = new ProcPerc { Percent = 0.55, Procs = new[] { pr2 } },
                [1, 1] = new ProcPerc { Percent = 0.65, Procs = new[] { pr3 } }
            };
            for (int k = 0; k < 1000; k++)
            {
                Assert.AreEqual(null, sr.FindRelation(0, 1));
                Assert.AreEqual(false, sr.FindRelation(string.Empty));
                Assert.AreEqual(null, sr.FindRelation((IList<string>)new List<string>()));
                Assert.AreEqual(null, sr.FindRelation((ICollection)new List<string>()));
                Assert.AreEqual(false, sr.FindRelation("rwefd"));
                Assert.AreEqual(false, sr.FindRelation("19"));
                Assert.AreEqual(false, sr.FindRelation("5568", 0, 4));

                Assert.AreEqual(true, sr.FindRelation("1"));
                Assert.AreEqual(true, sr.FindRelation("5"));
                Assert.AreEqual(true, sr.FindRelation("6"));

                Assert.AreEqual(true, sr.FindRelation("1", 2));
                Assert.AreEqual(true, sr.FindRelation("2", 2));
                Assert.AreEqual(true, sr.FindRelation("3", 2));

                Assert.AreEqual(true, sr.FindRelation("121212", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("1255", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("125568", 0, 2));
                Assert.AreEqual(false, sr.FindRelation("83835221", 0, 2));

                Assert.AreEqual(true, sr.FindRelation("1212", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("1255", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("1268", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("5512", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("5555", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("5568", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("6812", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("6855", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("6868", 0, 2));

                Assert.AreEqual(true, sr.FindRelation("2121", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("2152", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("2183", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("5221", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("5252", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("5283", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("8321", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("8352", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("8383", 1, 2));

                Assert.AreEqual(true, sr.FindRelation("212121", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("212152", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("212183", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("215221", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("215252", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("215283", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("218321", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("218352", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("218383", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("832121", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("832152", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("832183", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("835221", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("835252", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("835283", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("838321", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("838352", 1, 2));
                Assert.AreEqual(true, sr.FindRelation("838383", 1, 2));

                Assert.AreEqual(true, sr.FindRelation("121212", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("121255", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("121268", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("125512", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("125555", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("125568", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("126812", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("126855", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("126868", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("551212", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("551255", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("551268", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("555512", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("555555", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("555568", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("556812", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("556855", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("556868", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("681212", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("681255", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("681268", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("685512", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("685555", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("685568", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("686812", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("686855", 0, 2));
                Assert.AreEqual(true, sr.FindRelation("686868", 0, 2));

                ConcurrentBag<string> bag = sr.FindRelation(0, 2, "121212", "121255", "121268", "125512", "125555",
                    "125568", "126812", "126855", "126868", "551212",
                    "551255", "551268", "555512", "555555", "555568", "556812", "556855", "556868", "681212", "681255",
                    "681268", "685512", "685555", "685568",
                    "686812", "686855", "686868");

                Assert.AreEqual(true, bag.Contains("121212"));
                Assert.AreEqual(true, bag.Contains("121255"));
                Assert.AreEqual(true, bag.Contains("121268"));
                Assert.AreEqual(true, bag.Contains("125512"));
                Assert.AreEqual(true, bag.Contains("125555"));
                Assert.AreEqual(true, bag.Contains("125568"));
                Assert.AreEqual(true, bag.Contains("126812"));
                Assert.AreEqual(true, bag.Contains("126855"));
                Assert.AreEqual(true, bag.Contains("126868"));
                Assert.AreEqual(true, bag.Contains("551212"));
                Assert.AreEqual(true, bag.Contains("551255"));
                Assert.AreEqual(true, bag.Contains("551268"));
                Assert.AreEqual(true, bag.Contains("555512"));
                Assert.AreEqual(true, bag.Contains("555555"));
                Assert.AreEqual(true, bag.Contains("555568"));
                Assert.AreEqual(true, bag.Contains("556812"));
                Assert.AreEqual(true, bag.Contains("556855"));
                Assert.AreEqual(true, bag.Contains("556868"));
                Assert.AreEqual(true, bag.Contains("681212"));
                Assert.AreEqual(true, bag.Contains("681255"));
                Assert.AreEqual(true, bag.Contains("681268"));
                Assert.AreEqual(true, bag.Contains("685512"));
                Assert.AreEqual(true, bag.Contains("685555"));
                Assert.AreEqual(true, bag.Contains("685568"));
                Assert.AreEqual(true, bag.Contains("686812"));
                Assert.AreEqual(true, bag.Contains("686855"));
                Assert.AreEqual(true, bag.Contains("686868"));

                bag = sr.FindRelation((ICollection)new[]
                {
                    "121212", "121255", "121268", "125512", "125555", "125568", "126812", "126855", "126868", "551212",
                    "551255", "551268", "555512", "555555", "555568", "556812", "556855", "556868", "681212", "681255",
                    "681268", "685512", "685555", "685568",
                    "686812", "686855", "686868"
                }, 0, 2);

                Assert.AreEqual(true, bag.Contains("121212"));
                Assert.AreEqual(true, bag.Contains("121255"));
                Assert.AreEqual(true, bag.Contains("121268"));
                Assert.AreEqual(true, bag.Contains("125512"));
                Assert.AreEqual(true, bag.Contains("125555"));
                Assert.AreEqual(true, bag.Contains("125568"));
                Assert.AreEqual(true, bag.Contains("126812"));
                Assert.AreEqual(true, bag.Contains("126855"));
                Assert.AreEqual(true, bag.Contains("126868"));
                Assert.AreEqual(true, bag.Contains("551212"));
                Assert.AreEqual(true, bag.Contains("551255"));
                Assert.AreEqual(true, bag.Contains("551268"));
                Assert.AreEqual(true, bag.Contains("555512"));
                Assert.AreEqual(true, bag.Contains("555555"));
                Assert.AreEqual(true, bag.Contains("555568"));
                Assert.AreEqual(true, bag.Contains("556812"));
                Assert.AreEqual(true, bag.Contains("556855"));
                Assert.AreEqual(true, bag.Contains("556868"));
                Assert.AreEqual(true, bag.Contains("681212"));
                Assert.AreEqual(true, bag.Contains("681255"));
                Assert.AreEqual(true, bag.Contains("681268"));
                Assert.AreEqual(true, bag.Contains("685512"));
                Assert.AreEqual(true, bag.Contains("685555"));
                Assert.AreEqual(true, bag.Contains("685568"));
                Assert.AreEqual(true, bag.Contains("686812"));
                Assert.AreEqual(true, bag.Contains("686855"));
                Assert.AreEqual(true, bag.Contains("686868"));

                bag = sr.FindRelation((IList<string>)new[]
                {
                    "121212", "121255", "121268", "125512", "125555", "125568", "126812", "126855", "126868", "551212",
                    "551255", "551268", "555512", "555555", "555568", "556812", "556855", "556868", "681212", "681255",
                    "681268", "685512", "685555", "685568",
                    "686812", "686855", "686868"
                }, 0, 2);

                Assert.AreEqual(true, bag.Contains("121212"));
                Assert.AreEqual(true, bag.Contains("121255"));
                Assert.AreEqual(true, bag.Contains("121268"));
                Assert.AreEqual(true, bag.Contains("125512"));
                Assert.AreEqual(true, bag.Contains("125555"));
                Assert.AreEqual(true, bag.Contains("125568"));
                Assert.AreEqual(true, bag.Contains("126812"));
                Assert.AreEqual(true, bag.Contains("126855"));
                Assert.AreEqual(true, bag.Contains("126868"));
                Assert.AreEqual(true, bag.Contains("551212"));
                Assert.AreEqual(true, bag.Contains("551255"));
                Assert.AreEqual(true, bag.Contains("551268"));
                Assert.AreEqual(true, bag.Contains("555512"));
                Assert.AreEqual(true, bag.Contains("555555"));
                Assert.AreEqual(true, bag.Contains("555568"));
                Assert.AreEqual(true, bag.Contains("556812"));
                Assert.AreEqual(true, bag.Contains("556855"));
                Assert.AreEqual(true, bag.Contains("556868"));
                Assert.AreEqual(true, bag.Contains("681212"));
                Assert.AreEqual(true, bag.Contains("681255"));
                Assert.AreEqual(true, bag.Contains("681268"));
                Assert.AreEqual(true, bag.Contains("685512"));
                Assert.AreEqual(true, bag.Contains("685555"));
                Assert.AreEqual(true, bag.Contains("685568"));
                Assert.AreEqual(true, bag.Contains("686812"));
                Assert.AreEqual(true, bag.Contains("686855"));
                Assert.AreEqual(true, bag.Contains("686868"));
            }
        }

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