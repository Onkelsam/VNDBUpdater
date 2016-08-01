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
            foreach (var vn in _MainScreen.AllVisualNovels.Where(x => x.ExePath == null || x.ExePath == ""))
            {
                vn.CrawlExePath();
                _MainScreen.CompletedPendingTasks++;
            }

            _Status = BackgroundTaskState.Finished;
            _MainScreen.UpdateStatusText();
        }
    }
}
