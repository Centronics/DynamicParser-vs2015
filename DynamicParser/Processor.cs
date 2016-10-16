using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class Data
    {
        static long _idCount;

        readonly List<SignValue> _signList = new List<SignValue>();
        public long Id { get; }
        int _nextDataCounter, currentDataCounter;
        List<List<Data>> _nextData { get; } = new List<List<Data>>();

        List<Data> GetNextData()
        {
            if (_nextDataCounter <= 0)
            {
                _nextDataCounter = 1;
                return null;
            }
            if (_nextDataCounter < _nextData.Count) return _nextData[_nextDataCounter++];
            if (currentDataCounter < _nextData.Count)
            {
                List<Data> nd = _nextData[currentDataCounter];

            }
            _nextDataCounter = 0;
            return null;
        }

        public Data()
        {
            if (_idCount == long.MaxValue)
                throw new ArgumentException();
            Id = Interlocked.Increment(ref _idCount);
        }

        bool InCase(IList<SignValue> lst)
        {
            if (lst == null)
                return false;
            if (lst.Count <= 0)
                return false;
            if (_signList.Count < lst.Count)
                return false;
            return !_signList.Where((t, k) => t != lst[k]).Any();
        }

        public Data ReadValues(long id)
        {
            if (id < 0)
                return null;
            for (Data nd = this; nd != null; nd = nd.GetNextData())
                if (nd.Id == Id)
                    return nd;
            return null;
        }

        public Data ReadValues(List<SignValue> lstSv)
        {
            for (Data nd = this; nd != null; nd = nd._nextData)
                if (nd.InCase(lstSv))
                    return this;
            return null;
        }

        public long WriteValues(List<SignValue> lstSv)
        {
            if (lstSv == null)
                return -1;
            if (lstSv.Count <= 0)
                return -1;
            Data data = ReadValues(lstSv);
            if (data == null)
                return -1;
            if ()
        }

        public Data WriteValues(List<SignValue> lstSv, long id)
        {

        }
    }

    public sealed class Processor
    {
        public Processor ReadValues(List<Data> lstData)
        {

        }

        public Processor ReadValues(List<SignValue> lstSv)
        {

        }

        public Data WriteData(List<Data> lstData)
        {

        }
    }
}