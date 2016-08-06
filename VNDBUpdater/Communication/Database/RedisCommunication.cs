using CommunicationLib;
using CommunicationLib.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNUpdater.Data;

namespace VNDBUpdater.Communication.Database
{
    public static class RedisCommunication
    {
        private static readonly string ExeAndStringPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Resources\";

        private static IRedisCommunication Connection;

        private static bool _IsConnected;
        private static int _ConnectionTries = 0;

        public static bool IsConnected
        {
            get { return _IsConnected; }
        }

        private static void Connect()
        {
            if (!_IsConnected)
            {
                try
                {
                    Connection = new CommunicationLib.Communication().GetRedisCommunication();
                    Connection.Connect(Constants.RedisIP, Constants.RedisPort, ExeAndStringPath + Constants.RedisExe, ExeAndStringPath + Constants.RedisConfig);
                    _IsConnected = true;
                    Trace.TraceInformation("Connection to Redis DB established.");
                }
                catch (ConnectionException ex)
                {
                    Trace.TraceError("Connection to Redis DB could not be established. Error caught: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.GetType().Name + Environment.NewLine + ex.StackTrace);                    
                    _IsConnected = false;
                    _ConnectionTries++;

                    if (_ConnectionTries != Constants.MaxConnectionTries)
                    {
                        Trace.TraceInformation("Error handled. Trying connect again. Current ConnectionTries: " + _ConnectionTries.ToString());
                        Connect();
                    }
                    else
                    {
                        _ConnectionTries = 0;
                        Trace.TraceInformation("Error couldn't be handled. Max ConnectionTries reached.");
                    }                    
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Connection to Redis DB could not be established. Error caught: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.GetType().Name + Environment.NewLine + ex.StackTrace);
                    _IsConnected = false;
                }
            }
        }

        public static void AddVisualNovelsToDB(List<VisualNovel> visualNovels)
        {
            foreach (var visualNovel in visualNovels)
                AddVisualNovelToDB(visualNovel);
        }

        public static void AddVisualNovelToDB(VisualNovel visualNovel)
        {
            WriteEntity<VisualNovel>("VisualNovel_" + visualNovel.Basics.id, visualNovel);

            LocalVisualNovelHelper.ResetVisualNovels();
        }

        public static List<VisualNovel> GetVisualNovelsFromDB()
        {
            var existingVNs = new List<VisualNovel>();

            foreach (string key in GetAllKeys("VisualNovel_*"))
            {
                var entity = ReadEntity<VisualNovel>(key);

                existingVNs.Add(entity);
            }

            return existingVNs;
        }

        public static VisualNovel GetVisualNovelFromDB(int id)
        {
            return ReadEntity<VisualNovel>("VisualNovel_" + id.ToString());
        }

        public static void DeleteVisualNovel(int id)
        {
            DeleteKey("VisualNovel_" + id);

            LocalVisualNovelHelper.ResetVisualNovels();
        }

        public static void AddToVisualNovelSynchronizerQueue(VN VNToAdd)
        {
            WriteToList<VN>("VisualNovelSynchronizerQueue", VNToAdd);
        }

        public static List<VN> ReadFromVisualNovelSynchronizerQueue(int amount)
        {
            return ReadList<VN>("VisualNovelSynchronizerQueue", amount);
        }

        public static void AddToWishListSynchronizerQueue(Wish WishToAdd)
        {
            WriteToList<Wish>("WishSynchronizerQueue", WishToAdd);
        }

        public static List<Wish> ReadFromWishSynchronizerQueue(int amount)
        {
            return ReadList<Wish>("WishSynchronizerQueue", amount);
        }

        public static void AddToVoteListSynchronizerQueue(Vote VoteToAdd)
        {
            WriteToList<Vote>("VoteSynchronizerQueue", VoteToAdd);
        }

        public static List<Vote> ReadFromVoteListSynchronzierQueue(int amount)
        {
            return ReadList<Vote>("VoteSynchronizerQueue", amount);
        }

        public static void AddToSetVNDBSynchronizationQueue(VisualNovel VN)
        {
            WriteToList<VisualNovel>("SetVNDBSynchronizationQueue", VN);
        }

        public static List<VisualNovel> ReadFromSetVNDBSynchronizationQueue(int amount)
        {
            return ReadList<VisualNovel>("SetVNDBSynchronizationQueue", amount);
        }

