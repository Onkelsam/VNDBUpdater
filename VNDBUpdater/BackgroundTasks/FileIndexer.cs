using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.BackgroundTasks
{
    public class FileIndexer : TaskBase
    {
        private IVNService _VNService;
        private IUserService _UserService;

        private FileIndexerSettingsModel _Settings;
        private List<VisualNovelModel> _IndexedVisualNovels;

        public FileIndexer(IStatusService StatusService, IVNService VNService, IUserService UserService, ILoggerService LoggerService)
            : base(StatusService, LoggerService)
        {
            _VNService = VNService;
            _UserService = UserService;

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            var user = await _UserService.Get();

            _Settings = user.FileIndexerSetting;
        }

        public override async Task ExecuteTask(Action<bool> OnTaskCompleted)
        {
            _OnTaskCompleted = OnTaskCompleted;

            await Task.Factory.StartNew(async () => await Start(Index));
        }

        private async Task Index()
        {
            List<DirectoryInfo> folders = GetFolders();

            bool vnFound = false;
            int successfull = 0;
            var indexedVNs = new List<VisualNovelModel>();

            IEnumerable<VisualNovelModel> localVNs = await _VNService.GetLocal();

            _TasksToDo = localVNs.Count(x => string.IsNullOrEmpty(x.ExePath));
            _IndexedVisualNovels = localVNs.Where(x => !string.IsNullOrEmpty(x.ExePath)).OrderBy(x => x.Basics.Title).ToList();

            foreach (var vn in localVNs.Where(x => string.IsNullOrEmpty(x.ExePath)).OrderBy(x => x.Basics.Title))
            {
                vnFound = await UseIdenticalMatch(vn, folders);

                if (!vnFound)
                {
                    vnFound = await UseDistanceAlgorithm(vn, folders);
                }

                if (vnFound)
                {
                    successfull++;
                    indexedVNs.Add(vn);
                }

                vnFound = false;

                UpdateProgess(1, "Visual Novels have been indexed...");
            }

            await _VNService.Add(indexedVNs);
        }

        private async Task<bool> UseIdenticalMatch(VisualNovelModel vn, List<DirectoryInfo> folders)
        {
            foreach (var folder in folders)
            {
                if (string.Equals(folder.Name, vn.Basics.Title, StringComparison.OrdinalIgnoreCase))
                {
                    return await CheckForMatch(vn, folder);
                }
            }

            return false;
        }

        private async Task<bool> UseDistanceAlgorithm(VisualNovelModel vn, List<DirectoryInfo> folders)
        {
            for (int maxDistance = 2; maxDistance <= _Settings.MaxDeviation; maxDistance++)
            {
                foreach (var folder in folders)
                {
                    if (ComputeLevenshteinDistance(folder.Name.ToLower().Trim(), vn.Basics.Title.ToLower().Trim()) <= maxDistance)
                    {
                        return await CheckForMatch(vn, folder);
                    }
                }
            }

            return false;
        }

        private async Task<bool> CheckForMatch(VisualNovelModel vn, DirectoryInfo folder)
        {
            var executeable = GetExecuteable(folder);

            if (_IndexedVisualNovels.OrderBy(x => x.Basics.Title).Any(x => string.Equals(x.ExePath, executeable, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            else
            {
                await _VNService.SetExePath(vn, executeable);
                _IndexedVisualNovels.Add(vn);

                return true;
            }
        }

        private List<DirectoryInfo> GetFolders()
        {
            var folders = new List<DirectoryInfo>();

            foreach (var folder in _Settings.Folders)
            {
                var rawFolders = Directory.GetDirectories(folder, "*.*", SearchOption.AllDirectories).ToList();

                foreach (var rawFolder in rawFolders)
                {
                    if (new DirectoryInfo(rawFolder).Name.Length < _Settings.MinFolderLength || _Settings.ExcludedFolders.Any(x => rawFolder.Contains(x)))
                    {
                        continue;
                    }                        

                    folders.Add(new DirectoryInfo(rawFolder));
                }
            }

            return folders;
        }

        private string GetExecuteable(DirectoryInfo folder)
        {
            foreach (var file in Directory.GetFiles(folder.FullName, "*.exe", SearchOption.AllDirectories))
            {
                if (_Settings.ExcludedExes.Any(x => string.Equals(x, file, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                                
                return file;
            }

            return string.Empty;
        }

        private int ComputeLevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }
}
