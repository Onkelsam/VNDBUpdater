using System.Collections.Generic;

namespace VNDBUpdater.Data
{
    public static class Constants
    {
        public const string RedisIP = "localhost";
        public const int RedisPort = 6379;
        public const string RedisExe = "redis-server.exe";
        public const string RedisConfig = "redis.windows.conf";
        public const string DatabaseName = "LocalVNStorage.rdb";
        public const string PathToDatabase = @"Resources\";
        public const string BackupDatabaseName = "LocalVNStorage_Backup.rdb";        

        public const int MaxVNsPerRequest = 25;

        public const string TagsJsonFileName = "tags.json";
        public const string TraitsJsonFileName = "traits.json";

        public const string TagsZipFileName = "tags.json.gz";
        public const string TagsDownloadLink = "http://vndb.org/api/tags.json.gz";

        public const string TraitsZipFileName = "traits.json.gz";
        public const string TraitsDownloadLink = "http://vndb.org/api/traits.json.gz";

        public const string WalkthroughFileName = "walkthrough.txt";

        public const int MaxConnectionTries = 4;
        public const int MaxDistanceBetweenStringsForIndexer = 5;

        public const string EventlogFileName = "Eventlog.txt";

        public static readonly Dictionary<int?, string> VNLengthMapper = new Dictionary<int?, string>()
        {
            { 0, "Unknown" },
            { 1, "Very short (< 2 hours)" },
            { 2, "Short (2 - 10 hours)" },
            { 3, "Medium (10 - 30 hours)" },
            { 4, "Long (30 - 50 hours)" },
            { 5, "Very long (> 50 hours)" }
        };

        public static readonly List<int> PossibleScores = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public static readonly List<string> ExcludedExeFileNames = new List<string>() { "unins", "エンジン設定" };
    }
}
