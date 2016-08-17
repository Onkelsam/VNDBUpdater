using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.Models.Internal;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.BackgroundTasks
{
    public class FileIndexer : BackgroundTask
    {
        private FileIndexerViewModel _FileIndexerView;

        private static TaskStatus _Status = TaskStatus.WaitingToRun;
        private FileIndexerSettings Settings = UserHelper.CurrentUser.FileIndexerSetting;

        public static TaskStatus Status
        {
            get { return _Status; }
        }

        public static string StatusString
        {
            get
            {
                switch(_Status)
                {
                    case (TaskStatus.Running):
                        return nameof(FileIndexer) +  Constants.TaskRunning + _MainScreen.CompletedPendingTasks + " of " + _MainScreen.CurrentPendingTasks + " Visual Novels indexed.";
                    case (TaskStatus.RanToCompletion):
                        return nameof(FileIndexer) + Constants.TaskFinished + " " + LocalVisualNovelHelper.LocalVisualNovels.Count(x => x.ExePath == null || x.ExePath == "") + " could not be indexed.";
                    case (TaskStatus.Faulted):
                        return nameof(FileIndexer) + Constants.TaskFaulted;
                    default:
                        return string.Empty;
                }
            }
        }

        public void Start(MainViewModel MainScreen, FileIndexerViewModel IndexerView)
        {
            if (_Status != TaskStatus.Running)
            {
                base.Start(MainScreen);

                _FileIndexerView = IndexerView;

                EventLogger.LogInformation(nameof(FileIndexer) + ":" + nameof(Start), Constants.TaskStarted);

                _MainScreen.CurrentPendingTasks = LocalVisualNovelHelper.LocalVisualNovels.Count(x => x.ExePath == null || x.ExePath == "");
                _MainScreen.CompletedPendingTasks = 0;
                _Status = TaskStatus.Running;

                _BackgroundTask = new Task(Indexing, _CancelToken);
                _BackgroundTask.Start();
            }
        }

        public static void Cancel()
        {
            if (_Status == TaskStatus.Running)
            {
                _CancelTokenSource.Cancel();
            }
        }

        private void Indexing()
        {
            try
            {
                EventLogger.LogInformation(nameof(FileIndexer) + ":" + nameof(Indexing), Constants.TasksPending + _MainScreen.CurrentPendingTasks.ToString());

                List<DirectoryInfo> folders = GetFolders();

                bool vnFound = false;
                var indexedVNs = new List<VisualNovel>();

                foreach (var vn in LocalVisualNovelHelper.LocalVisualNovels.Where(x => x.ExePath == null || x.ExePath == ""))
                {
                    vnFound = CheckForIdenticalMatch(vn, folders);

                    if (!vnFound)
                        vnFound = UseDistanceAlgorithm(vn, folders);

                    if (vnFound)
                        indexedVNs.Add(vn);

                    _MainScreen.CompletedPendingTasks++;
                    vnFound = false;
                }

                LocalVisualNovelHelper.AddVisualNovels(indexedVNs);
                
                _Status = TaskStatus.RanToCompletion;
                _FileIndexerView.OnPropertyChanged(nameof(_FileIndexerView.NonIndexedVisualNovels));
                _MainScreen.UpdateStatusText();

                EventLogger.LogInformation(nameof(FileIndexer) + ":" + nameof(Indexing), Constants.TaskFinished);
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                EventLogger.LogError(nameof(FileIndexer) + ":" + nameof(Indexing), ex);
            }
        }

        private bool CheckForIdenticalMatch(VisualNovel vn, List<DirectoryInfo> folders)
        {
            foreach (var folder in folders)
            {
                if (folder.Name.ToLower().Trim() == vn.Basics.VNDBInformation.title.ToLower().Trim())
                {
                    vn.ExePath = GetExecuteable(folder);
                    return true;
                }
            }

            return false;
        }

        private bool UseDistanceAlgorithm(VisualNovel vn, List<DirectoryInfo> folders)
        {
            for (int maxDistance = 2; maxDistance <= Settings.MaxDeviation; maxDistance++)
            {
                foreach (var folder in folders)
                {
                    if (ComputeLevenshteinDistance(folder.Name.ToLower().Trim(), vn.Basics.VNDBInformation.title.ToLower().Trim()) <= maxDistance)
                    {
                        vn.ExePath = GetExecuteable(folder);
                        return true;
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
                        continue;

                    folders.Add(new DirectoryInfo(rawFolder));
                }
            }

            return folders;
        }

        private string GetExecuteable(DirectoryInfo folder)
        {
            foreach (var file in Directory.GetFiles(folder.FullName, "*.exe", SearchOption.AllDirectories))
            {
                if (Settings.ExcludedExes.Any(x => file.Contains(x.ToLower())))
                    continue;
            
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
