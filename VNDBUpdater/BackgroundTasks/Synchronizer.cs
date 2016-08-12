using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.BackgroundTasks
{
    public class Synchronizer : BackgroundTask
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
                switch (_Status)
                {
                    case (TaskStatus.Running):
                        return nameof(Synchronizer) + Constants.TaskRunning + _MainScreen.CompletedPendingTasks + " of " + _MainScreen.CurrentPendingTasks + " Visual Novels synced.";
                    case (TaskStatus.RanToCompletion):
                        return nameof(Synchronizer) + Constants.TaskFinished;
                    case (TaskStatus.Faulted):
                        return nameof(Synchronizer) + Constants.TaskFaulted;
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

                EventLogger.LogInformation(nameof(Synchronizer) + ":" + nameof(Start), Constants.TaskStarted);

                VNDBCommunication.Connect();

                if (VNDBCommunication.Status != VNDBCommunicationStatus.LoggedIn)
                {
                    _Status = TaskStatus.WaitingToRun;
                    Cancel();
                    return;
                }

                _Status = TaskStatus.Running;

                _BackgroundTask = new Task(Synchronize, _CancelToken);
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

        private void Synchronize()
        {
            try
            {
                List<VN> VNList = VNDBCommunication.GetVisualNovelListFromVNDB();
                List<Vote> VoteList = VNDBCommunication.GetVoteListFromVNDB();

                int count = VNList.Count + VoteList.Count;

                EventLogger.LogInformation(nameof(Synchronizer) + ":" + nameof(Synchronize), Constants.TasksPending + count.ToString());

                _MainScreen.CompletedPendingTasks = 0;
                _MainScreen.CurrentPendingTasks = count;

                SynchronizeVNs(VNList);
                SynchronizeVotes(VoteList);

                _Status = TaskStatus.RanToCompletion;
                _MainScreen.UpdateStatusText();
                _MainScreen.UpdateVisualNovelGrid();

                EventLogger.LogInformation(nameof(Synchronizer) + ":" + nameof(Synchronize), Constants.TaskFinished);
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                EventLogger.LogError(nameof(Synchronizer), ex);
            }        
        }

        private void SynchronizeVNs(List<VN> VNsToSynchronize)
        {
            var VNsToAdd = new List<VN>();

            foreach (var vn in VNsToSynchronize)
            {                
                if (LocalVisualNovelHelper.VisualNovelExists(vn.vn))
                {
                    _MainScreen.CompletedPendingTasks++;
                    EventLogger.LogInformation(nameof(Synchronizer) + ":" + nameof(SynchronizeVNs), Constants.TasksCompleted + _MainScreen.CompletedPendingTasks.ToString());

                    VisualNovel LocalVN = LocalVisualNovelHelper.GetVisualNovel(vn.vn);

                    if (LocalVN.Category != (VisualNovelCatergory)vn.status)
                    {
                        LocalVN.Category = (VisualNovelCatergory)vn.status;
                        LocalVisualNovelHelper.AddVisualNovel(LocalVN);
                    }
                }
                else
                    VNsToAdd.Add(vn);
            }

            if (VNsToAdd.Any())
                GetVNs(VNsToAdd);
        }

        /*private void SynchronizeWishes(List<Wish> WishesToSynchronize)
        {
            var VNsToAdd = new List<VN>();

            foreach (var wish in WishesToSynchronize)
            {                
                if (LocalVisualNovelHelper.VisualNovelExists(wish.vn))
                {
                    _MainScreen.CompletedPendingTasks++;
                    EventLogger.LogInformation(nameof(Synchronizer), "completed Tasks: " + _MainScreen.CompletedPendingTasks.ToString());

                    VisualNovel LocalVN = LocalVisualNovelHelper.GetVisualNovel(wish.vn);

                    if (LocalVN.Category != VisualNovelCatergory.Wish)
                    {
                        LocalVN.Category = VisualNovelCatergory.Wish;
                        LocalVisualNovelHelper.AddVisualNovel(LocalVN);
                    }
                }
                else
                    VNsToAdd.Add(new VN { vn = wish.vn, status = (int)VisualNovelCatergory.Wish });
            }

            if (VNsToAdd.Any())
                GetVNs(VNsToAdd);
        }*/

        private void SynchronizeVotes(List<Vote> VotesToSynchronize)
        {
            foreach (var vote in VotesToSynchronize)
            {                
                if (LocalVisualNovelHelper.VisualNovelExists(vote.vn))
                {
                    _MainScreen.CompletedPendingTasks++;
                    EventLogger.LogInformation(nameof(Synchronizer) + ":" + nameof(SynchronizeVotes), Constants.TasksCompleted + _MainScreen.CompletedPendingTasks.ToString());

                    VisualNovel LocalVN = LocalVisualNovelHelper.GetVisualNovel(vote.vn);

                    if (LocalVN.Score != vote.vote)
                    {
                        LocalVN.Score = vote.vote;
                        LocalVisualNovelHelper.AddVisualNovel(LocalVN);
                    }
                }
            }
        }

        private void GetVNs(List<VN> VNs)
        {
            var idSplitter = new VNIDsSplitter(VNs.Select(x => x.vn).ToArray());

            idSplitter.Split();

            if (idSplitter.SplittingNeccessary)
            {
                for (int round = 0; round < idSplitter.NumberOfRequest; round++)
                    AddVNs(idSplitter.IDs.Take(round * Constants.MaxVNsPerRequest, Constants.MaxVNsPerRequest), VNs);

                if (idSplitter.Remainder > 0)
                    AddVNs(idSplitter.IDs.Take(idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder), VNs);
            }
            else
                AddVNs(idSplitter.IDs, VNs);
        }

        private void AddVNs(int[] ids, List<VN> VNs)
        {           
            foreach (var vn in VNDBCommunication.FetchVisualNovels(ids.ToList()))
            {
                var VNFromVNDB = VNs.Where(x => x.vn == vn.Basics.VNDBInformation.id).First();

                vn.Category = (VisualNovelCatergory)VNFromVNDB.status;
                vn.Score = 0;                
                vn.CrawlExePath();

                LocalVisualNovelHelper.AddVisualNovel(vn);

                _MainScreen.CompletedPendingTasks++;
                _MainScreen.UpdateStatusText();               
            }

            EventLogger.LogInformation(nameof(Synchronizer) + ":" + nameof(AddVNs), Constants.TasksCompleted + _MainScreen.CompletedPendingTasks.ToString());
        }
    }
}
