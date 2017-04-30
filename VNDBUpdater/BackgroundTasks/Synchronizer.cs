using System;
using System.Collections.Generic;
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

        private async Task Synchronize(Action<bool> OnTaskCompleted)
        {
            try
            {
                PercentageCompleted = 0;
                IsRunning = true;
                CurrentStatus = nameof(Synchronizer) + " running.";

                var vns = await _VNService.GetVNList();
                var votes = await _VNService.GetVoteList();

                var VNList = new List<VN>(vns);
                var VoteList = new List<Vote>(votes);

                _TasksToDo = VNList.Count + VoteList.Count;

                CurrentStatus = _TasksDone + " Visual Novels of " + _TasksToDo + " synchronized";                

                await SynchronizeVNs(VNList);
                await SynchronizeVotes(VoteList);

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

        private async Task SynchronizeVNs(List<VN> VNsToSynchronize)
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
                await GetVNs(VNsToAdd);
            }
        }

        private async Task SynchronizeVotes(List<Vote> VotesToSynchronize)
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

        private async Task GetVNs(List<VN> VNs)
        {
            var idSplitter = new VNIDsSplitter(VNs.Select(x => x.vn).ToArray());

            idSplitter.Split();

            if (idSplitter.SplittingNeccessary)
            {
                for (int round = 0; round < idSplitter.NumberOfRequests; round++)
                {
                    await AddVNs(Take(idSplitter.IDs, round * idSplitter.MaxVNsPerRequest, idSplitter.MaxVNsPerRequest), VNs);
                }

                if (idSplitter.Remainder > 0)
                {
                    await AddVNs(Take(idSplitter.IDs, idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder), VNs);
                }
            }
            else
            {
                await AddVNs(idSplitter.IDs, VNs);
            }
        }

        private async Task AddVNs(int[] ids, List<VN> VNs)
        {
            var newVNs = await _VNService.Get(ids.ToList());
            var newVisualNovels = new List<VisualNovelModel>(newVNs);

            newVisualNovels.ForEach(x => x.Category = (VisualNovelModel.VisualNovelCatergory)VNs.First(y => y.vn == x.Basics.ID).status);
            newVisualNovels.ForEach(x => x.Score = 0);

            _VNService.Add(newVisualNovels);

            UpdateProgess(ids.Length, " Visual Novels have been synchronized...");
        }
    }
}
