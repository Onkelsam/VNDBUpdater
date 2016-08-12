using CommunicationLib;
using CommunicationLib.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.Models.Internal;

namespace VNDBUpdater.Communication.Database
{
    public static class RedisCommunication
    {
        private static readonly string ExeAndStringPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\" + Constants.PathToDatabase;

        private static IRedisCommunication Connection;

        private static bool _IsConnected;
        private static int _ConnectionTries = 0;

        private static object Lock = new object();

        public static bool IsConnected
        {
            get { return _IsConnected; }
        }

        private static void Connect()
        {
            // Lock this specific method as it can occur that the MainViewModel.GetFiltersFromDatabase is as fast as the startup task.
            // that doesn't really matter. At least it doesn't lead to errors but it still is not nice to establish a connection two times.
            lock (Lock)
            {
                if (!_IsConnected)
                {
                    try
                    {
                        Connection = new CommunicationLib.Communication().GetRedisCommunication();
                        Connection.Connect(Constants.RedisIP, Constants.RedisPort, ExeAndStringPath + Constants.RedisExe, ExeAndStringPath + Constants.RedisConfig);
                        _IsConnected = true;
                        _ConnectionTries = 0;
                        EventLogger.LogInformation(nameof(RedisCommunication) + ":" + nameof(Connect), Constants.ConnectionEstablished);
                    }
                    catch (ConnectionException ex)
                    {
                        EventLogger.LogError(nameof(RedisCommunication) + ":" + nameof(Connect), ex);
                        _IsConnected = false;
                        _ConnectionTries++;

                        if (_ConnectionTries != Constants.MaxConnectionTries)
                        {
                            EventLogger.LogInformation(nameof(RedisCommunication) + ":" + nameof(Connect), Constants.ConnectionErrorHandled + _ConnectionTries.ToString());
                            Connect();
                        }
                        else
                        {
                            _ConnectionTries = 0;
                            EventLogger.LogInformation(nameof(RedisCommunication) + ":" + nameof(Connect), Constants.ConnectionErrorNotHandled + "(" + Constants.MaxConnectionTries + ")");
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogger.LogError(nameof(RedisCommunication) + ":" + nameof(Connect), ex);
                        _IsConnected = false;
                    }
                }
            }
        }

        public static void Reconnect()
        {
            ResetDatabase();
            Disconnect();
            Connect();
        }

        public static void AddVisualNovelToDB(VisualNovel visualNovel)
        {
            WriteEntity<VisualNovel>(Constants.VisualNovelKey + visualNovel.Basics.VNDBInformation.id, visualNovel);
        }

        public static List<VisualNovel> GetVisualNovelsFromDB()
        {
            var existingVNs = new List<VisualNovel>();

            foreach (string key in GetAllKeys(Constants.VisualNovelKey + "*"))
            {
                var entity = ReadEntity<VisualNovel>(key);

                entity.Basics = new BasicInformation(entity.Basics.VNDBInformation);

                for (int i = 0; i < entity.Characters.Count; i++)
                    entity.Characters[i] = new CharacterInformation(entity.Characters[i].VNDBInformation);

                existingVNs.Add(entity);
            }

            return existingVNs;
        }


        public static void DeleteVisualNovel(int id)
        {
            DeleteKey(Constants.VisualNovelKey + id);
        }

        public static void ResetDatabase()
        {
            foreach (var key in GetAllKeys("*"))
                DeleteKey(key);

            FileHelper.DeleteFile(Constants.PathToDatabase + Constants.DatabaseName);
        }

        public static void SaveFilter(Filter filter)
        {
            WriteEntity<Filter>(Constants.FilterKey + filter.Name, filter);
        }

        public static void DeleteFilter(Filter filter)
        {
            DeleteKey(Constants.FilterKey + filter.Name);
        }

        public static Filter GetFilter(string name)
        {
            return ReadEntity<Filter>(Constants.FilterKey + name);
        }

        public static List<Filter> GetFiltersFromDatabase()
        {
            var filters = new List<Filter>();

            foreach (string key in GetAllKeys(Constants.FilterKey + "*"))
                filters.Add(GetFilter(key.Replace(Constants.FilterKey, "")));

            return filters;
        }

        public static User GetUser()
        {
            return ReadEntity<User>(Constants.UserKey);
        }

        public static void SetUser(User user)
        {
            WriteEntity<User>(Constants.UserKey, user);
        }

        public static void SaveRedis()
        {
            SaveCurrentDB();
            EventLogger.LogInformation(nameof(RedisCommunication) + ":" + nameof(SaveRedis), "Database saved successfully.");
        }

        private static void WriteEntity<T>(string key, T entity) where T : class
        {
            CheckConnection();

            Connection.WriteEntity<T>(key, entity);
        }

        private static T ReadEntity<T>(string key) where T : class
        {
            CheckConnection();

            return Connection.ReadEntity<T>(key);
        }

        private static void DeleteKey(string key)
        {
            CheckConnection();

            Connection.DeleteKey(key);
        }

        private static List<string> GetAllKeys(string pattern)
        {
            CheckConnection();

            return Connection.GetKeys(pattern);
        }

        private static void SaveCurrentDB()
        {
            CheckConnection();

            Connection.ForceSave();
        }

        private static void CheckConnection()
        {
            if (!_IsConnected)
                Connect();
        }

        public static void Disconnect()
        {
            if (IsConnected)
            {
                Connection.Disconnect();
                _IsConnected = false;
                EventLogger.LogInformation(nameof(RedisCommunication) + ":" + nameof(Disconnect), Constants.ConnectionAborted);
            }
        }

        public static void Dispose()
        {
            if (IsConnected)
            {
                Disconnect();
                Connection.Dispose();
                _IsConnected = false;
                EventLogger.LogInformation(nameof(RedisCommunication) + ":" + nameof(Dispose), Constants.ObjectDisposed);
            }
        }
    }
}
