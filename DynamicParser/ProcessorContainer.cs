using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
    public sealed class ProcessorContainer
    {
        readonly List<Processor> _lstProcs = new List<Processor>();

        public Processor this[int index] => _lstProcs[index];

        public int Count => _lstProcs.Count;

        public int Width { get; }

        public int Height { get; }

        public ProcessorContainer(Processor first, params Processor[] processors)
        {
            if (first == null)
                throw new ArgumentNullException();
            if (first.Length <= 0)
                throw new ArgumentException();
            if (processors == null)
                return;
            if (!InOneSize(first, processors))
                throw new ArgumentException();
            _lstProcs.Add(first);
            Width = first.Width;
            Height = first.Height;
            foreach (Processor proc in processors)
            {
                if (proc == null)
                    continue;
                _lstProcs.Add(proc);
            }
        }

        public void Add(Processor proc)
        {
            if (proc == null)
                throw new ArgumentNullException();
            if (proc.Length <= 0)
                throw new ArgumentException();
            if (proc.Width != Width)
                throw new ArgumentException();
            if (proc.Height != Height)
                throw new ArgumentException();
            _lstProcs.Add(proc);
        }

        public static bool InOneSize(Processor proc, Processor[] processors)
        {
            if (proc == null)
                return false;
            return processors != null && processors.All(pr => pr?.Width == proc.Width && pr.Height == proc.Height);
        }
    }
}