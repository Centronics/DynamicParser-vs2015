using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using DynamicParser;
using DynamicProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Processor = DynamicParser.Processor;
using Region = DynamicParser.Region;

namespace DynamicParserTest
{
    [TestClass]
    public class ProcessorTest
    {
        [TestMethod]
        public void ProcessorTestGetName()
        {
            Processor proc = new Processor(new[] { SignValue.MaxValue }, "123456789");
            Assert.AreEqual("567", proc.GetProcessorName(4, 3));
            Assert.AreEqual(null, proc.GetProcessorName(4, 19));
            Assert.AreEqual(null, proc.GetProcessorName(-1, 1));
            Assert.AreEqual("1", proc.GetProcessorName(0, 1));
            Assert.AreEqual(null, proc.GetProcessorName(0, -1));
            Assert.AreEqual(null, proc.GetProcessorName(0, 0));
            Assert.AreEqual(null, proc.GetProcessorName(4, 19));
            Assert.AreEqual(null, proc.GetProcessorName(0, 10));
            Assert.AreEqual("123456789", proc.GetProcessorName(0, 9));

            Assert.AreEqual(true, proc.IsProcessorName("567", 4));
            Assert.AreEqual(false, proc.IsProcessorName(null, 4));
            Assert.AreEqual(false, proc.IsProcessorName(null, -4));
            Assert.AreEqual(false, proc.IsProcessorName(string.Empty, 4));
            Assert.AreEqual(false, proc.IsProcessorName(null, -1));
            Assert.AreEqual(false, proc.IsProcessorName(null, 9));
            Assert.AreEqual(false, proc.IsProcessorName("123", 9));
            Assert.AreEqual(false, proc.IsProcessorName("123", -4));
            Assert.AreEqual(true, proc.IsProcessorName("123456789", 0));
        }

        [TestMethod]
        public void ToStringTest()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                {
                    Processor pr = new Processor(new[] { SignValue.MaxValue }, "Tag");
                    pr.Serialize(ms);
                    Assert.AreEqual("Tag", pr.Tag);
                    Assert.AreEqual("Tag", pr.ToString());
                }
                {
                    ms.Position = 0;
                    Processor pr = new Processor(ms);
                    Assert.AreEqual("Tag", pr.Tag);
                    Assert.AreEqual("Tag", pr.ToString());
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                {
                    Processor pr = new Processor(new SignValue[5, 10], "Tag");
                    pr.Serialize(ms);
                    Assert.AreEqual("Tag", pr.Tag);
                    Assert.AreEqual("Tag", pr.ToString());
                }
                {
                    ms.Position = 0;
                    Processor pr = new Processor(ms);
                    Assert.AreEqual("Tag", pr.Tag);
                    Assert.AreEqual("Tag", pr.ToString());
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                {
                    Processor pr = new Processor(new Bitmap(1, 1), "Tag");
                    pr.Serialize(ms);
                    Assert.AreEqual("Tag", pr.Tag);
                    Assert.AreEqual("Tag", pr.ToString());
                }
                {
                    ms.Position = 0;
                    Processor pr = new Processor(ms);
                    Assert.AreEqual("Tag", pr.Tag);
                    Assert.AreEqual("Tag", pr.ToString());
                }
            }
        }

        [TestMethod]
        public void ProcessorTestSerialization()
        {
            Bitmap btm = new Bitmap(2, 3);
            btm.SetPixel(0, 0, Color.Black);
            btm.SetPixel(0, 1, Color.Red);
            btm.SetPixel(0, 2, Color.Green);
            btm.SetPixel(1, 0, Color.Orange);
            btm.SetPixel(1, 1, Color.White);
            btm.SetPixel(1, 2, Color.Blue);
            using (MemoryStream ms = new MemoryStream(100))
            {
                Processor pr1 = new Processor(btm, "TemP");
                pr1.Serialize(ms);
                ms.Position = 0;
                Processor pr2 = new Processor(ms);
                Assert.AreEqual(pr2[0, 0], pr1[0, 0]);
                Assert.AreEqual(pr2[0, 1], pr1[0, 1]);
                Assert.AreEqual(pr2[0, 2], pr1[0, 2]);
                Assert.AreEqual(pr2[1, 0], pr1[1, 0]);
                Assert.AreEqual(pr2[1, 1], pr1[1, 1]);
                Assert.AreEqual(pr2[1, 2], pr1[1, 2]);
                Assert.AreEqual(pr2.Width, pr1.Width);
                Assert.AreEqual(pr2.Height, pr1.Height);
                Assert.AreEqual(pr2.Tag, pr1.Tag);
            }
        }

