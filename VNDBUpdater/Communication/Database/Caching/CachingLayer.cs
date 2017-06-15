using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Interfaces;

namespace VNDBUpdater.Communication.Database.Caching
{
    public class CachingLayer : ICache
    {
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> itemCallback)
            where T : class
        {
            T item = MemoryCache.Default.Get(key) as T;

            if (item == null)
            {
                item = await itemCallback();
                MemoryCache.Default.Add(key, item, null);
            }

            return item;
        }

        public async Task<IList<T>> GetAsync<T>(string key, Func<Task<IList<T>>> itemCallback)
            where T : class
        {
            IList<T> items = MemoryCache.Default.Get(key) as IList<T>;

            if (items == null)
            {
                items = await itemCallback();
                MemoryCache.Default.Add(key, items, null);
            }

            return items;
        }

        public async Task SetAsync<T> (string key, T item)
        {
            await Task.Run(() =>
            {
                MemoryCache.Default.Set(key, item, null);
            });          
        }

        public async Task SetAsync<T>(string key, IList<T> items)
        {
            await Task.Run(() =>
            {
                MemoryCache.Default.Set(key, items, null);
            });            
        }
    }
}
