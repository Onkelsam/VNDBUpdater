using System;
using System.Collections.Generic;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface ICache
    {
        T Get<T>(string key, Func<T> itemCallback) where T : class;
        IList<T> GetList<T>(string key, Func<IList<T>> itemCallback) where T : class;

        void Set<T>(string key, T item);
        void SetList<T>(string key, IList<T> items);
    }
}
