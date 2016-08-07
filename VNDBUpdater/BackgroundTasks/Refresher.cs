using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.BackgroundTasks
{
    public class Refresher : BackgroundTask
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
                        return "Refresher is currently running. " + _MainScreen.CompletedPendingTasks + " of " + _MainScreen.CurrentPendingTasks + " Visual Novels updated.";
                    case (TaskStatus.RanToCompletion):
                        return "Refresher is finished.";
                    case (TaskStatus.Faulted):
                        return "Error occured while running Refresher. Please check the eventlog.";
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

                EventLogger.LogInformation(nameof(Refresher), "started.");                

                _MainScreen.CurrentPendingTasks = _MainScreen.AllVisualNovels.Count;
                _MainScreen.CompletedPendingTasks = 0;
                _Status = TaskStatus.Running;

                _BackgroundTask = new Task(Refresh, _CancelToken);
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

        private void Refresh()
        {
            try
            {
                EventLogger.LogInformation(nameof(Refresher), "pending Tasks: " + _MainScreen.CurrentPendingTasks.ToString());

                var updatedVNs = new List<VisualNovel>();

                var idSplitter = new VNIDsSplitter(_MainScreen.AllVisualNovels.Select(x => x.Basics.id).ToArray());

                idSplitter.Split();

                if (idSplitter.SplittingNeccessary)
                {
                    for (int round = 0; round < idSplitter.NumberOfRequest; round++)
                    {                        
                        updatedVNs.AddRange(VNDBCommunication.FetchVisualNovels(idSplitter.IDs.Take(round * Constants.MaxVNsPerRequest, Constants.MaxVNsPerRequest).ToList()));
                        _MainScreen.CompletedPendingTasks += Constants.MaxVNsPerRequest;
                        EventLogger.LogInformation(nameof(Refresher), "completed Tasks: " + _MainScreen.CompletedPendingTasks.ToString());                        
                    }
                        
                    if (idSplitter.Remainder > 0)
                    {                        
                        updatedVNs.AddRange(VNDBCommunication.FetchVisualNovels(idSplitter.IDs.Take(idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder).ToList()));
                        _MainScreen.CompletedPendingTasks += idSplitter.Remainder;
                        EventLogger.LogInformation(nameof(Refresher), "completed Tasks: " + _MainScreen.CompletedPendingTasks.ToString());
                    }                        
                }
                else
                {
                    updatedVNs.AddRange(VNDBCommunication.FetchVisualNovels(idSplitter.IDs.ToList()));
                    _MainScreen.CompletedPendingTasks += idSplitter.IDs.Length;
                    EventLogger.LogInformation(nameof(Refresher), "completed Tasks: " + _MainScreen.CompletedPendingTasks.ToString());
                }                    

                foreach (var vn in updatedVNs)
                {
                    var vnToUpdate = _MainScreen.AllVisualNovels.Where(x => x.Basics.id == vn.Basics.id).FirstOrDefault();

                    if (vnToUpdate != null)
                    {
                        vnToUpdate.Basics = vn.Basics;
                        vnToUpdate.Characters = vn.Characters;
                    }

                    RedisCommunication.AddVisualNovelToDB(vnToUpdate);
                }

                Cancel();
                _Status = TaskStatus.RanToCompletion;
                _MainScreen.UpdateStatusText();

                EventLogger.LogInformation(nameof(Refresher), "finished successfully.");                 
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                EventLogger.LogError(nameof(Refresher), ex);
            }
        }
    }
}
