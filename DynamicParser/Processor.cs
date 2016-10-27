using System;
using System.Drawing;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class Processor
    {
        readonly SignValue[,] _bitmap;

        public Processor(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException();
            if (height <= 0)
                throw new ArgumentException();
            _bitmap = new SignValue[width, height];
        }

        public Processor(Bitmap btm)
        {
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Width <= 0 || btm.Height <= 0)
                throw new ArgumentException();
            _bitmap = new SignValue[btm.Width, btm.Height];
            for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                    _bitmap[x, y] = new SignValue(btm.GetPixel(x, y));
        }

        public Processor GetMap()
        {

        }

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;
    }
}