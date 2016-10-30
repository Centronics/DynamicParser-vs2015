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
            Processor proc = new Processor(new Bitmap(@"D:\разработки\tst\A_My.png"));
            proc.Add(new Processor(new Bitmap(@"D:\разработки\tst\A_orig.png")), 0, 0);
            proc.Add(new Processor(new Bitmap(@"D:\разработки\tst\L_orig.png")), 0, 0);
            Processor.ProcStruct? ps = proc.GetEqual();
        }
    }
}