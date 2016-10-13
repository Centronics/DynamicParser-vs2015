using System;
using System.Collections.Generic;
using System.Linq;
using DynamicProcessor;

namespace DynamicParser
{
    public class ContextCube
    {
        public ContextCube(ContextLine source, List<ContextLine> find, SignValue diff)
        {
            if (source == null)
                throw new ArgumentNullException();
            if (find == null)
                throw new ArgumentNullException();
            if (find.Count <= 0)
                throw new ArgumentException();
            List<ObjectQuad> lstObj = new List<ObjectQuad>(ContextWidth.Objects(source, diff));
            List<List<ObjectQuad>> lstFind = new List<List<ObjectQuad>>(find.Select(cl => new List<ObjectQuad>(ContextWidth.Objects(cl, diff))));
        }
    }
}