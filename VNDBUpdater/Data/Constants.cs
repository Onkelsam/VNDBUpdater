using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace VNDBUpdater.Data
{
    public static class Constants
    {
        public const string RedisIP = "localhost";
        public const int RedisPort = 6379;
        public const string RedisExe = "redis-server.exe";
        public const string RedisConfig = "redis.windows.conf";
        public const string DatabaseName = "LocalVNStorage.rdb";
        public const string PathToDatabase = @"\Resources\";
        public const string BackupDatabaseName = "LocalVNStorage_Backup.rdb";

        public const string VisualNovelKey = "VisualNovel_";
        public const string FilterKey = "Filter_";
        public const string UserKey = "User";

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

        public const string TaskRunning = " currently running. ";
        public const string TaskFinished = " finished successfully.";
        public const string TaskFaulted = " faulted. Please check the " + Constants.EventlogFileName + " found in the program directory.";
        public const string TaskStarted = " started.";
        public const string TasksPending = " pending Tasks: ";
        public const string TasksCompleted = " completed Tasks: ";

        public const string ConnectionEstablished = "Connection established successfully.";
        public const string ConnectionErrorHandled = "Error was handled. Trying reconnect. Current tries: ";
        public const string ConnectionErrorNotHandled = "Error could not be handled. Maximal connection tries reached. ";
        public const string ConnectionAborted = "Disconnected successfully";
        public const string ObjectDisposed = "Disposed successfully";

        public const string LoggedIn = "Currently logged in as '";
        public const string NotLoggedIn = "Currently not logged in.";
        public const string VNDBConnectionFailedThrottling = "Connecting to VNDB failed because of throttling error. Trying reconnect.";
        public const string VNDBConnectionFailedAuthentication = "Connecting to VNDB failed because of authentication error. Abort connection procedure.";
        public const string VNDBConnectionFailedUnknownError = "Connecting to VNDB failed because of unknown error. Abort connection procedure.";

        public const string VNDBConnectionThrottlingError = "Throttled error by VNDB received. Waiting: ";
        public const string VNDBConnectionAuthenticationError = "Authentication error by VNDB received.";
        public const string VNDBConnectionUnknownErrorReceived = "Unknown error by VNDB received.";

        public const string InputValidationStringEmpty = "The input cannot be empty!";
        public const string InputValidationFalseInput = "Only numbers allowed! Split IDs by ','.";

        public const string VNDBVNLink = "https://vndb.org/v";
        public const string GoogleLink = "https://www.google.de/#q=";
        public const string GitHubReleaseLink = "https://github.com/Onkelsam/VNDBUpdater/releases";

        public const string NoImage = @"\Resources\NoImage.png";
        public const string NSFWImage = @"\Resources\NSFWImage.png";

        public static readonly string DirectoryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static readonly Dictionary<int?, string> VNLengthMapper = new Dictionary<int?, string>()
        {
            { 0, "Unknown" },
            { 1, "Very short (< 2 hours)" },
            { 2, "Short (2 - 10 hours)" },
            { 3, "Medium (10 - 30 hours)" },
            { 4, "Long (30 - 50 hours)" },
            { 5, "Very long (> 50 hours)" }
        };

        public static readonly Dictionary<string, string> RelationsMapper = new Dictionary<string, string>()
        {
            { "seq", "Sequel" },
            { "set", "Same Setting" },
            { "preq", "Prequel" },
            { "fan", "Fandisc" },
            { "ser", "Same Series" },
            { "orig", "Original Game" },
            { "alt", "Alternative Version" },
            { "char", "Shares Characters" },
            { "side", "Side Story" },
            { "par", "Parent Story" }
        };

        public static readonly Dictionary<TagCategory, string> TagCategoryMapper = new Dictionary<TagCategory, string>()
        {
            { TagCategory.All, "All" },
            { TagCategory.cont, "Content" },
            { TagCategory.ero, "Sexual Content" },
            { TagCategory.tech, "Technical Content" },
        };

        public static readonly List<int> PossibleScores = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public static readonly List<string> ExcludedExeFileNames = new List<string>() { "unins", "エンジン設定" };
    }
}