        [TestMethod]
        public void ProcessorTestSerialization1()
        {
            Bitmap btm = new Bitmap(1, 1);
            btm.SetPixel(0, 0, Color.White);
            using (MemoryStream ms = new MemoryStream(100))
            {
                Processor pr1 = new Processor(btm, "T");
                pr1.Serialize(ms);
                Assert.AreEqual(17, ms.Length);
                ms.Position = 0;
                Processor pr2 = new Processor(ms);
                Assert.AreEqual(17, ms.Position);
                Assert.AreEqual(pr2[0, 0], pr1[0, 0]);
                Assert.AreEqual(pr2.Width, pr1.Width);
                Assert.AreEqual(pr2.Height, pr1.Height);
                Assert.AreEqual(pr2.Tag, pr1.Tag);
            }
        }

        static MemoryStream GetStream(int width, int height, int taglen, string tag, params int[] signs)
        {
            MemoryStream ms = new MemoryStream(100);
            byte[] bw = BitConverter.GetBytes(width);
            byte[] bh = BitConverter.GetBytes(height);
            byte[] bl = BitConverter.GetBytes(taglen);
            byte[] ta = Encoding.UTF8.GetBytes(tag);
            ms.Write(bw, 0, bw.Length);
            ms.Write(bh, 0, bh.Length);
            ms.Write(bl, 0, bl.Length);
            ms.Write(ta, 0, ta.Length);
            foreach (int sign in signs)
            {
                byte[] sBytes = BitConverter.GetBytes(sign);
                ms.Write(sBytes, 0, sBytes.Length);
            }
            ms.Position = 0;
            return ms;
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ProcessorTestSerializationEx0()
        {
            using (Stream s = GetStream(1, 5, 1, "t", 999))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(s);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ProcessorTestSerializationEx1()
        {
            using (Stream s = GetStream(5, 1, 1, "t", 999))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(s);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ProcessorTestSerializationEx2()
        {
            using (Stream ms = GetStream(1, 1, 10, "t", 999))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorTestSerializationEx3()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(5);
                ms.WriteByte(0);
                ms.Position = 0;
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProcessorTestSerializationEx4()
        {
            using (Stream ms = GetStream(-567, 1, 1, "t", 999))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProcessorTestSerializationEx5()
        {
            using (Stream ms = GetStream(1, -567, 1, "t", 999))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProcessorTestSerializationEx6()
        {
            using (Stream ms = GetStream(1, 1, -2, "t", 999))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProcessorTestSerializationEx7()
        {
            using (Stream ms = GetStream(1, 1, 1, "t", -1))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProcessorTestSerializationEx8()
        {
            using (Stream ms = GetStream(1, 2, 1, "t", 999, -888))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProcessorTestSerializationEx9()
        {
            using (Stream ms = GetStream(2, 1, 1, "t", 999, -888))
            {
                // ReSharper disable once UnusedVariable
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorTestSerializationEx10()
        {
            // ReSharper disable once UnusedVariable
            Processor pr = new Processor(null);
        }

        [TestMethod]
        public void ProcessorTest1()
        {
            Processor proc = new Processor(new Bitmap(15, 10), " f1   ");
            Assert.AreEqual("f1", proc.Tag);
            Assert.AreEqual(15, proc.Width);
            Assert.AreEqual(10, proc.Height);
            Assert.AreEqual(150, proc.Length);
            Assert.AreEqual(15, proc.Size.Width);
            Assert.AreEqual(10, proc.Size.Height);
            Region cr = proc.CurrentRegion;
            Assert.AreEqual(15, cr.Width);
            Assert.AreEqual(10, cr.Height);
            Attacher at = proc.CurrentAttacher;
            Assert.AreEqual(15, at.Width);
            Assert.AreEqual(10, at.Height);
        }

        [TestMethod]
        public void ProcessorTest2()
        {
            Bitmap btm = new Bitmap(5, 5);
            btm.SetPixel(0, 0, Color.Blue);
            btm.SetPixel(1, 0, Color.Red);
            btm.SetPixel(2, 0, Color.Orange);
            btm.SetPixel(3, 0, Color.Green);
            btm.SetPixel(4, 0, Color.Gray);
            btm.SetPixel(0, 1, Color.Wheat);
            btm.SetPixel(1, 1, Color.Yellow);
            btm.SetPixel(2, 1, Color.YellowGreen);
            btm.SetPixel(3, 1, Color.GreenYellow);
            btm.SetPixel(4, 1, Color.Black);
            btm.SetPixel(0, 2, Color.DarkMagenta);
            btm.SetPixel(1, 2, Color.Violet);
            btm.SetPixel(2, 2, Color.Brown);
            btm.SetPixel(3, 2, Color.RosyBrown);
            btm.SetPixel(4, 2, Color.Coral);
            btm.SetPixel(0, 3, Color.DeepPink);
            btm.SetPixel(1, 3, Color.Gold);
            btm.SetPixel(2, 3, Color.IndianRed);
            btm.SetPixel(3, 3, Color.Chocolate);
            btm.SetPixel(4, 3, Color.Chartreuse);
            btm.SetPixel(0, 4, Color.ForestGreen);
            btm.SetPixel(1, 4, Color.DeepPink);
            btm.SetPixel(2, 4, Color.ForestGreen);
            btm.SetPixel(3, 4, Color.Aquamarine);
            btm.SetPixel(4, 4, Color.DarkSeaGreen);
            Bitmap btm1 = new Bitmap(2, 2);
            btm1.SetPixel(0, 0, Color.Blue);
            btm1.SetPixel(1, 0, Color.Red);
            btm1.SetPixel(0, 1, Color.Wheat);
            btm1.SetPixel(1, 1, Color.Yellow);
            Bitmap btm2 = new Bitmap(2, 2);
            btm2.SetPixel(0, 0, Color.Chocolate);
            btm2.SetPixel(1, 0, Color.Chartreuse);
            btm2.SetPixel(0, 1, Color.Aquamarine);
            btm2.SetPixel(1, 1, Color.DarkSeaGreen);

            Processor proc = new Processor(btm, "Main"), prt1 = new Processor(btm1, "1"), prt2 = new Processor(btm2, "2");
            for (int i = 0; i < 50000; i++)
            {
                SearchResults sr1 = proc.GetEqual(prt1, prt2);
                SearchResults sr2 = proc.GetEqual(new ProcessorContainer(prt1, prt2));
                SearchResults[] srp3 = proc.GetEqual(new ProcessorContainer(prt1), new ProcessorContainer(prt2));
                SearchResults[] srp4 = proc.GetEqual(new List<ProcessorContainer>
                {
                    new ProcessorContainer(prt1),
                    new ProcessorContainer(prt2)
                });
                Assert.AreEqual(sr1.Width * sr1.Height, sr2.Width * sr2.Height);
                Assert.AreEqual(proc.Width, sr1.Width);
                Assert.AreEqual(proc.Height, sr1.Height);
                Assert.AreEqual(proc.Width, sr2.Width);
                Assert.AreEqual(proc.Height, sr2.Height);
                foreach (SearchResults srs in srp3)
                {
                    Assert.AreEqual(proc.Width, srs.Width);
                    Assert.AreEqual(proc.Height, srs.Height);
                }
                foreach (SearchResults srs in srp4)
                {
                    Assert.AreEqual(proc.Width, srs.Width);
                    Assert.AreEqual(proc.Height, srs.Height);
                }

                Assert.AreEqual(1, sr1[0, 0].Procs.Length);
                Assert.AreEqual(1, sr2[0, 0].Procs.Length);
                Assert.AreEqual(1, srp3[0][0, 0].Procs.Length);
                Assert.AreEqual(1, srp3[1][0, 0].Procs.Length);
                Assert.AreEqual(1, srp4[0][0, 0].Procs.Length);
                Assert.AreEqual(1, srp4[1][0, 0].Procs.Length);

                Assert.AreEqual(1, sr1[0, 0].Percent);
                Assert.AreEqual("1", sr1[0, 0].Procs[0].Tag);

                Assert.AreEqual(1, sr2[0, 0].Percent);
                Assert.AreEqual("1", sr2[0, 0].Procs[0].Tag);

                Assert.AreEqual(2, srp3.Length);
                Assert.AreEqual(2, srp4.Length);
                Assert.AreEqual(1, srp3[0][0, 0].Percent);
                Assert.AreEqual(1, srp3[1][0, 0].Percent);
                Assert.AreEqual("1", srp3[0][0, 0].Procs[0].Tag);
                Assert.AreEqual("2", srp3[1][0, 0].Procs[0].Tag);
                Assert.AreEqual(1, srp4[0][0, 0].Percent);
                Assert.AreEqual(1, srp4[1][0, 0].Percent);
                Assert.AreEqual("1", srp4[0][0, 0].Procs[0].Tag);
                Assert.AreEqual("2", srp4[1][0, 0].Procs[0].Tag);

                Assert.AreNotEqual(null, sr1[0, 1].Procs);
                Assert.AreNotEqual(null, sr2[0, 1].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[0, 1].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[0, 1].Procs);

                Assert.AreNotEqual(null, sr1[0, 2].Procs);
                Assert.AreNotEqual(null, sr2[0, 2].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[0, 2].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[0, 2].Procs);

                Assert.AreNotEqual(null, sr1[0, 3].Procs);
                Assert.AreNotEqual(null, sr2[0, 3].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[0, 3].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[0, 3].Procs);

                Assert.AreEqual(null, sr1[0, 4].Procs);
                Assert.AreEqual(null, sr2[0, 4].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[0, 4].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[0, 4].Procs);

                Assert.AreNotEqual(null, sr1[1, 0].Procs);
                Assert.AreNotEqual(null, sr2[1, 0].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[1, 0].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[1, 0].Procs);

                Assert.AreNotEqual(null, sr1[1, 1].Procs);
                Assert.AreNotEqual(null, sr2[1, 1].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[1, 1].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[1, 1].Procs);

                Assert.AreNotEqual(null, sr1[1, 2].Procs);
                Assert.AreNotEqual(null, sr2[1, 2].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[1, 2].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[1, 2].Procs);

                Assert.AreNotEqual(null, sr1[1, 3].Procs);
                Assert.AreNotEqual(null, sr2[1, 3].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[1, 3].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[1, 3].Procs);

                Assert.AreEqual(null, sr1[1, 4].Procs);
                Assert.AreEqual(null, sr2[1, 4].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[1, 4].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[1, 4].Procs);

                Assert.AreNotEqual(null, sr1[2, 0].Procs);
                Assert.AreNotEqual(null, sr2[2, 0].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[2, 0].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[2, 0].Procs);

                Assert.AreNotEqual(null, sr1[2, 1].Procs);
                Assert.AreNotEqual(null, sr2[2, 1].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[2, 1].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[2, 1].Procs);

                Assert.AreNotEqual(null, sr1[2, 2].Procs);
                Assert.AreNotEqual(null, sr2[2, 2].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[2, 2].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[2, 2].Procs);

                Assert.AreNotEqual(null, sr1[2, 3].Procs);
                Assert.AreNotEqual(null, sr2[2, 3].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[2, 3].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[2, 3].Procs);

                Assert.AreEqual(null, sr1[2, 4].Procs);
                Assert.AreEqual(null, sr2[2, 4].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[2, 4].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[2, 4].Procs);

                Assert.AreNotEqual(null, sr1[3, 0].Procs);
                Assert.AreNotEqual(null, sr2[3, 0].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[3, 0].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[3, 0].Procs);

                Assert.AreNotEqual(null, sr1[3, 1].Procs);
                Assert.AreNotEqual(null, sr2[3, 1].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[3, 1].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[3, 1].Procs);

                Assert.AreNotEqual(null, sr1[3, 2].Procs);
                Assert.AreNotEqual(null, sr2[3, 2].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreNotEqual(null, srs[3, 2].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreNotEqual(null, srs[3, 2].Procs);

                Assert.AreEqual(1, sr1[3, 3].Percent);
                Assert.AreEqual(1, sr1[3, 3].Procs.Length);
                Assert.AreEqual("2", sr1[3, 3].Procs[0].Tag);
                Assert.AreNotEqual(null, sr1[3, 3].Procs);

                Assert.AreEqual(1, sr2[3, 3].Percent);
                Assert.AreEqual(1, sr2[3, 3].Procs.Length);
                Assert.AreEqual("2", sr2[3, 3].Procs[0].Tag);
                Assert.AreNotEqual(null, sr2[3, 3].Procs);

                Assert.AreEqual(2, srp3.Length);
                Assert.AreEqual(2, srp4.Length);
                Assert.AreEqual(1, srp3[0][3, 3].Percent);
                Assert.AreEqual(1, srp3[1][3, 3].Percent);
                Assert.AreNotEqual(null, srp3[0][3, 3].Procs);
                Assert.AreNotEqual(null, srp3[1][3, 3].Procs);
                Assert.AreNotEqual(null, srp4[0][3, 3].Procs);
                Assert.AreNotEqual(null, srp4[1][3, 3].Procs);
                Assert.AreEqual(1, srp3[0][3, 3].Procs.Length);
                Assert.AreEqual(1, srp3[1][3, 3].Procs.Length);
                Assert.AreEqual(1, srp4[0][3, 3].Procs.Length);
                Assert.AreEqual(1, srp4[1][3, 3].Procs.Length);
                Assert.AreEqual("1", srp3[0][3, 3].Procs[0].Tag);
                Assert.AreEqual("2", srp3[1][3, 3].Procs[0].Tag);
                Assert.AreEqual("1", srp4[0][3, 3].Procs[0].Tag);
                Assert.AreEqual("2", srp4[1][3, 3].Procs[0].Tag);

                Assert.AreEqual(null, sr1[3, 4].Procs);
                Assert.AreEqual(null, sr2[3, 4].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[3, 4].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[3, 4].Procs);

                Assert.AreEqual(null, sr1[4, 0].Procs);
                Assert.AreEqual(null, sr2[4, 0].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[4, 0].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[4, 0].Procs);

                Assert.AreEqual(null, sr1[4, 1].Procs);
                Assert.AreEqual(null, sr2[4, 1].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[4, 1].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[4, 1].Procs);

                Assert.AreEqual(null, sr1[4, 2].Procs);
                Assert.AreEqual(null, sr2[4, 2].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[4, 2].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[4, 2].Procs);

                Assert.AreEqual(null, sr1[4, 3].Procs);
                Assert.AreEqual(null, sr2[4, 3].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[4, 3].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[4, 3].Procs);

                Assert.AreEqual(null, sr1[4, 4].Procs);
                Assert.AreEqual(null, sr2[4, 4].Procs);
                foreach (SearchResults srs in srp3)
                    Assert.AreEqual(null, srs[4, 4].Procs);
                foreach (SearchResults srs in srp4)
                    Assert.AreEqual(null, srs[4, 4].Procs);

                Assert.AreEqual(srp3.Length, srp4.Length);

                for (int y = 0; y < proc.Height; y++)
                    for (int x = 0; x < proc.Width; x++)
                    {
                        Assert.AreEqual(sr1[x, y].Percent, sr2[x, y].Percent);
                        for (int k = 0; k < srp3.Length; k++)
                            Assert.AreEqual(srp3[k][x, y].Percent, srp4[k][x, y].Percent);
                        for (int k = 0; k < (sr1[x, y].Procs?.Length ?? 0); k++)
                            Assert.AreEqual(sr1[x, y].Procs?[k].Tag, sr2[x, y].Procs?[k].Tag);
                        if (x > 3 || y > 3)
                            break;
                        for (int j = 0; j < srp3.Length; j++)
                        {
                            Assert.AreNotEqual(null, srp3[j][x, y].Procs);
                            Assert.AreNotEqual(null, srp4[j][x, y].Procs);
                            Assert.AreEqual(srp3[j][x, y].Procs.Length, srp4[j][x, y].Procs.Length);
                            for (int k = 0; k < srp3[j][x, y].Procs.Length; k++)
                                Assert.AreEqual(srp3[j][x, y].Procs?[k].Tag, srp4[j][x, y].Procs?[k].Tag);
                        }
                    }

                Attacher attacher;
                {
                    Region region = proc.CurrentRegion;
                    region.Add(new Rectangle(0, 0, 2, 2));
                    region.Add(new Rectangle(3, 3, 1, 1));
                    Assert.AreEqual(RegionStatus.Ok, sr1.FindRegion(region));
                    attacher = proc.CurrentAttacher;
                    attacher.Add(0, 0);
                    attacher.Add(3, 3);
                    attacher.SetMask(region);
                }

                Attacher attacher1;
                {
                    Region region = proc.CurrentRegion;
                    region.Add(new Rectangle(0, 0, 2, 2));
                    region.Add(new Rectangle(3, 3, 1, 1));
                    Assert.AreEqual(RegionStatus.Ok, sr2.FindRegion(region));
                    attacher1 = proc.CurrentAttacher;
                    attacher1.Add(0, 0);
                    attacher1.Add(3, 3);
                    attacher1.SetMask(region);
                }

                Attacher attacher2, attacher21;
                {
                    Region region = proc.CurrentRegion;
                    region.Add(new Rectangle(0, 0, 2, 2));
                    region.Add(new Rectangle(3, 3, 1, 1));
                    Region region1 = proc.CurrentRegion;
                    region1.Add(new Rectangle(0, 0, 2, 2));
                    region1.Add(new Rectangle(3, 3, 1, 1));
                    Assert.AreEqual(RegionStatus.Ok, srp3[0].FindRegion(region));
                    Assert.AreEqual(RegionStatus.Ok, srp3[1].FindRegion(region1));
                    attacher2 = proc.CurrentAttacher;
                    attacher2.Add(0, 0);
                    attacher2.SetMask(region);
                    attacher21 = proc.CurrentAttacher;
                    attacher21.Add(3, 3);
                    attacher21.SetMask(region1);
                }

                Attacher attacher3, attacher4;
                {
                    Region region = proc.CurrentRegion;
                    region.Add(new Rectangle(0, 0, 2, 2));
                    region.Add(new Rectangle(3, 3, 1, 1));
                    Region region1 = proc.CurrentRegion;
                    region1.Add(new Rectangle(0, 0, 2, 2));
                    region1.Add(new Rectangle(3, 3, 1, 1));
                    Assert.AreEqual(RegionStatus.Ok, srp4[0].FindRegion(region));
                    Assert.AreEqual(RegionStatus.Ok, srp4[1].FindRegion(region1));
                    attacher3 = proc.CurrentAttacher;
                    attacher3.Add(0, 0);
                    attacher3.SetMask(region);
                    attacher4 = proc.CurrentAttacher;
                    attacher4.Add(3, 3);
                    attacher4.SetMask(region1);
                }

                Assert.AreEqual(2, attacher.Attaches.Count());
                Assert.AreEqual(2, attacher1.Attaches.Count());
                Assert.AreEqual(1, attacher2.Attaches.Count());
                Assert.AreEqual(1, attacher3.Attaches.Count());
                Assert.AreEqual(1, attacher4.Attaches.Count());
                Assert.AreEqual(1, attacher21.Attaches.Count());

                AttacherTest(attacher);
                AttacherTest(attacher1);
                AttacherTest(attacher2);
                AttacherTest(attacher3);
                AttacherTest(attacher4);
                AttacherTest(attacher21);

                List<Attach.Proc> lst = attacher.Attaches.Select(att => att.Unique).ToList();
                List<Attach.Proc> lst1 = attacher1.Attaches.Select(att => att.Unique).ToList();
                List<Attach.Proc> lst3 = attacher2.Attaches.Select(att => att.Unique).ToList();
                List<Attach.Proc> lst4 = attacher3.Attaches.Select(att => att.Unique).ToList();

                Assert.AreEqual(2, lst.Count);
                Assert.AreEqual(2, lst1.Count);
                Assert.AreEqual(1, lst3.Count);
                Assert.AreEqual(1, lst4.Count);
                Assert.AreEqual(lst.Count, lst1.Count);
                Assert.AreEqual(lst3.Count, lst4.Count);

                for (int k = 0; k < lst.Count; k++)
                {
                    Attach.Proc pr = lst[k], pr1 = lst1[k];
                    Assert.AreEqual(true, pr.Place == pr1.Place);
                    Assert.AreEqual(true, pr.Procs.Count() == pr1.Procs.Count());
                    for (int j = 0; j < pr.Procs.Count(); j++)
                        Assert.AreEqual(pr.Procs.ElementAt(j).Tag, pr1.Procs.ElementAt(j).Tag);
                }

                for (int k = 0; k < lst3.Count; k++)
                {
                    Attach.Proc pr3 = lst3[k], pr4 = lst4[k];
                    Assert.AreEqual(true, pr3.Place == pr4.Place);
                    Assert.AreEqual(true, pr3.Procs.Count() == pr4.Procs.Count());
                    for (int j = 0; j < pr3.Procs.Count(); j++)
                        Assert.AreEqual(pr3.Procs.ElementAt(j).Tag, pr4.Procs.ElementAt(j).Tag);
                }

                {
                    Bitmap btm3 = new Bitmap(1, 1);
                    btm3.SetPixel(0, 0, Color.DarkSeaGreen);
                    Processor pr3 = new Processor(btm3, "3");
                    Assert.AreEqual(1, pr3.Width);
                    Assert.AreEqual(1, pr3.Height);
                    Assert.AreEqual("3", pr3.Tag);

                    SearchResults sr3 = proc.GetEqual(pr3);
                    Assert.AreEqual(5, sr3.Width);
                    Assert.AreEqual(5, sr3.Height);
                    Assert.AreEqual(1, sr3[4, 4].Percent);

                    for (int y = 0; y < sr3.Height; y++)
                        for (int x = 0; x < sr3.Width; x++)
                        {
                            Assert.AreNotEqual(null, sr3[x, y].Procs);
                            Assert.AreEqual(1, sr3[x, y].Procs.Length);
                            Assert.AreSame(pr3, sr3[x, y].Procs[0]);
                        }
                }
            }
        }

        static void AttacherTest(Attacher attacher)
        {
            foreach (Attach att in attacher.Attaches)
                Assert.AreNotEqual(null, att);
            foreach (Attach att in attacher.Attaches)
                Assert.AreNotEqual(null, att.Regs);
            foreach (Attach att in attacher.Attaches)
                Assert.AreNotEqual(null, att.Regs.Where(r => r.Position == new Point(0, 0)));
            foreach (Attach att in attacher.Attaches)
                Assert.AreNotEqual(null, att.Regs.Where(r => r.Position == new Point(3, 3)));
            foreach (Attach att in attacher.Attaches)
                Assert.AreEqual(true, att.Regs.Count(r => r.Position == new Point(0, 0)) == 1 || att.Regs.Count(r => r.Position == new Point(3, 3)) == 1);
            foreach (Attach att in attacher.Attaches)
                Assert.AreEqual(0, att.Regs.Where(r => r.Position == new Point(0, 0)).Count(r => r.SelectedProcessor == null));
            foreach (Attach att in attacher.Attaches)
                Assert.AreEqual(0, att.Regs.Where(r => r.Position == new Point(3, 3)).Count(r => r.SelectedProcessor == null));
            foreach (Attach att in attacher.Attaches)
                Assert.AreEqual(0, att.Regs.Where(r => r.Position == new Point(0, 0)).Count(r => r.SelectedProcessor.Length != 1));
            foreach (Attach att in attacher.Attaches)
                Assert.AreEqual(0, att.Regs.Where(r => r.Position == new Point(3, 3)).Count(r => r.SelectedProcessor.Length != 1));
            foreach (Attach att in attacher.Attaches)
            {
                List<Reg> r00 = new List<Reg>(att.Regs.Where(r => r.Position == new Point(0, 0) || r.Position == new Point(1, 0) ||
                    r.Position == new Point(0, 1) || r.Position == new Point(1, 1)));
                if (!r00.Any()) continue;
                Assert.AreEqual(r00.Count, r00.Count(r => r.SelectedProcessor[0].Tag == "1"));
            }
            foreach (Attach att in attacher.Attaches)
            {
                List<Reg> r33 = new List<Reg>(att.Regs.Where(r => r.Position == new Point(3, 3)));
                if (!r33.Any()) continue;
                Assert.AreEqual(r33.Count, r33.Count(r => r.SelectedProcessor[0].Tag == "2"));
            }
        }

        [TestMethod]
        public void ProcessorTest3()
        {
            Bitmap btm1 = new Bitmap(2, 2);
            btm1.SetPixel(0, 0, Color.IndianRed);
            btm1.SetPixel(1, 0, Color.Chocolate);
            btm1.SetPixel(0, 1, Color.Aquamarine);
            btm1.SetPixel(1, 1, Color.DarkSeaGreen);
            Bitmap btm2 = new Bitmap(2, 1);
            btm2.SetPixel(0, 0, Color.Red);
            btm2.SetPixel(1, 0, SignValue.MaxValue.ValueColor);

            SignValue[,] mas1 = new SignValue[2, 2];
            mas1[0, 0] = new SignValue(Color.IndianRed);
            mas1[1, 0] = new SignValue(Color.Chocolate);
            mas1[0, 1] = new SignValue(Color.Aquamarine);
            mas1[1, 1] = new SignValue(Color.DarkSeaGreen);
            SignValue[] mas2 = { new SignValue(Color.Red), SignValue.MaxValue };

            Processor procb1 = new Processor(btm1, "b1");
            Processor procb2 = new Processor(btm2, "b2");
            Processor procm1 = new Processor(mas1, "m1");
            Processor procm2 = new Processor(mas2, "m2");

            Assert.AreEqual(procb1[0, 0], procm1[0, 0]);
            Assert.AreEqual(procb1[0, 1], procm1[0, 1]);
            Assert.AreEqual(procb1[0, 1], procm1[0, 1]);
            Assert.AreEqual(procb1[1, 1], procm1[1, 1]);

            Assert.AreEqual(procb1[0, 0].ValueColor, Color.FromArgb(Color.IndianRed.ToArgb() | unchecked((int)0xFF000000)));
            Assert.AreEqual(procb1[0, 1].ValueColor, Color.FromArgb(Color.Aquamarine.ToArgb() | unchecked((int)0xFF000000)));
            Assert.AreEqual(procb1[1, 0].ValueColor, Color.FromArgb(Color.Chocolate.ToArgb() | unchecked((int)0xFF000000)));
            Assert.AreEqual(procb1[1, 1].ValueColor, Color.FromArgb(Color.DarkSeaGreen.ToArgb() | unchecked((int)0xFF000000)));

            Assert.AreEqual(procb2[0, 0], procm2[0, 0]);
            Assert.AreEqual(procb2[1, 0], procm2[1, 0]);

            Assert.AreEqual(procb2[0, 0].ValueColor, Color.FromArgb(Color.Red.ToArgb() | unchecked((int)0xFF000000)));
            Assert.AreEqual(procb2[1, 0].ValueColor, SignValue.MaxValue.ValueColor);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx1Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor((Bitmap)null, "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx2Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor((SignValue[,])null, "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx3Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor((SignValue[])null, "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx4Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new Bitmap(0, 1), "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx5Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new Bitmap(1, 0), "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx6Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new Bitmap(0, 0), "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx7Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[1, 0], "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx8Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[0, 1], "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx9Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[0, 0], "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorContainerEx10Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[0], "f");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx11Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new Bitmap(1, 1), "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx12Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new Bitmap(1, 1), "     ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx13Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new Bitmap(1, 1), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx14Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[1, 1], "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx15Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[1, 1], "     ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx16Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[1, 1], null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx17Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[1], "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx18Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[1], "   ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessorContainerEx19Test()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Processor(new SignValue[1], null);
        }
    }
}