using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DynamicParser
{
    public enum SignColor : byte
    {
        Red,
        Orange,
        Yellow,
        Green,
        LightBlue,
        Blue,
        Violet,
        Nothing = 8
    }

    public struct Information
    {
        public SignColor CurrentSign { get; set; }
        public List<SignColor> Maps { get; set; }
    }

    public sealed class Map
    {
        readonly SignColor[,] _bitmap;

        public Map(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException();
            if (height <= 0)
                throw new ArgumentException();
            _bitmap = new SignColor[width, height];
        }

        public Map(Bitmap btm)
        {
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Width <= 0 || btm.Height <= 0)
                throw new ArgumentException();
            _bitmap = new SignColor[btm.Width, btm.Height];
            for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                    _bitmap[x, y] = (SignColor)(unchecked((uint)btm.GetPixel(x, y).ToArgb()) / 2396745U);
        }

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        public IEnumerable<Map> GetSymbols(IEnumerable<Map> lstFunc)
        {
            if (lstFunc == null)
                throw new ArgumentNullException();
            IEnumerable<Map> enumerable = lstFunc as IList<Map> ?? lstFunc.ToList();
            if (!IsEqual(enumerable))
                throw new ArgumentException();
            foreach (Map map in enumerable)
            {
                Map operations = new Map(map.Width, map.Height);
                for (int y = 0; y < map.Height; y++)
                    for (int x = 0; x < map.Width; x++)
                    {
                        SignColor sc = _bitmap[x, y];
                        operations._bitmap[x, y] = map._bitmap[x, y] == sc
                            ? SignColor.Nothing
                            : sc;
                    }
                yield return operations;
            }
        }

        bool IsEqual(IEnumerable<Map> lstFunc)
        {
            if (lstFunc == null)
                return false;
            int len = Length;
            return lstFunc.All(map => len == map?.Length);
        }

        public IEnumerable<Information[,]> GetMap(IList<Map> lstFunc)
        {
            foreach (Map func in lstFunc)
            {
                if (func == null)
                    throw new ArgumentNullException();
                if (func.Length != Length)
                    throw new ArgumentException();
                Information[,] map = new Information[func.Width, func.Height];
                for (int y = 0; y < func.Height; y++)
                    for (int x = 0; x < func.Width; x++)
                    {
                        List<SignColor> lst = new List<SignColor>();
                        if (map[x, y].Maps == null)
                            map[x, y] = new Information { CurrentSign = _bitmap[x, y], Maps = lst };
                        if (_bitmap[x, y] == func._bitmap[x, y])
                            lst.Add(_bitmap[x, y]);
                    }
                yield return map;
            }
        }
    }
}