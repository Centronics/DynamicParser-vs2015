using System;
using System.Drawing;
using DynamicProcessor;

namespace DynamicParser
{
    [Serializable]
    public sealed class Processor
    {
        SignValue[,] _bitmap;
        int? _x, _y;
        Processor _nextProcessor;

        public void Add(Bitmap btm)
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

        public void Add(Processor proc, int x, int y)
        {
            if (_nextProcessor != null || _x == null || _y == null)
                throw new ArgumentException();
            if (proc == null)
                throw new ArgumentNullException();
            if (proc.Length <= 0)
                throw new ArgumentException();
            if (x < 0 || y < 0)
                throw new ArgumentException();
            if (proc.Width + x >= Width)
                throw new ArgumentException();
            if (proc.Height + y >= Height)
                throw new ArgumentException();
            _x = x;
            _y = y;
            _nextProcessor = proc;
        }

        public ulong GetEqual(int level)
        {
            for(int y=0;y< Height;y++)
                for (int x = 0; x < Width; x++)
                
        }

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;
    }
}