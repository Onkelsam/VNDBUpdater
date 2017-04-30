using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface ICache
    {
        Task<T> Get<T>(string key, Func<Task<T>> itemCallback) where T : class;
        Task<IList<T>> GetList<T>(string key, Func<Task<IList<T>>> itemCallback) where T : class;

        Task Set<T>(string key, T item);
        Task SetList<T>(string key, IList<T> items);
    }
}
