using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface ICache
    {
        Task<T> GetAsync<T>(string key, Func<Task<T>> itemCallback) where T : class;
        Task<IList<T>> GetAsync<T>(string key, Func<Task<IList<T>>> itemCallback) where T : class;

        Task SetAsync<T>(string key, T item);
        Task SetAsync<T>(string key, IList<T> items);
    }
}
