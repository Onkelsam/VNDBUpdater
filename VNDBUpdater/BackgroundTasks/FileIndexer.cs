using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Data;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.BackgroundTasks
{
    public class FileIndexer : BackgroundTask
    {
        private static BackgroundTaskState _Status = BackgroundTaskState.NotRunning;

        public static BackgroundTaskState Status
        {
            get { return _Status; }
        }

        public static string StatusString
        {
            get
            {
                switch(_Status)
                {
                    case (BackgroundTaskState.Running):
                        return "Fileindexer is currently running. " + _MainScreen.CompletedPendingTasks + " of " + _MainScreen.CurrentPendingTasks + " Visual Novels indexed.";
                    case (BackgroundTaskState.Finished):
                        return "Fileindexer is finished. " + _MainScreen.AllVisualNovels.Count(x => x.ExePath == null || x.ExePath == "") + " could not be indexed.";
                    default:
                        return "Fileindexer not running.";
                }
            }
        }

        public override void Start(MainViewModel MainScreen)
        {
            if (_Status != BackgroundTaskState.Running)
            {
                base.Start(MainScreen);

                Trace.TraceInformation("Fileindexer started.");

                _MainScreen.CurrentPendingTasks = _MainScreen.AllVisualNovels.Count(x => x.ExePath == null || x.ExePath == "");
                _MainScreen.CompletedPendingTasks = 0;
                _Status = BackgroundTaskState.Running;

                _BackgroundTask = new Task(Indexing, _CancelToken);
                _BackgroundTask.Start();
            }
        }

        public static void Cancel()
        {
            if (_Status == BackgroundTaskState.Running)
            {
                _CancelTokenSource.Cancel();
            }
        }

        private void Indexing()
        {
            try
            {
                Trace.TraceInformation("FileIndexer pending Tasks: " + _MainScreen.CurrentPendingTasks.ToString());

                foreach (var vn in _MainScreen.AllVisualNovels.Where(x => x.ExePath == null || x.ExePath == ""))
                {
                    vn.CrawlExePath();
                    _MainScreen.CompletedPendingTasks++;
                    Trace.TraceInformation("FileIndexer completed Tasks: " + _MainScreen.CompletedPendingTasks.ToString());
                }

                _Status = BackgroundTaskState.Finished;
                _MainScreen.UpdateStatusText();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error caught in FileIndexer: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.GetType().Name + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
