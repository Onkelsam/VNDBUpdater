using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.BackgroundTasks
{
    public class FileIndexer : BackgroundTask
    {
        private static TaskStatus _Status = TaskStatus.WaitingToRun;

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
                        return nameof(FileIndexer) + Constants.TaskFinished + LocalVisualNovelHelper.LocalVisualNovels.Count(x => x.ExePath == null || x.ExePath == "") + " could not be indexed.";
                    case (TaskStatus.Faulted):
                        return nameof(FileIndexer) + Constants.TaskFaulted;
                    default:
                        return string.Empty;
                }
            }
        }

        public override void Start(MainViewModel MainScreen)
        {
            if (_Status != TaskStatus.Running)
            {
                base.Start(MainScreen);

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

                var crawledVNs = new List<VisualNovel>();

                foreach (var vn in LocalVisualNovelHelper.LocalVisualNovels.Where(x => x.ExePath == null || x.ExePath == ""))
                {
                    vn.CrawlExePath();
                    crawledVNs.Add(vn);                 
                    _MainScreen.CompletedPendingTasks++;
                    EventLogger.LogInformation(nameof(FileIndexer) + ":" + nameof(Indexing), Constants.TasksCompleted + _MainScreen.CompletedPendingTasks.ToString());
                }

                LocalVisualNovelHelper.AddVisualNovels(crawledVNs);

                _Status = TaskStatus.RanToCompletion;
                _MainScreen.UpdateStatusText();

                EventLogger.LogInformation(nameof(FileIndexer) + ":" + nameof(Indexing), Constants.TaskFinished);
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                EventLogger.LogError(nameof(FileIndexer) + ":" + nameof(Indexing), ex);
            }
        }
    }
}
