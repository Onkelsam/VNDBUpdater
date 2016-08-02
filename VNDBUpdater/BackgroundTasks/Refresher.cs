using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.ViewModels;
using VNUpdater.Helper;

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

                Trace.TraceInformation("Refresher started.");

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
                Trace.TraceInformation("Refresher pending Tasks: " + _MainScreen.CurrentPendingTasks.ToString());

                var updatedVNs = new List<VisualNovel>();

                var idSplitter = new VNIDsSplitter(_MainScreen.AllVisualNovels.Select(x => x.Basics.id).ToArray());

                idSplitter.Split();

                if (idSplitter.SplittingNeccessary)
                {
                    for (int round = 0; round < idSplitter.NumberOfRequest; round++)
                    {                        
                        updatedVNs.AddRange(VNDBCommunication.FetchVisualNovels(idSplitter.IDs.Take(round * VNDBCommunication.MAXVNSPERREQUEST, VNDBCommunication.MAXVNSPERREQUEST).ToList()));
                        _MainScreen.CompletedPendingTasks += VNDBCommunication.MAXVNSPERREQUEST;
                    }
                        
                    if (idSplitter.Remainder > 0)
                    {                        
                        updatedVNs.AddRange(VNDBCommunication.FetchVisualNovels(idSplitter.IDs.Take(idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder).ToList()));
                        _MainScreen.CompletedPendingTasks += idSplitter.Remainder;
                    }                        
                }
                else
                {
                    updatedVNs.AddRange(VNDBCommunication.FetchVisualNovels(idSplitter.IDs.ToList()));
                    _MainScreen.CompletedPendingTasks += idSplitter.IDs.Length;
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
                _MainScreen.UpdateVisualNovelGrid();
                _MainScreen.UpdateStatusText();

                Trace.TraceInformation("Refresher finished successfully.");                
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                Trace.TraceError("Error caught in Refresher: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.GetType().Name + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
