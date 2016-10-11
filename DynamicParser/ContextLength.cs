using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicProcessor;

namespace DynamicParser
{
    class ContextLength
    {
        public ContextLength(ContextLine cl, SignValue diff)
        {
            if (cl == null)
                throw new ArgumentNullException();
            foreach (Line pts in cl.GetHorizontalLine(diff))
                pts.
        }
    }
}
