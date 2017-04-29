using System;
using System.Collections.Generic;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IRedis : IDisposable
    {
        bool IsConnected { get; }

        void Connect();
        void Reconnect();
        void Disconnect();
        void Reset();
        void Save();
        void CheckConnection();

        T ReadEntity<T>(string key) where T : class;
        void WriteEntity<T>(string key, T entity) where T : class;
        bool KeyExists(string key);
        void DeleteKey(string key);
        IList<string> GetAllKeys(string pattern);        
    }
}
