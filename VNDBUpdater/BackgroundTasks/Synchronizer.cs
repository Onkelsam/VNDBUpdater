using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.ViewModels;
using VNUpdater.Helper;

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
                        return "Synchronizer is currently running. " + _MainScreen.CompletedPendingTasks + " from " + _MainScreen.CurrentPendingTasks + " Visual Novels synced.";
                    case (TaskStatus.RanToCompletion):
                        return "Synchronizer is finished.";
                    case (TaskStatus.Faulted):
                        return "Error occured while running Synchronizer. Please check the eventlog.";
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

                Trace.TraceInformation("Synchronizer started.");

                VNDBCommunication.Connect();

                if (VNDBCommunication.Status != VNDBCommunicationStatus.LoggedIn)
                {
                    _Status = TaskStatus.WaitingToRun;
                    Cancel();
                    return;
                }

                _MainScreen.CompletedPendingTasks = 0;
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
                List<Wish> WishList = VNDBCommunication.GetWishListFromVNDB();
                List<Vote> VoteList = VNDBCommunication.GetVoteListFromVNDB();

                int count = VNList.Count + WishList.Count + VoteList.Count;

                Trace.TraceInformation("Synchronizer pending Tasks: " + count.ToString());

                _MainScreen.CurrentPendingTasks = count;

                SynchronizeVNs(VNList);
                SynchronizeWishes(WishList);
                SynchronizeVotes(VoteList);

                Cancel();
                _Status = TaskStatus.RanToCompletion;
                _MainScreen.UpdateStatusText();                

                Trace.TraceInformation("Synchronizer finished successfully.");
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                Trace.TraceError("Error caught in Synchronizer: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.GetType().Name + Environment.NewLine + ex.StackTrace);
            }

        }

        private void SynchronizeVNs(List<VN> VNsToSynchronize)
        {
            var VNsToAdd = new List<VN>();

            foreach (var vn in VNsToSynchronize)
            {                
                if (VisualNovelHelper.VisualNovelExists(vn.vn))
                {
                    _MainScreen.CompletedPendingTasks++;
                    Trace.TraceInformation("Synchronizer synced VNs: " + _MainScreen.CompletedPendingTasks.ToString());

                    VisualNovel LocalVN = VisualNovelHelper.GetVisualNovel(vn.vn);

                    if (LocalVN.Category != (VisualNovelCatergory)vn.status)
                    {
                        LocalVN.Category = (VisualNovelCatergory)vn.status;
                        RedisCommunication.AddVisualNovelToDB(LocalVN);
                    }
                }
                else
                    VNsToAdd.Add(vn);
            }

            if (VNsToAdd.Any())
                GetVNs(VNsToAdd);
        }

        private void SynchronizeWishes(List<Wish> WishesToSynchronize)
        {
            var VNsToAdd = new List<VN>();

            foreach (var wish in WishesToSynchronize)
            {                
                if (VisualNovelHelper.VisualNovelExists(wish.vn))
                {
                    _MainScreen.CompletedPendingTasks++;
                    Trace.TraceInformation("Synchronizer synced Wishes: " + _MainScreen.CompletedPendingTasks.ToString());

                    VisualNovel LocalVN = VisualNovelHelper.GetVisualNovel(wish.vn);

                    if (LocalVN.Category != VisualNovelCatergory.Wish)
                    {
                        LocalVN.Category = VisualNovelCatergory.Wish;
                        RedisCommunication.AddVisualNovelToDB(LocalVN);
                    }
                }
                else
                    VNsToAdd.Add(new VN { vn = wish.vn, status = (int)VisualNovelCatergory.Wish });
            }

            if (VNsToAdd.Any())
                GetVNs(VNsToAdd);
        }

        private void SynchronizeVotes(List<Vote> VotesToSynchronize)
        {
            foreach (var vote in VotesToSynchronize)
            {                
                if (VisualNovelHelper.VisualNovelExists(vote.vn))
                {
                    _MainScreen.CompletedPendingTasks++;
                    Trace.TraceInformation("Synchronizer synced Votes: " + _MainScreen.CompletedPendingTasks.ToString());

                    VisualNovel LocalVN = VisualNovelHelper.GetVisualNovel(vote.vn);

                    if (LocalVN.Score != vote.vote)
                    {
                        LocalVN.Score = vote.vote;
                        RedisCommunication.AddVisualNovelToDB(LocalVN);
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
                    AddVNs(idSplitter.IDs.Take(round * VNDBCommunication.MAXVNSPERREQUEST, VNDBCommunication.MAXVNSPERREQUEST), VNs);

                if (idSplitter.Remainder > 0)
                    AddVNs(idSplitter.IDs.Take(idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder), VNs);
            }
            else
                AddVNs(idSplitter.IDs, VNs);
        }

        private void AddVNs(int[] ids, List<VN> VNs)
        {
            Trace.TraceInformation("Added VNs: " + ids.Length.ToString());

            foreach (var vn in VNDBCommunication.FetchVisualNovels(ids.ToList()))
            {
                var VNFromVNDB = VNs.Where(x => x.vn == vn.Basics.id).First();

                vn.Category = (VisualNovelCatergory)VNFromVNDB.status;
                vn.Score = 0;
                RedisCommunication.AddVisualNovelToDB(vn);
                vn.CrawlExePath();

                _MainScreen.AddOnUI(_MainScreen.AllVisualNovels, vn);

                if (vn.Category == (VisualNovelCatergory)_MainScreen.SelectedVisualNovelTab)
                    _MainScreen.AddOnUI(_MainScreen.VisualNovelsInGrid, vn);

                _MainScreen.CompletedPendingTasks++;
                _MainScreen.UpdateStatusText();
            }            
        }
    }
}
