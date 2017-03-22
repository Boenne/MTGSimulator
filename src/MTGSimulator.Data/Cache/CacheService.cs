using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGSimulator.Data.Cache
{
    public interface ICacheService
    {
        void Cache(string methodName, string tag, object output, params object[] args);
        void Invalidate(string tag);
        CacheServiceResponse<T> Get<T>(string methodName, params object[] args);
    }

    public class CacheService : ICacheService
    {
        private static readonly List<CacheItem> CacheItems = new List<CacheItem>();

        public void Cache(string methodName, string tag, object output, params object[] args)
        {
            lock (CacheItems)
            {
                CacheItems.Add(new CacheItem
                {
                    MethodName = methodName,
                    Output = output,
                    Args = args.Select(x => x.GetHashCode()).ToList(),
                    Tag = tag
                });
            }
        }

        public void Invalidate(string tag)
        {
            lock (CacheItems)
            {
                CacheItems.RemoveAll(x => x.Tag == tag);
            }
        }

        public CacheServiceResponse<T> Get<T>(string methodName, params object[] args)
        {
            lock (CacheItems)
            {
                var cacheItem =
                    CacheItems.FirstOrDefault(
                        x =>
                            x.MethodName == methodName &&
                            IsEqual(x.Args, args.Select(y => y.GetHashCode()).ToList()));
                if (cacheItem != null)
                    return new CacheServiceResponse<T>
                    {
                        Hit = true,
                        Value = (T) cacheItem.Output
                    };
                return new CacheServiceResponse<T>
                {
                    Hit = false
                };
            }
        }

        private static bool IsEqual(IReadOnlyList<int> list, IReadOnlyList<int> list2)
        {
            if (list.Count != list2.Count) return false;
            for (var i = 0; i < list.Count; i++)
                if (list[i] != list2[i]) return false;
            return true;
        }

        private class CacheItem
        {
            public CacheItem()
            {
                CachedTime = DateTime.Now;
            }

            public string MethodName { get; set; }
            public object Output { get; set; }
            public List<int> Args { get; set; }
            public string Tag { get; set; }
            public DateTime CachedTime { get; private set; }
        }
    }
}