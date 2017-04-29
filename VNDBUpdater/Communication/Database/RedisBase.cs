using CommunicationLib;
using CommunicationLib.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.Services.Logger;

namespace VNDBUpdater.Communication.Database
{
    internal class RedisBase : IRedis
    {
        private IRedisCommunication _Connection;
        private ILoggerService _Logger;

        private const string _RedisIP = "localhost";
        private const int _RedisPort = 6379;

        private const string _RedisExe = "redis-server.exe";
        private const string _RedisConfig = "redis.windows.conf";
        private const string _DatabaseName = "LocalVNStorage.rdb";
        private const string _PathToDatabse = "Resources";

        private readonly string _DirectoryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        private int _ConnectionTries = 0;
        private const int _MaxConnectionTries = 4;

        public RedisBase(ILoggerService LoggerService)
        {
            _Logger = LoggerService;

            Connect();
        }

        private bool _IsConnected;

        public bool IsConnected
        {
            get { return _IsConnected; }
        }

        public void Connect()
        {
            if (!_IsConnected)
            {
                try
                {
                    _Connection = new CommunicationLib.Communication().GetRedisCommunication();
                    _Connection.Connect(_RedisIP, _RedisPort, Path.Combine(_DirectoryPath, _PathToDatabse, _RedisExe), Path.Combine(_DirectoryPath, _PathToDatabse, _RedisConfig));

                    _IsConnected = true;
                    _ConnectionTries = 0;

                    _Logger.Log("Connection to Redis established.");

                    BackupUpDatabase();
                }
                catch (ConnectionException ex)
                {
                    _Logger.Log(ex);
                    _IsConnected = false;
                    _ConnectionTries++;

                    if (_ConnectionTries != _MaxConnectionTries)
                    {
                        _Logger.Log("Error handled. Trying reconnect. Current tries: " + _ConnectionTries.ToString());
                        Connect();
                    }
                    else
                    {
                        _ConnectionTries = 0;
                        _Logger.Log("Error could not be handled. Maximal connection tries reached.");
                    }
                }
                catch (Exception ex)
                {
                    _Logger.Log(ex);
                    _IsConnected = false;
                }
            }
        }

        public void Reconnect()
        {
            Disconnect();
            Connect();
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                _Connection.Disconnect();
                _IsConnected = false;
            }
        }

        public void Reset()
        {
            foreach (var key in GetAllKeys("*"))
            {
                DeleteKey(key);
            }                

            if (File.Exists(Path.Combine(_PathToDatabse, _DatabaseName)))
            {
                File.Delete(Path.Combine(_PathToDatabse, _DatabaseName));
            }
        }        

        public void Save()
        {
            CheckConnection();

            DateTime lastSave = _Connection.GetLastSave();

            _Connection.ForceSave();

            while (_Connection.GetLastSave() == lastSave) { }            
        }

        public void CheckConnection()
        {
            if (!_IsConnected)
            {
                Connect();
            }
        }

        public T ReadEntity<T>(string key) where T : class
        {
            CheckConnection();

            return _Connection.ReadEntity<T>(key);
        }

        public void WriteEntity<T>(string key, T entity) where T : class
        {
            CheckConnection();

            _Connection.WriteEntity<T>(key, entity);
        }

        public bool KeyExists(string key)
        {
            return _Connection.KeyExists(key);
        }

        public void DeleteKey(string key)
        {
            CheckConnection();

            _Connection.DeleteKey(key);
        }

        public IList<string> GetAllKeys(string pattern)
        {
            CheckConnection();

            return _Connection.GetKeys(pattern);
        }

        private void BackupUpDatabase()
        {
            string BackupDatabaseName = "LocalVNStorage_Backup.rdb";

            if (File.Exists(Path.Combine(_DirectoryPath, _PathToDatabse, _DatabaseName)))
            {
                File.Copy(Path.Combine(_DirectoryPath, _PathToDatabse, _DatabaseName), Path.Combine(_DirectoryPath, _PathToDatabse, BackupDatabaseName), true);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (IsConnected)
                {
                    Disconnect();
                    _Connection.Dispose();
                    _IsConnected = false;
                }
            }
        }
    }
}
