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
            Context ctx = new Context(30);
            ctx.Add(10);//Assert.AreEqual(true, 
            ctx.Add(17);
            ctx.Add(15);
            ctx.Add(30);
            ctx.Add(15);
        }
    }
}