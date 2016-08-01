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
    public class Synchronizer : BackgroundTask
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
                switch (_Status)
                {
                    case (BackgroundTaskState.Running):
                        return "Synchronizer is currently running. " + _MainScreen.CompletedPendingTasks + " from " + _MainScreen.CurrentPendingTasks + " Visual Novels synced.";
                    case (BackgroundTaskState.Finished):
                        return "Synchronizer is finished.";
                    default:
                        return "Synchronizer not running.";
                }
            }
        }

        public override void Start(MainViewModel MainScreen)
        {
            if (_Status != BackgroundTaskState.Running)
            {
                base.Start(MainScreen);

                VNDBCommunication.Connect();

                if (VNDBCommunication.Status != VNDBCommunicationStatus.LoggedIn)
                {
                    _Status = BackgroundTaskState.NotRunning;
                    Cancel();
                    return;
                }

                _MainScreen.CompletedPendingTasks = 0;
                _Status = BackgroundTaskState.Running;

                _BackgroundTask = new Task(Synchronize, _CancelToken);
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

        private void Synchronize()
        {
            List<VN> VNList = VNDBCommunication.GetVisualNovelListFromVNDB();
            List<Wish> WishList = VNDBCommunication.GetWishListFromVNDB();
            List<Vote> VoteList = VNDBCommunication.GetVoteListFromVNDB();

            _MainScreen.CurrentPendingTasks = VNList.Count + WishList.Count + VoteList.Count;

            SynchronizeVNs(VNList);
            SynchronizeWishes(WishList);
            SynchronizeVotes(VoteList);

            _Status = BackgroundTaskState.Finished;
            _MainScreen.UpdateStatusText();
            Cancel();
        }

        private void SynchronizeVNs(List<VN> VNsToSynchronize)
        {
            var VNsToAdd = new List<VN>();

            foreach (var vn in VNsToSynchronize)
            {               
                if (VisualNovelHelper.VisualNovelExists(vn.vn))
                {
                    _MainScreen.CompletedPendingTasks++;

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

            GetVNs(VNsToAdd);
        }

        private void SynchronizeVotes(List<Vote> VotesToSynchronize)
        {
            foreach (var vote in VotesToSynchronize)
            {                
                if (VisualNovelHelper.VisualNovelExists(vote.vn))
                {
                    _MainScreen.CompletedPendingTasks++;

                    VisualNovel LocalVN = VisualNovelHelper.GetVisualNovel(vote.vn);

                    if (LocalVN.Score != (vote.vote / 10))
                    {
                        LocalVN.Score = (vote.vote / 10);
                        RedisCommunication.AddVisualNovelToDB(LocalVN);
                    }
                }
            }
        }

        private void GetVNs(List<VN> VNs)
        {
            int[] ids = VNs.Select(x => x.vn).ToArray();

            if (VNs.Count >= VNDBCommunication.MAXVNSPERREQUEST)
            {                
                int numberOfRequests = (ids.Length / VNDBCommunication.MAXVNSPERREQUEST);
                int remainder = ids.Length - (numberOfRequests * VNDBCommunication.MAXVNSPERREQUEST);

                for (int round = 0; round < numberOfRequests; round++)
                    AddVNs(ids.Take(round * VNDBCommunication.MAXVNSPERREQUEST, VNDBCommunication.MAXVNSPERREQUEST), VNs);

                if (remainder > 0)
                    AddVNs(ids.Take(ids.Length - remainder, remainder), VNs);
            }
            else
                AddVNs(ids, VNs);
        }

        private void AddVNs(int[] ids, List<VN> VNs)
        {
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
