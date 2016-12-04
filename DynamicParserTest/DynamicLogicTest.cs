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
            Bitmap btm = new Bitmap(@"D:\разработки\Примеры\Пример1\ImgMain.bmp");//new Bitmap(1, 1);
            Bitmap btm1 = new Bitmap(@"D:\разработки\Примеры\Пример1\Img1.bmp");//new Bitmap(1, 1);
            Bitmap btm2 = new Bitmap(@"D:\разработки\Примеры\Пример1\Img2.bmp");//new Bitmap(1, 1);
            Bitmap btm3 = new Bitmap(@"D:\разработки\Примеры\Пример1\Img3.bmp");//new Bitmap(1, 1);

            //btm.SetPixel(0, 0, Color.Black);
            //btm1.SetPixel(0, 0, Color.Red);
            //btm2.SetPixel(0, 0, Color.Gray);
            //btm3.SetPixel(0, 0, Color.Gray);

            Processor proc = new Processor(btm, "Main");
            Processor proc1 = new Processor(btm1, "A");
            Processor proc2 = new Processor(btm2, "L");
            Processor proc3 = new Processor(btm3, "PA");

            SearchResults sr = proc.GetEqual(proc1, proc2, proc3);
            Region region = proc.CurrentRegion;
            region.Add(new Rectangle(0, 0, 44, 43));
            region.Add(new Rectangle(47, 7, 44, 43));
            sr.FindRegion(region);
        }
    }
}