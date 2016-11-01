using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class Map
    {
        Thread _thread;
        public string ErrorString { get; private set; }
        public int Width { get; }
        public int Height { get; }
        public int Length => Width * Height;
        public decimal Step => Length / 100.0m;

        public Map(SignValue[,] signs)
        {
            if (signs == null)
                throw new ArgumentNullException();
            if (signs.Length <= 0)
                throw new ArgumentException();
            Width = signs.GetLength(0);
            Height = signs.GetLength(1);
        }

        void CreateMatrix(Map map)
        {
            decimal step = Length > map.Length ? map.Length / 100.0m : Length / 100.0m;
            for (int k = 0; k < Length; k++)
                //if...
        }

        public int GetIndex(decimal index)
        {
            if (index < 0)
                throw new ArgumentException();
            decimal step = Length * index;
            int ind = Convert.ToInt32(step);
            return ind >= Length ? Length - 1 : ind;
        }

        public string GetMatrix(Map map)
        {
            if (_thread != null)
                return ErrorString;
            ErrorString = string.Empty;
            if (map == null)
                throw new ArgumentNullException();
            if (map.Length <= 0)
                throw new ArgumentException();
            (_thread = new Thread(() =>
            {
                try
                {
                    CreateMatrix(map);
                }
                catch (Exception ex)
                {
                    ErrorString = ex.Message;
                }
            })
            {
                IsBackground = true,
                Name = nameof(GetMatrix),
                Priority = ThreadPriority.AboveNormal
            }).Start();
            return ErrorString;
        }
    }
}