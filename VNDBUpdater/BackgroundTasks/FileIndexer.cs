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

        private FileIndexerSettingsModel Settings;
        private List<VisualNovelModel> _IndexedVisualNovels;

        public FileIndexer(IStatusService StatusService, IVNService VNService, IUserService UserService, ILoggerService LoggerService)
            : base(StatusService, LoggerService)
        {
            _VNService = VNService;
            _UserService = UserService;

            Settings = _UserService.Get().FileIndexerSetting;
        }

        public override void Start(Action<bool> OnTaskCompleted)
        {
            CurrentTask = nameof(FileIndexer);

            Task.Factory.StartNew(() => Indexing(OnTaskCompleted));
        }

        private void Indexing(Action<bool> OnTaskCompleted)
        {
            try
            {
                PercentageCompleted = 0;
                IsRunning = true;
                CurrentStatus = nameof(FileIndexer) + " running. Currently getting all subfolders... Will take a while...";
                
                List<DirectoryInfo> folders = GetFolders();

                bool vnFound = false;
                int successfull = 0;
                var indexedVNs = new List<VisualNovelModel>();

                _TasksToDo = _VNService.GetLocal().Count(x => string.IsNullOrEmpty(x.ExePath));

                _IndexedVisualNovels = _VNService.GetLocal().Where(x => !string.IsNullOrEmpty(x.ExePath)).OrderBy(x => x.Basics.Title).ToList();

                foreach (var vn in _VNService.GetLocal().Where(x => string.IsNullOrEmpty(x.ExePath)).OrderBy(x => x.Basics.Title))
                {
                    vnFound = CheckForIdenticalMatch(vn, folders);

                    if (!vnFound)
                    {
                        vnFound = UseDistanceAlgorithm(vn, folders);
                    }                        
                    if (vnFound)
                    {
                        successfull++;
                        indexedVNs.Add(vn);
                    }
                        
                    vnFound = false;

                    UpdateProgess(1, "Visual Novels have been indexed...");
                }

                _VNService.Add(indexedVNs);

                CurrentStatus = nameof(FileIndexer) + " finished. " + successfull + " of " + _TasksToDo + " were successfully indexed...";
                IsRunning = false;

                OnTaskCompleted(true);
            }
            catch (Exception ex)
            {
                _Logger.Log(ex);

                CurrentStatus = nameof(FileIndexer) + " ran into error.";
                IsRunning = false;

                OnTaskCompleted(false);
            }
        }

        private bool CheckForIdenticalMatch(VisualNovelModel vn, List<DirectoryInfo> folders)
        {
            foreach (var folder in folders)
            {
                if (string.Equals(folder.Name, vn.Basics.Title, StringComparison.OrdinalIgnoreCase))
                {
                    var executeable = GetExecuteable(folder);

                    if (_IndexedVisualNovels.OrderBy(x => x.Basics.Title).Any(x => string.Equals(x.ExePath, executeable, StringComparison.OrdinalIgnoreCase)))
                    {
                        return false;
                    }
                    else
                    {
                        _VNService.SetExePath(vn, executeable);
                        _IndexedVisualNovels.Add(vn);

                        return true;
                    }                    
                }
            }

            return false;
        }

        private bool UseDistanceAlgorithm(VisualNovelModel vn, List<DirectoryInfo> folders)
        {
            for (int maxDistance = 2; maxDistance <= Settings.MaxDeviation; maxDistance++)
            {
                foreach (var folder in folders)
                {
                    if (ComputeLevenshteinDistance(folder.Name.ToLower().Trim(), vn.Basics.Title.ToLower().Trim()) <= maxDistance)
                    {
                        var executeable = GetExecuteable(folder);

                        if (_IndexedVisualNovels.OrderBy(x => x.Basics.Title).Any(x => string.Equals(x.ExePath, executeable, StringComparison.OrdinalIgnoreCase)))
                        {
                            return false;
                        }
                        else
                        {
                            _VNService.SetExePath(vn, executeable);
                            _IndexedVisualNovels.Add(vn);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private List<DirectoryInfo> GetFolders()
        {
            var folders = new List<DirectoryInfo>();

            foreach (var folder in Settings.Folders)
            {
                var rawFolders = Directory.GetDirectories(folder, "*.*", SearchOption.AllDirectories).ToList();

                foreach (var rawFolder in rawFolders)
                {
                    if (new DirectoryInfo(rawFolder).Name.Length < Settings.MinFolderLength || Settings.ExcludedFolders.Any(x => rawFolder.Contains(x)))
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
                if (Settings.ExcludedExes.Any(x => string.Equals(x, file, StringComparison.OrdinalIgnoreCase)))
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
