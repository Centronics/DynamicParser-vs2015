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
    public class ProcessorTest
    {
        [TestMethod]
        public void ProcessorTestX()
        {
            Processor proc = new Processor(new Bitmap(@"D:\разработки\Примеры\Пример1\ImgMain.bmp"), "Main");
            SearchResults sr = proc.GetEqual(
                new Processor(new Bitmap(@"D:\разработки\Примеры\Пример1\Img1.bmp"), "A"),
                new Processor(new Bitmap(@"D:\разработки\Примеры\Пример1\Img2.bmp"), "L"),
                new Processor(new Bitmap(@"D:\разработки\Примеры\Пример1\Img3.bmp"), "PA"));
            Region region = proc.CurrentRegion;
            region.Add(new Rectangle(0, 0, 44, 43));
            region.Add(new Rectangle(47, 7, 44, 43));
            sr.FindRegion(region);
            Attacher attacher = proc.CurrentAttacher;
            attacher.Add(0, 0);
            attacher.Add(48, 7);
            //region.SetMask(attacher);
            attacher.SetMask(region);
            List<Attach.Proc> lst = attacher.Attaches.Select(att => att.Unique).ToList();
        }

        [TestMethod]
        public void ProcessorTest1()
        {
            Processor proc = new Processor(new Bitmap(15, 10), "f1");
            Assert.AreEqual("f1", proc.Tag);
            Assert.AreEqual(15, proc.Width);
            Assert.AreEqual(10, proc.Height);
            Assert.AreEqual(150, proc.Length);
            Region cr = proc.CurrentRegion;
            Assert.AreEqual(15, cr.Width);
            Assert.AreEqual(10, cr.Width);
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

                Assert.AreEqual(100.0, sr1[0, 0].Percent);
                Assert.AreEqual(100.0, sr2[2, 3].Percent);

                for (int y = 0; y < sr1.Height; y++)
                    for (int x = 0; x < sr1.Width; x++)
                    {
                        Assert.AreEqual(sr1[x, y].Percent, sr2[x, y].Percent);
                        if (x != 0 && y != 0 && x != 2 && y != 3)
                        {
                            Assert.AreEqual(true, sr1[x, y].Percent < 100);
                            Assert.AreEqual(true, sr2[x, y].Percent < 100);
                        }
                        if (x >= 3 || y >= 3)
                        {
                            Assert.AreEqual(0, sr1[x, y].Percent);
                            Assert.AreEqual(null, sr1[x, y].Procs);
                            Assert.AreEqual(0, sr2[x, y].Percent);
                            Assert.AreEqual(null, sr2[x, y].Procs);
                        }
                        for (int k = 0; k < sr1[x, y].Procs.Length; k++)
                            Assert.AreEqual(sr1[x, y].Procs[k].Tag, sr2[x, y].Procs[k].Tag);
                    }

                Attacher attacher;
                {
                    Region region = proc.CurrentRegion;
                    region.Add(new Rectangle(0, 0, 44, 43));
                    region.Add(new Rectangle(47, 7, 44, 43));
                    sr1.FindRegion(region);
                    attacher = proc.CurrentAttacher;
                    attacher.Add(0, 0);
                    attacher.Add(48, 7);
                    //region.SetMask(attacher);
                    attacher.SetMask(region);
                }

                Attacher attacher1;
                {
                    Region region1 = proc.CurrentRegion;
                    region1.Add(new Rectangle(0, 0, 44, 43));
                    region1.Add(new Rectangle(47, 7, 44, 43));
                    sr2.FindRegion(region1);
                    attacher1 = proc.CurrentAttacher;
                    attacher1.Add(0, 0);
                    attacher1.Add(48, 7);
                    //region1.SetMask(attacher1);
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
                        Assert.AreEqual(pr.Procs.ElementAt(j).Tag, pr1.Procs.ElementAt(j));
                }

                Bitmap btm3 = new Bitmap(1, 1);
                btm3.SetPixel(0, 0, Color.DarkSeaGreen);
                Processor pr3 = new Processor(btm3, "3");
                SearchResults sr3 = proc.GetEqual(pr3);

                Assert.AreEqual(5, sr3.Width);
                Assert.AreEqual(5, sr3.Height);
                Assert.AreEqual(100.0, sr3[4, 4].Percent);

                for (int y = 0; y < sr3.Height; y++)
                    for (int x = 0; x < sr3.Width; x++)
                    {
                        Assert.AreNotEqual(null, sr3[x, y].Procs);
                        Assert.AreEqual(1, sr3[x, y].Procs.Length);
                        Assert.AreSame(pr3, sr3[x, y].Procs[0]);
                        if (x != 4 && y != 4)
                            Assert.AreEqual(true, sr3[x, y].Percent < 100.0);
                    }
            }
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