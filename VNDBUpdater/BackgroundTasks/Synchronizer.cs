using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.BackgroundTasks
{
    public class Synchronizer : TaskBase
    {
        private IVNService _VNService;

        public Synchronizer(IStatusService StatusService, IVNService VNService, ILoggerService LoggerService)
            : base(StatusService, LoggerService)
        {
            _VNService = VNService;
        }

        public override void Start(Action<bool> OnTaskCompleted)
        {
            CurrentTask = nameof(Synchronizer);

            Task.Factory.StartNew(() => Synchronize(OnTaskCompleted));
        }

        private void Synchronize(Action<bool> OnTaskCompleted)
        {
            try
            {
                PercentageCompleted = 0;
                IsRunning = true;
                CurrentStatus = nameof(Synchronizer) + " running.";

                Stopwatch sw = new Stopwatch();
                sw.Start();

                List<VN> VNList = _VNService.GetVNList().ToList();

                _Logger.Log("Get VN List: " + sw.ElapsedMilliseconds.ToString());
                sw.Restart();

                List<Vote> VoteList = _VNService.GetVoteList().ToList();

                _Logger.Log("Get Vote List: " + sw.ElapsedMilliseconds.ToString());
                sw.Restart();

                _TasksToDo = VNList.Count + VoteList.Count;

                CurrentStatus = _TasksDone + " Visual Novels of " + _TasksToDo + " synchronized";                

                SynchronizeVNs(VNList);

                _Logger.Log("Synchronize VNs: " + sw.ElapsedMilliseconds.ToString());
                sw.Restart();

                SynchronizeVotes(VoteList);

                _Logger.Log("Synchronize Votes: " + sw.ElapsedMilliseconds.ToString());
                sw.Restart();

                CurrentStatus = nameof(Synchronizer) + " finished.";
                IsRunning = false;

                OnTaskCompleted(true);
            }
            catch (Exception ex)
            {
                _Logger.Log(ex);

                CurrentStatus = nameof(Synchronize) + " ran into error.";
                IsRunning = false;

                OnTaskCompleted(false);
            }
        }

        private void SynchronizeVNs(List<VN> VNsToSynchronize)
        {
            var VNsToAdd = new List<VN>();

            foreach (var vn in VNsToSynchronize)
            {
                if (_VNService.VNExists(vn.vn))
                {
                    VisualNovelModel LocalVN = _VNService.GetLocal(vn.vn);

                    if (LocalVN.Category != (VisualNovelModel.VisualNovelCatergory)vn.status)
                    {
                        LocalVN.Category = (VisualNovelModel.VisualNovelCatergory)vn.status;
                        _VNService.Add(LocalVN);
                    }

                    UpdateProgess(1, "Visual Novels have been synchronized...");
                }
                else
                {
                    VNsToAdd.Add(vn);
                }
            }

            if (VNsToAdd.Any())
            {
                GetVNs(VNsToAdd);
            }
        }

        private void SynchronizeVotes(List<Vote> VotesToSynchronize)
        {
            foreach (var vote in VotesToSynchronize)
            {
                if (_VNService.VNExists(vote.vn))
                {
                    VisualNovelModel LocalVN = _VNService.GetLocal(vote.vn);

                    if (LocalVN.Score != vote.vote)
                    {
                        LocalVN.Score = vote.vote;
                        _VNService.Add(LocalVN);
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
                for (int round = 0; round < idSplitter.NumberOfRequests; round++)
                {
                    AddVNs(Take(idSplitter.IDs, round * idSplitter.MaxVNsPerRequest, idSplitter.MaxVNsPerRequest), VNs);
                }

                if (idSplitter.Remainder > 0)
                {
                    AddVNs(Take(idSplitter.IDs, idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder), VNs);
                }
            }
            else
            {
                AddVNs(idSplitter.IDs, VNs);
            }
        }

        private void AddVNs(int[] ids, List<VN> VNs)
        {
            List<VisualNovelModel> newVisualNovels = _VNService.Get(ids.ToList()).ToList();

            newVisualNovels.ForEach(x => x.Category = (VisualNovelModel.VisualNovelCatergory)VNs.First(y => y.vn == x.Basics.ID).status);
            newVisualNovels.ForEach(x => x.Score = 0);

            _VNService.Add(newVisualNovels);

            UpdateProgess(ids.Length, " Visual Novels have been synchronized...");
        }
    }
}
