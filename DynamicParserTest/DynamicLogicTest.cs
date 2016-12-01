using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicParser;
using Region = DynamicParser.Region;

namespace DynamicParserTest
{
    [TestClass]
    public class DynamicLogicTest
    {
        [TestMethod]
        public void ParserTest()
        {
            Bitmap btm = new Bitmap(1, 1); //new Bitmap(@"D:\разработки\Примеры\Пример1\ImgMain.bmp");
            Bitmap btm1 = new Bitmap(1, 1);//(@"D:\разработки\Примеры\Пример1\Img1.bmp");
            Bitmap btm2 = new Bitmap(1, 1); //(@"D:\разработки\Примеры\Пример1\Img2.bmp");
            Bitmap btm3 = new Bitmap(1, 1); //(@"D:\разработки\Примеры\Пример1\Img3.bmp");

            btm.SetPixel(0, 0, Color.Black);
            btm1.SetPixel(0, 0, Color.Red);
            btm2.SetPixel(0, 0, Color.Gray);
            btm3.SetPixel(0, 0, Color.Gray);

            Processor proc = new Processor(btm, "Main");
            Processor proc1 = new Processor(btm1, "Black");
            Processor proc2 = new Processor(btm2, "Gray");
            Processor proc3 = new Processor(btm3, "Gray");

            ProcessorContainer pc = new ProcessorContainer(proc1, proc2, proc3);
            SearchResults pr = proc.GetEqual(pc);
            Region region = new Region(pc.Width, pc.Height);
            region.Add(new Rectangle(0, 0, 1, 1));
            pr.FindRegion(region);
        }
    }
}