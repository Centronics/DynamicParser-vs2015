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

        IEnumerable<Data> NextData
        {
            get
            {
                yield return this;
                foreach (Data dt in _nextData)
                    yield return dt;
            }
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
            return id < 0 ? null : NextData.FirstOrDefault(nd => (nd?.Id ?? -1) == Id);
        }

        public Data ReadValues(List<SignValue> lstSv)
        {
            return NextData.Any(nd => nd.InCase(lstSv)) ? this : null;
        }

        public Data ReadValues(Data data)
        {
            return (data?.IsEmpty ?? true) ? null : ReadValues(data._signList);
        }

        public Data WriteValues(Data data)
        {
            return (data?.IsEmpty ?? true) ? null : WriteValues(data._signList);
        }

        public void AddValues(Data data)
        {
            if (data?.IsEmpty ?? true)
                return;
            _nextData.Add(data);
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
        static IEnumerable<List<SignValue>> GetData(IList<SignValue> lstSv)
        {
            if (lstSv == null || lstSv.Count <= 0)
                yield break;
            List<SignValue> lst = new List<SignValue>(lstSv);
            for (int k = 0; k < lstSv.Count; k++)
            {
                lst.RemoveAt(0);
                yield return lst;
            }
            for (int h = 0; h < lstSv.Count; h++)
                for (int j = 1; j <= lstSv.Count; j++)
                {
                    List<SignValue> lst1 = new List<SignValue>(j);
                    for (int k = h; k < lstSv.Count; k++)
                    {
                        if (lstSv.Count - k > j)
                            break;
                        lst1.Add(lstSv[k]);
                    }
                    yield return lst1;
                }
        }

        public static Data ReadData(List<SignValue> dt, Data readData)
        {
            if (dt == null || dt.Count <= 0 || (readData?.IsEmpty ?? true)) return null;
            Data data = readData.ReadValues(dt);
            foreach (List<SignValue> lst in GetData(dt))
                data.AddValues(readData.ReadValues(lst));
            return data;
        }

        public static Data WriteData(Data dt, Data writeData)
        {
            if ((dt?.IsEmpty ?? true) || (writeData?.IsEmpty ?? true)) return null;
            Data data = writeData.WriteValues(dt);
            for (int k = 1; k < dt.Length; k++)
                data.AddValues(writeData.WriteValues(dt));
            return data;
        }
    }
}