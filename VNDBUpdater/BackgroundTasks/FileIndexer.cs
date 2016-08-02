﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
                        return "Fileindexer is currently running. " + _MainScreen.CompletedPendingTasks + " of " + _MainScreen.CurrentPendingTasks + " Visual Novels indexed.";
                    case (TaskStatus.RanToCompletion):
                        return "Fileindexer is finished. " + _MainScreen.AllVisualNovels.Count(x => x.ExePath == null || x.ExePath == "") + " could not be indexed.";
                    case (TaskStatus.Faulted):
                        return "Error occured while running Fileindexer. Please check the eventlog.";
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

                Trace.TraceInformation("Fileindexer started.");

                _MainScreen.CurrentPendingTasks = _MainScreen.AllVisualNovels.Count(x => x.ExePath == null || x.ExePath == "");
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
                Trace.TraceInformation("FileIndexer pending Tasks: " + _MainScreen.CurrentPendingTasks.ToString());

                foreach (var vn in _MainScreen.AllVisualNovels.Where(x => x.ExePath == null || x.ExePath == ""))
                {
                    vn.CrawlExePath();
                    _MainScreen.CompletedPendingTasks++;
                    Trace.TraceInformation("FileIndexer completed Tasks: " + _MainScreen.CompletedPendingTasks.ToString());
                }

                Cancel();
                _Status = TaskStatus.RanToCompletion;
                _MainScreen.UpdateStatusText();

                Trace.TraceInformation("Fileindexer finished successfully.");
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                Trace.TraceError("Error caught in FileIndexer: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.GetType().Name + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