        public static int GetPendingSynchronizationTasks()
        {
            return
                  GetPendingVoteSynchronizationTasks()
                + GetPendingWishSynchronizationTasks()
                + GetPendingVisualNovelSynchronizationTasks()
                + GetPendingSetVNDBSynchronizationTasks();
        }

        public static int GetPendingVoteSynchronizationTasks()
        {
            return Convert.ToInt32(GetNumberOfItemsOnList("VoteSynchronizerQueue"));
        }

        public static int GetPendingWishSynchronizationTasks()
        {
            return Convert.ToInt32(GetNumberOfItemsOnList("WishSynchronizerQueue"));
        }

        public static int GetPendingVisualNovelSynchronizationTasks()
        {
            return Convert.ToInt32(GetNumberOfItemsOnList("VisualNovelSynchronizerQueue"));
        }

        public static int GetPendingSetVNDBSynchronizationTasks()
        {
            return Convert.ToInt32(GetNumberOfItemsOnList("SetVNDBSynchronizationQueue"));
        }

        public static bool VisualNovelExistsInDatabase(int ID)
        {
            return KeyExists("VisualNovel_" + ID.ToString());
        }

        public static void ResetDatabase()
        {
            foreach (var key in GetAllKeys("*"))
                DeleteKey(key);

            FileHelper.DeleteFile(@"Resources\LocalVNStorage.rdb");
        }

        private static List<T> ReadList<T>(string ListName, int amount) where T : class, new()
        {
            var retval = new List<T>();

            for (int i = 0; i < amount; i++)
            {
                var read = ReadFromList<T>(ListName);

                if (read != null)
                    retval.Add(read);
            }

            return retval;
        }

        public static void SaveFilter(Filter filter)
        {
            WriteEntity<Filter>("Filter_" + filter.Name, filter);
        }

        public static void DeleteFilter(Filter filter)
        {
            DeleteKey("Filter_" + filter.Name);
        }

        public static Filter GetFilter(string name)
        {
            return ReadEntity<Filter>("Filter_" + name);
        }

        public static List<Filter> GetFiltersFromDatabase()
        {
            var filters = new List<Filter>();

            foreach (string key in GetAllKeys("Filter_*"))
                filters.Add(GetFilter(key.Replace("Filter_", "")));

            return filters;
        }

        public static void SetUserCredentials(string username, string password)
        {
            WriteEntity<string>("Username", username);
            WriteEntity<string>("Password", password);
        }

        public static string GetUsername()
        {
            return ReadEntity<string>("Username");
        }

        public static string GetUserPassword()
        {
            return ReadEntity<string>("Password");
        }

        public static void SetInstallFolder(string folderPath)
        {
            WriteEntity<string>("InstallFolder", folderPath);
        }

        public static string GetInstallFolder()
        {
            return ReadEntity<string>("InstallFolder");
        }

        public static bool UserCredentialsAvailable()
        {
            return KeyExists("Username");
        }

        public static void SaveRedis()
        {
            SaveCurrentDB();
            Trace.TraceInformation("Redis saved successfully.");
        }

        private static void WriteEntity<T>(string key, T entity) where T : class
        {
            CheckConnection();

            Connection.WriteEntity<T>(key, entity);
        }

        private static void WriteToList<T>(string key, T entity) where T : class
        {
            CheckConnection();

            Connection.WriteToList<T>(key, entity);
        }

        private static T ReadEntity<T>(string key) where T : class
        {
            CheckConnection();

            return Connection.ReadEntity<T>(key);
        }

        private static T ReadFromList<T>(string key) where T : class
        {
            CheckConnection();

            return Connection.ReadFromList<T>(key);
        }

        private static double GetNumberOfItemsOnList(string key)
        {
            CheckConnection();

            return Connection.GetNumberOfItemsOnList(key);
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

        private static bool KeyExists(string Key)
        {
            CheckConnection();

            return Connection.KeyExists(Key);
        }

        private static void SaveCurrentDB()
        {
            CheckConnection();

            Connection.ForceSave();
        }

        private static void CheckConnection()
        {
            Connect();

            if (!IsConnected)
                throw new Exception("Connection to RedisDB does not exist!");
        }

        public static void Disconnect()
        {
            if (IsConnected)
            {
                Connection.Disconnect();
                _IsConnected = false;
                Trace.TraceInformation("Redis disconnected successfully.");
            }
        }

        public static void Dispose()
        {
            if (IsConnected)
            {
                Connection.Disconnect();
                Connection.Dispose();
                _IsConnected = false;
                Trace.TraceInformation("Redis disposed successfully.");
            }
        }
    }
}
