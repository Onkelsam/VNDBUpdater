using System;
using System.Collections.Generic;
using System.Linq;

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

        public void WriteEntity<T>(string key, T entity) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            if (entity == null)
                throw new ArgumentNullException(nameof(entity) + " is null!");

            Client.WriteEntity<T>(key, entity);
        }
        
        public void WriteEntities<T>(string key, List<T> entities) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            if (entities == null)
                throw new ArgumentNullException(nameof(entities) + " is null!");

            if (!entities.Any())
                throw new ArgumentException(nameof(entities) + " is empty!");

            Client.WriteEntities<T>(key, entities);
        }
        
        public void WriteToList<T>(string key, T entity) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            if (entity == null)
                throw new ArgumentNullException(nameof(entity) + " is null");

            Client.WriteToList<T>(key, entity);
        }

        public T ReadEntity<T>(string key) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return Client.ReadEntity<T>(key);
        }
                 
        public List<T> ReadEntities<T>(string key) where T : class, new()
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return Client.ReadEntities<T>(key);
        }
        
        public T ReadFromList<T>(string key) where T : class
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return Client.ReadFromList<T>(key);
        }

        public double GetNumberOfItemsOnList(string key)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return Client.GetNumberOfItemsOnList(key);
        }
        
        public void DeleteKey(string key)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            Client.DeleteKey(key);
        }
        
        public List<string> GetKeys(string pattern)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(pattern);

            return Client.GetKeys(pattern);
        }

        public bool KeyExists(string key)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(key);

            return Client.KeyExists(key);
        }

        public void ForceSave()
        {
            CheckConnection();

            Client.ForceSave();
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
