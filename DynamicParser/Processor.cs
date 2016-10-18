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

        public bool IsEmpty => (_signList?.Count ?? 0) <= 0;

        public int Length => _signList?.Count ?? 0;

        public void Clear()
        {
            _signList?.Clear();
            _nextData?.Clear();
            Id = -1;
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

        public Data ReadValues(Data data)
        {
            if (data == null)
                return null;
            return data.IsEmpty ? null : ReadValues(data._signList);
        }

        public Data WriteValues(Data data)
        {
            if (data == null)
                return null;
            return data.IsEmpty ? null : WriteValues(data._signList);
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

    public static class DataProcessor
    {
        static IEnumerable<List<SignValue>> GetData(ICollection<SignValue> lstSv)
        {
            if (lstSv == null || lstSv.Count <= 0)
                yield break;
            List<SignValue> lst = new List<SignValue>(lstSv);
            for (int k = 0; k < lstSv.Count; k++)
            {
                lst.RemoveAt(0);
                yield return lst;
            }
            lst.Clear();
            lst.AddRange(lst);
            for (int j = 0; j < lstSv.Count; j++)
                for (int k = 0; k < lst.Count; k++)
                {
                    List<SignValue> lst1 = new List<SignValue>(j);
                    for (int i = 0; i <= j && lst.Count - k <= j; i++)
                        lst1.Add(lst[k]);
                    yield return lst1;
                }
        }

        public static Data ReadData(List<SignValue> dt, Data readData)
        {
            if (dt == null || dt.Count <= 0 || (readData?.IsEmpty ?? true)) return null;
            Data data = readData.ReadValues(dt);
            foreach (List<SignValue> lst in GetData(dt))
                data.ReadValues(readData.ReadValues(lst));
            return data;
        }

        public static Data WriteData(Data dt, Data writeData)
        {
            if ((dt?.IsEmpty ?? true) || (writeData?.IsEmpty ?? true)) return null;
            Data data = writeData.WriteValues(dt);
            for (int k = 1; k < dt.Length; k++)
                data.WriteValues(writeData.WriteValues(dt));
            return data;
        }
    }
}