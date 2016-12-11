using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DynamicParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Processor = DynamicParser.Processor;
using Region = DynamicParser.Region;

namespace DynamicParserTest
{
    [TestClass]
    public class ProcessorTest
    {
        [TestMethod]
        public void ProcessorTest1()
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
            region.SetMask(attacher);
            attacher.SetMask(region);
            List<Attach.Proc> lst = attacher.Attaches.Select(att => att.Unique).ToList();
        }
    }
}
