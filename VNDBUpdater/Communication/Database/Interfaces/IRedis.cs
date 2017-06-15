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
        Task ResetAsync();
        void Save();
        void CheckConnection();

        Task<T> ReadAsync<T>(string key) where T : class;
        Task WriteAsync<T>(string key, T entity) where T : class;
        Task<bool> CheckIfKeyExistsAsync(string key);
        Task DeleteKeyAsync(string key);
        Task<IList<string>> GetAllKeysAsync(string pattern);        
    }
}
