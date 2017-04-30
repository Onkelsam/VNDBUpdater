﻿using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Interfaces;

namespace VNDBUpdater.Communication.Database.Caching
{
    public class CachingLayer : ICache
    {
        public async Task<T> Get<T>(string key, Func<Task<T>> itemCallback)
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

        public async Task<IList<T>> GetList<T>(string key, Func<Task<IList<T>>> itemCallback)
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

        public async Task Set<T> (string key, T item)
        {
            MemoryCache.Default.Set(key, item, null);
        }

        public async Task SetList<T>(string key, IList<T> items)
        {
            MemoryCache.Default.Set(key, items, null);
        }
    }
}
