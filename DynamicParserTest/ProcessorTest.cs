using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
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
        public void ProcessorTestSerialization()
        {
            Bitmap btm = new Bitmap(2, 2);
            btm.SetPixel(0, 0, Color.Black);
            btm.SetPixel(0, 1, Color.Red);
            btm.SetPixel(1, 0, Color.Orange);
            btm.SetPixel(1, 1, Color.White);
            //MemoryStream ms = new MemoryStream(100);
            using (FileStream ms = new FileStream("file.xml", FileMode.Create, FileAccess.Write))
            {
                new Processor(btm, "t").SaveToStream(ms);
            }
            //Processor pr = (Processor) new XmlSerializer(typeof(Processor)).Deserialize(ms);
            using (FileStream ms = new FileStream("file.xml", FileMode.Open, FileAccess.Read))
            {
                Processor pr = new Processor(ms);
            }
        }

        [TestMethod]
        public void ProcessorTest1()
        {
            Processor proc = new Processor(new Bitmap(15, 10), " f1   ");
            Assert.AreEqual("f1", proc.Tag);
            Assert.AreEqual(15, proc.Width);
            Assert.AreEqual(10, proc.Height);
            Assert.AreEqual(150, proc.Length);
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
            btm2.SetPixel(0, 0, Color.IndianRed);
            btm2.SetPixel(1, 0, Color.Chocolate);
            btm2.SetPixel(0, 1, Color.Aquamarine);
            btm2.SetPixel(1, 1, Color.DarkSeaGreen);

            for (int i = 0; i < 10; i++)
            {
                Processor proc = new Processor(btm, "Main");
                SearchResults sr1 = proc.GetEqual(new Processor(btm1, "1"), new Processor(btm2, "2"));
                ProcessorContainer pc = new ProcessorContainer(new Processor(btm1, "1"), new Processor(btm2, "2"));
                SearchResults sr2 = proc.GetEqual(pc);

                Assert.AreEqual(sr1.Width * sr1.Height, sr2.Width * sr2.Height);
                Assert.AreEqual(proc.Width, sr1.Width);
                Assert.AreEqual(proc.Height, sr1.Height);
                Assert.AreEqual(proc.Width, sr2.Width);
                Assert.AreEqual(proc.Height, sr2.Height);

                Assert.AreEqual(1, sr1[0, 0].Percent);
                Assert.AreEqual("1", sr1[0, 0].Procs[0].Tag);

                Assert.AreEqual(1, sr2[0, 0].Percent);
                Assert.AreEqual("1", sr2[0, 0].Procs[0].Tag);

                Assert.AreNotEqual(null, sr1[0, 1].Procs);
                Assert.AreNotEqual(null, sr2[0, 1].Procs);

                Assert.AreNotEqual(null, sr1[0, 2].Procs);
                Assert.AreNotEqual(null, sr2[0, 2].Procs);

                Assert.AreNotEqual(null, sr1[0, 3].Procs);
                Assert.AreNotEqual(null, sr2[0, 3].Procs);

                Assert.AreEqual(null, sr1[0, 4].Procs);
                Assert.AreEqual(null, sr2[0, 4].Procs);

                Assert.AreNotEqual(null, sr1[1, 0].Procs);
                Assert.AreNotEqual(null, sr2[1, 0].Procs);

                Assert.AreNotEqual(null, sr1[1, 1].Procs);
                Assert.AreNotEqual(null, sr2[1, 1].Procs);

                Assert.AreNotEqual(null, sr1[1, 2].Procs);
                Assert.AreNotEqual(null, sr2[1, 2].Procs);

                Assert.AreNotEqual(null, sr1[1, 3].Procs);
                Assert.AreNotEqual(null, sr2[1, 3].Procs);

                Assert.AreEqual(null, sr1[1, 4].Procs);
                Assert.AreEqual(null, sr2[1, 4].Procs);

                Assert.AreNotEqual(null, sr1[2, 0].Procs);
                Assert.AreNotEqual(null, sr2[2, 0].Procs);

                Assert.AreNotEqual(null, sr1[2, 1].Procs);
                Assert.AreNotEqual(null, sr2[2, 1].Procs);

                Assert.AreNotEqual(null, sr1[2, 2].Procs);
                Assert.AreNotEqual(null, sr2[2, 2].Procs);

                Assert.AreEqual(1, sr1[2, 3].Percent);
                Assert.AreEqual("2", sr1[2, 3].Procs[0].Tag);
                Assert.AreNotEqual(null, sr1[2, 3].Procs);

                Assert.AreEqual(1, sr2[2, 3].Percent);
                Assert.AreEqual("2", sr2[2, 3].Procs[0].Tag);
                Assert.AreNotEqual(null, sr2[2, 3].Procs);

                Assert.AreEqual(null, sr1[2, 4].Procs);
                Assert.AreEqual(null, sr2[2, 4].Procs);

                Assert.AreNotEqual(null, sr1[3, 0].Procs);
                Assert.AreNotEqual(null, sr2[3, 0].Procs);

                Assert.AreNotEqual(null, sr1[3, 1].Procs);
                Assert.AreNotEqual(null, sr2[3, 1].Procs);

                Assert.AreNotEqual(null, sr1[3, 2].Procs);
                Assert.AreNotEqual(null, sr2[3, 2].Procs);

                Assert.AreNotEqual(null, sr1[3, 3].Procs);
                Assert.AreNotEqual(null, sr2[3, 3].Procs);

                Assert.AreEqual(null, sr1[3, 4].Procs);
                Assert.AreEqual(null, sr2[3, 4].Procs);

                Assert.AreEqual(null, sr1[4, 0].Procs);
                Assert.AreEqual(null, sr2[4, 0].Procs);

                Assert.AreEqual(null, sr1[4, 1].Procs);
                Assert.AreEqual(null, sr2[4, 1].Procs);

                Assert.AreEqual(null, sr1[4, 2].Procs);
                Assert.AreEqual(null, sr2[4, 2].Procs);

                Assert.AreEqual(null, sr1[4, 3].Procs);
                Assert.AreEqual(null, sr2[4, 3].Procs);

                Assert.AreEqual(null, sr1[4, 4].Procs);
                Assert.AreEqual(null, sr2[4, 4].Procs);

                for (int y = 0; y < proc.Height; y++)
                    for (int x = 0; x < proc.Width; x++)
                    {
                        Assert.AreEqual(sr1[x, y].Percent, sr2[x, y].Percent);
                        for (int k = 0; k < (sr1[x, y].Procs?.Length ?? 0); k++)
                            Assert.AreEqual(sr1[x, y].Procs?[k].Tag, sr2[x, y].Procs[k].Tag);
                    }

                Attacher attacher;
                {
                    Region region = proc.CurrentRegion;
                    region.Add(new Rectangle(0, 0, 2, 2));
                    region.Add(new Rectangle(2, 3, 1, 1));
                    Assert.AreEqual(RegionStatus.Ok, sr1.FindRegion(region));
                    attacher = proc.CurrentAttacher;
                    attacher.Add(0, 0);
                    attacher.Add(2, 3);
                    attacher.SetMask(region);
                }

                Attacher attacher1;
                {
                    Region region1 = proc.CurrentRegion;
                    region1.Add(new Rectangle(0, 0, 2, 2));
                    region1.Add(new Rectangle(2, 3, 1, 1));
                    Assert.AreEqual(RegionStatus.Ok, sr2.FindRegion(region1));
                    attacher1 = proc.CurrentAttacher;
                    attacher1.Add(0, 0);
                    attacher1.Add(2, 3);
                    attacher1.SetMask(region1);
                }

                List<Attach.Proc> lst = attacher.Attaches.Select(att => att.Unique).ToList();
                List<Attach.Proc> lst1 = attacher1.Attaches.Select(att => att.Unique).ToList();

                Assert.AreEqual(2, lst.Count);
                Assert.AreEqual(2, lst1.Count);
                Assert.AreEqual(lst.Count, lst1.Count);

                for (int k = 0; k < lst.Count; k++)
                {
                    Attach.Proc pr = lst[k], pr1 = lst1[k];
                    Assert.AreEqual(true, pr.Place == pr1.Place);
                    Assert.AreEqual(true, pr.Procs.Count() == pr1.Procs.Count());
                    for (int j = 0; j < pr.Procs.Count(); j++)
                        Assert.AreEqual(pr.Procs.ElementAt(j).Tag, pr1.Procs.ElementAt(j).Tag);
                }

                Bitmap btm3 = new Bitmap(1, 1);
                btm3.SetPixel(0, 0, Color.DarkSeaGreen);
                Processor pr3 = new Processor(btm3, "3");
                Assert.AreEqual(1, pr3.Width);
                Assert.AreEqual(1, pr3.Width);
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
                        if (x == 4 && y == 4)
                            break;
                    }
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