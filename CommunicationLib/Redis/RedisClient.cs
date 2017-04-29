using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CommunicationLib.Redis
{
    internal class RedisClient : IDisposable
    {
        private ConnectionMultiplexer Server;
        private IDatabase DB;

        private string IP;
        private int Port;
        private string RedisExecuteableLocation;
        private string RedisConfigurationLocation;

        private bool _IsConnected;

        public bool IsConnected
        {
            get { return _IsConnected; }
        }

        public RedisClient(string ip, int port, string redisExecuteableLocation, string redisConfigurationLocation)
        {
            IP = ip;
            Port = port;
            RedisExecuteableLocation = redisExecuteableLocation;
            RedisConfigurationLocation = redisConfigurationLocation;

            StartRedisDB();

            Connect();
        }

        private void Connect()
        {
            try
            {
                Server = ConnectionMultiplexer.Connect(IP + ":" + Port.ToString() + ",allowAdmin=true");
                DB = Server.GetDatabase();          
            }
            catch (StackExchange.Redis.RedisConnectionException e)
            {
                throw new ConnectionException("Could not connect to RedisDB. Make sure the database is running.", e);
            }

            _IsConnected = true;
        }

        public void Disconnect()
        {
            Server.Close();
            _IsConnected = false;
        }

        public void WriteEntity<T>(string key, T entity) where T : class
        {
            string entry = JsonConvert.SerializeObject(entity);

            DB.StringSet(key, entry, null, When.Always);
        }

        public void WriteEntities<T>(string key, List<T> entities) where T : class
        {
            string entry = JsonConvert.SerializeObject(entities);

            DB.StringSet(key, entry, null, When.Always);
        }

        public T ReadEntity<T>(string key) where T : class
        {
            if (KeyExists(key))
                return JsonConvert.DeserializeObject<T>(DB.StringGet(key));
            else
                return null;
        }

        public List<T> ReadEntities<T>(string key) where T : class, new()
        {
            if (KeyExists(key))
                return JsonConvert.DeserializeObject<List<T>>(DB.StringGet(key));
            else
                return new List<T>();
        }

        public void WriteToList<T>(string key, T entity) where T : class
        {
            DB.ListLeftPush(key, JsonConvert.SerializeObject(entity));
        }

        public T ReadFromList<T>(string key) where T : class
        {
            if (KeyExists(key))
                return JsonConvert.DeserializeObject<T>(DB.ListRightPop(key));
            else
                return null;
        }

        public double GetNumberOfItemsOnList(string key)
        {
            return DB.ListLength(key);
        }

        public bool KeyExists(string key)
        {
            return DB.KeyExists(key);
        }

        public void DeleteKey(string key)
        {
            DB.KeyDelete(key);
        }

        public List<string> GetKeys(string pattern)
        {
            var keys = new List<string>();

            foreach (var key in Server.GetServer(IP, Port).Keys(pattern: pattern))
                keys.Add(key.ToString());

            return keys;
        }

        public void ForceSave()
        {
            Server.GetServer(IP, Port).Save(SaveType.BackgroundSave);            
        }

        public DateTime GetLastSave()
        {
            return Server.GetServer(IP, Port).LastSave();
        }

        public void Dispose()
        {
            ShutDownRedis();
            Disconnect();
            Server.Dispose();
        }

        private void ShutDownRedis()
        {
            if (IsRedisAlreadyRunning())
            {
                var proc = Process.GetProcessesByName(Path.GetFileName(RedisExecuteableLocation).Replace(".exe", ""));
                if (proc.Any()) { proc[0].Kill(); }
            }                
        }

        private void StartRedisDB()
        {
            if (!IsRedisAlreadyRunning())
            {
                var redisProcess = new Process();
                redisProcess.StartInfo.FileName = RedisExecuteableLocation;
                redisProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(RedisConfigurationLocation);

                redisProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                redisProcess.StartInfo.CreateNoWindow = true;

                redisProcess.StartInfo.RedirectStandardOutput = true;
                redisProcess.StartInfo.Arguments = Path.GetFileName(RedisConfigurationLocation);

                redisProcess.StartInfo.UseShellExecute = false;

                redisProcess.Start();
            }
        }

        private bool IsRedisAlreadyRunning()
        {
            foreach (var process in Process.GetProcesses())
                if (process.ProcessName.Contains(Path.GetFileName(RedisExecuteableLocation).Replace(".exe", "")))
                    return true;

            return false;
        }
    }
}
