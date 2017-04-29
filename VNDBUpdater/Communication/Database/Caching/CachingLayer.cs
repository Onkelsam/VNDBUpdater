using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using VNDBUpdater.Communication.Database.Interfaces;

namespace VNDBUpdater.Communication.Database.Caching
{
    public class CachingLayer : ICache
    {
        public T Get<T>(string key, Func<T> itemCallback)
            where T : class
        {
            T item = MemoryCache.Default.Get(key) as T;

            if (item == null)
            {
                item = itemCallback();
                MemoryCache.Default.Add(key, item, null);
            }

            return item;
        }

        public IList<T> GetList<T>(string key, Func<IList<T>> itemCallback)
            where T : class
        {
            IList<T> items = MemoryCache.Default.Get(key) as IList<T>;

            if (items == null)
            {
                items = itemCallback();
                MemoryCache.Default.Add(key, items, null);
            }

            return items;
        }

        public void Set<T> (string key, T item)
        {
            MemoryCache.Default.Set(key, item, null);
        }

        public void SetList<T>(string key, IList<T> items)
        {
            MemoryCache.Default.Set(key, items, null);
        }
    }
}
