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
        readonly List<Data> _nextData = new List<Data>();
        public long Id { get; private set; }
        int _nextDataCounter = -1, _currentDataCounter;

        public Data()
        {
            IdInit();
        }

        Data GetNextData()
        {
            if (_nextDataCounter < 0)
            {
                _nextDataCounter = 0;
                return this;
            }
            if (_nextDataCounter < _nextData.Count) return _nextData[_nextDataCounter++];
            if (_currentDataCounter < _nextData.Count)
            {
                if (_currentDataCounter < _nextData.Count)
                    return _nextData[_currentDataCounter++];
                _currentDataCounter = 0;
            }
            _nextDataCounter = -1;
            return null;
        }

        public Data(ICollection<SignValue> lstSv)
        {
            if (lstSv == null)
                return;
            if (lstSv.Count <= 0)
                return;
            _signList.AddRange(lstSv);
        }

        void IdInit()
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
            for (Data nd = GetNextData(); nd != null; nd = nd.GetNextData())
                if (nd.InCase(lstSv))
                    return this;
            return null;
        }

        public Data WriteValues(List<SignValue> lstSv)
        {
            if (lstSv == null)
                return null;
            if (lstSv.Count <= 0)
                return null;
            Data data = ReadValues(lstSv);
            if (data != null) return data.WriteValues(lstSv);
            if (_signList.Count <= 0)
            {
                _signList.AddRange(lstSv);
                return this;
            }
            Data dt = new Data(lstSv);
            _nextData.Add(dt);
            return dt;
        }
    }

    public sealed class Processor
    {
        public readonly Data CurrentData = new Data();
        readonly List<Data> _lstBuffer = new List<Data>();
        readonly List<SignValue> _lstSv = new List<SignValue>();

        public Processor ReadData(List<SignValue> lstSv)
        {
            if (lstSv == null)
                throw new ArgumentNullException();
            if (lstSv.Count <= 0)
                throw new ArgumentException();
            _lstBuffer.Clear();

            for (int k = 0; k < lstSv.Count; k++)
                _lstBuffer.Add(CurrentData.WriteValues(lstSv));
            _lstSv.AddRange(lstSv);
        }

        IEnumerable<List<SignValue>> GetListFormat(List<SignValue> lst)
        {

        }

        public void StopRec()
        {
            _lstSv.Clear();
        }

        public Data WriteData(List<Data> lstData)
        {
            if (lstData == null)
                throw new ArgumentNullException();
            if (lstData.Count <= 0)
                throw new ArgumentException();

        }
    }
}