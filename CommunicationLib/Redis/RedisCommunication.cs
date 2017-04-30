using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunicationLib.Redis
{
    internal class RedisCommunication : IRedisCommunication
    {
        private RedisClient Client;

        private bool _IsConnected;

        public bool IsConnected
        {
            get { return _IsConnected; }
        }               
        
        public void Connect(string ip, int port, string redisExeLocation, string redisConfigLocation)
        {
            if (!_IsConnected)
            {
                InputSanitization.CheckStringArgument(ip);
                InputSanitization.CheckStringArgument(redisExeLocation);
                InputSanitization.CheckStringArgument(redisConfigLocation);

                if (port > 65535 || port < 1)
                    throw new ArgumentOutOfRangeException(nameof(port) + " is out of range!");

                Client = new RedisClient(ip, port, redisExeLocation, redisConfigLocation);

                _IsConnected = Client.IsConnected;
            }            
        }

        public void Disconnect()
        {
            if (_IsConnected)
            {
                Client.Disconnect();
                _IsConnected = false;
            }
        }

        public async Task WriteEntity<T>(string key, T entity) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            if (entity == null)
                throw new ArgumentNullException(nameof(entity) + " is null!");

            await Client.WriteEntity<T>(key, entity);
        }
        
        public async Task WriteEntities<T>(string key, List<T> entities) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            if (entities == null)
                throw new ArgumentNullException(nameof(entities) + " is null!");

            if (!entities.Any())
                throw new ArgumentException(nameof(entities) + " is empty!");

            await Client.WriteEntities<T>(key, entities);
        }
        
        public async Task WriteToList<T>(string key, T entity) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            if (entity == null)
                throw new ArgumentNullException(nameof(entity) + " is null");

            await Client.WriteToList<T>(key, entity);
        }

        public async Task<T> ReadEntity<T>(string key) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return await Client.ReadEntity<T>(key);
        }
                 
        public async Task<List<T>> ReadEntities<T>(string key) where T : class, new()
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return await Client.ReadEntities<T>(key);
        }
        
        public async Task<T> ReadFromList<T>(string key) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return await Client.ReadFromList<T>(key);
        }

        public async Task<double> GetNumberOfItemsOnList(string key)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return await Client.GetNumberOfItemsOnList(key);
        }
        
        public async Task DeleteKey(string key)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            await Client.DeleteKey(key);
        }
        
        public async Task<List<string>> GetKeys(string pattern)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(pattern);

            return await Client.GetKeys(pattern);
        }

        public async Task<bool> KeyExists(string key)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return await Client.KeyExists(key);
        }

        public void ForceSave()
        {
            CheckConnection();

            Client.ForceSave();
        }

        public async Task<DateTime> GetLastSave()
        {
            CheckConnection();

            return await Client.GetLastSave();
        }

        private void CheckConnection()
        {
            if (!_IsConnected)
                throw new ConnectionException("No connection to Redis DB! Be sure you called the connect method!");
        }

        public void Dispose()
        {
            if (Client != null)
            {
                Client.Dispose();
                _IsConnected = false;
            }
        }
    }
}
