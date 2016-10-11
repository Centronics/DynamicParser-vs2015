using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicProcessor;

namespace DynamicParser
{
    public struct ObjectQuad
    {
        public int X, Y, Width, Height;
    }

    public static class ContextWidth
    {
        public static IEnumerable<ObjectQuad> Objects(ContextLine cl, SignValue diff)
        {
            List<Line> linesVert = new List<Line>(cl.GetHorizontalLine(diff)),
                linesHorizont = new List<Line>(cl.GetVerticalLine(diff));
            //найти пересечения
        }
    }
}
