using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IRedis : IDisposable
    {
        bool IsConnected { get; }

        void Connect();
        void Reconnect();
        void Disconnect();
        Task Reset();
        void Save();
        void CheckConnection();

        Task<T> ReadEntity<T>(string key) where T : class;
        Task WriteEntity<T>(string key, T entity) where T : class;
        Task<bool> KeyExists(string key);
        Task DeleteKey(string key);
        Task<IList<string>> GetAllKeys(string pattern);        
    }
}
