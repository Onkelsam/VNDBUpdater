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
    }
}
