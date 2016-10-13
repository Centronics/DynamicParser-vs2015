using System.Collections.Generic;
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
            List<Line> linesHorizont = new List<Line>(cl.GetHorizontalLine(diff));
            while (linesHorizont.Count > 0)
            {
                Line line = linesHorizont[0];
                Line? line1 = null;
                linesHorizont.RemoveAt(0);
                for (int h = 0, x = line.PtStart.Pt.X; h < linesHorizont.Count; h++)
                {
                    Line line2 = linesHorizont[h];
                    if (line2.PtStart.Pt.X != x || (line1 != null && line2.PtStart.Pt.Y != line1.Value.PtStart.Pt.Y + 1)) continue;
                    linesHorizont.RemoveAt(h--);
                    if (line2.Length == line.Length)
                        line1 = line2;
                }
                if (line1 == null)
                    yield break;
                yield return new ObjectQuad
                {
                    X = line.PtStart.Pt.X,
                    Y = line.PtStart.Pt.Y,
                    Height = line1.Value.PtStart.Pt.Y - line.PtStart.Pt.Y,
                    Width = line.Length
                };
            }
        }
    }
}