using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            catch (RedisConnectionException e)
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

        public async Task WriteEntity<T>(string key, T entity) where T : class
        {
            string entry = JsonConvert.SerializeObject(entity);

            await DB.StringSetAsync(key, entry, null, When.Always);
        }

        public async Task WriteEntities<T>(string key, List<T> entities) where T : class
        {
            string entry = JsonConvert.SerializeObject(entities);

            await DB.StringSetAsync(key, entry, null, When.Always);
        }

        public async Task<T> ReadEntity<T>(string key) where T : class
        {
            if (await KeyExists(key))
                return JsonConvert.DeserializeObject<T>(await DB.StringGetAsync(key));
            else
                return null;
        }

        public async Task<List<T>> ReadEntities<T>(string key) where T : class, new()
        {
            if (await KeyExists(key))
                return JsonConvert.DeserializeObject<List<T>>(await DB.StringGetAsync(key));
            else
                return new List<T>();
        }

        public async Task WriteToList<T>(string key, T entity) where T : class
        {
            await DB.ListLeftPushAsync(key, JsonConvert.SerializeObject(entity));
        }

        public async Task<T> ReadFromList<T>(string key) where T : class
        {
            if (await KeyExists(key))
                return JsonConvert.DeserializeObject<T>(await DB.ListRightPopAsync(key));
            else
                return null;
        }

        public async Task<double> GetNumberOfItemsOnList(string key)
        {
            return await DB.ListLengthAsync(key);
        }

        public async Task<bool> KeyExists(string key)
        {
            return await DB.KeyExistsAsync(key);
        }

        public async Task DeleteKey(string key)
        {
            await DB.KeyDeleteAsync(key);
        }

        public async Task<List<string>> GetKeys(string pattern)
        {
            var keys = new List<string>();

            foreach (var key in Server.GetServer(IP, Port).Keys(pattern: pattern))
                keys.Add(key.ToString());

            return keys;
        }

        public void ForceSave()
        {            
            Server.GetServer(IP, Port).SaveAsync(SaveType.ForegroundSave);            
        }

        public async Task<DateTime> GetLastSave()
        {
            return await Server.GetServer(IP, Port).LastSaveAsync();
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
