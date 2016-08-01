using System;
using System.Collections.Generic;

namespace CommunicationLib.Redis
{
    /// <summary>
    /// Interface for Redis communication.
    /// </summary>
    public interface IRedisCommunication : IDisposable
    {
        /// <summary>
        /// Shows connection status.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to RedisDB with given IP and Port.
        /// ExeLocation must be full path to redis-server.exe.
        /// ConfigLocation must be full path to redis.windows.conf.
        /// </summary>
        void Connect(string ip, int port, string redisExeLocation, string redisConfigLocation);

        /// <summary>
        /// Disconnects from RedisDB.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Write given Entity to RedisDB with given key.
        /// </summary>
        /// <typeparam name="T">Must be a class</typeparam>
        void WriteEntity<T>(string key, T entity) where T : class;

        /// <summary>
        /// Writes given Entities to RedisDB with given key.
        /// </summary>
        /// <typeparam name="T">Must be a class</typeparam>
        void WriteEntities<T>(string key, List<T> entities) where T : class;

        /// <summary>
        /// Writes given entity to List from the left.
        /// If key doesn't exist it will be created.
        /// </summary>
        /// <typeparam name="T">Must be a class</typeparam>
        void WriteToList<T>(string key, T entity) where T : class;

        /// <summary>
        /// Read entity with given key.
        /// Returns null if key does not exist.
        /// </summary>
        /// <typeparam name="T">Must be a class.</typeparam>
        T ReadEntity<T>(string key) where T : class;

        /// <summary>
        /// Reads entities with given key.
        /// Returns an empty list if key does not exist.
        /// </summary>
        /// <typeparam name="T">Must be class and must have an empty constructor</typeparam>
        List<T> ReadEntities<T>(string key) where T : class, new();

        /// <summary>
        /// Reads entity from list with given key from the right side.
        /// </summary>
        /// <typeparam name="T">Must be a class.</typeparam>
        T ReadFromList<T>(string key) where T : class;

        /// <summary>
        /// Gets number of items on given list.
        /// </summary>
        double GetNumberOfItemsOnList(string key);

        /// <summary>
        /// Gets key with given pattern from RedisDB.
        /// </summary>
        /// <param name="pattern">Example: "Novels*" or "*" for every key.</param>
        List<string> GetKeys(string pattern);

        /// <summary>
        /// Deletes key from RedisDB.
        /// </summary>
        void DeleteKey(string key);

        /// <summary>
        /// Check if key exists.
        /// </summary>
        bool KeyExists(string Key);

        /// <summary>
        /// Forces RedisDB to save the current DB to disk.
        /// </summary>
        void ForceSave();
    }
}
