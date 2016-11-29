using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicParser;

namespace DynamicParserTest
{
    [TestClass]
    public class DynamicLogicTest
    {
        [TestMethod]
        public void ParserTest()
        {
            Bitmap btm = new Bitmap(@"D:\разработки\Примеры\Пример1\ImgMain.bmp");
            Bitmap btm1 = new Bitmap(@"D:\разработки\Примеры\Пример1\Img1.bmp");
            Bitmap btm2 = new Bitmap(@"D:\разработки\Примеры\Пример1\Img2.bmp");
            Bitmap btm3 = new Bitmap(@"D:\разработки\Примеры\Пример1\Img3.bmp");

            Processor proc = new Processor(btm, "Основной");
            Processor proc1 = new Processor(btm1, 'A');
            Processor proc2 = new Processor(btm2, 'L');
            Processor proc3 = new Processor(btm3, 'A');

            ProcessorContainer pc = new ProcessorContainer(proc1, proc2, proc3);
            Processor pr = proc.GetEqual(pc);
            Rectangle rect = new Rectangle { Width = 44, Height = 43 };
            pr[0, 0].Map.Add(new RectSign { Rect = rect });
            pr[44, 2].Map.Add(new RectSign { Rect = rect });
            Processor prc = pr.GetEqual();
            List<RectSign> lst = new List<RectSign>(prc.Mapping);
        }
    }
}