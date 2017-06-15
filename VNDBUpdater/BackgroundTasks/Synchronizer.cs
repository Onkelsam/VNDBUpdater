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

        public override async Task ExecuteTaskAsync(Action<bool> OnTaskCompleted)
        {
            _OnTaskCompleted = OnTaskCompleted;

            await Task.Factory.StartNew(async () => await Start(Synchronize));
        }

        private async Task Synchronize()
        {
            var VNList = new List<VN>(await _VNService.GetVNListAsync());
            var VoteList = new List<Vote>(await _VNService.GetVoteListAsync());

            _TasksToDo = VNList.Count + VoteList.Count;

            CurrentStatus = _TasksDone + " Visual Novels of " + _TasksToDo + " synchronized";

            await SynchronizeVNs(VNList);
            await SynchronizeVotes(VoteList);
        }

        private async Task SynchronizeVNs(List<VN> VNsToSynchronize)
        {
            var VNsToAdd = new List<VN>();

            foreach (var vn in VNsToSynchronize)
            {
                if (await _VNService.CheckIfVNExistsAsync(vn.vn))
                {
                    VisualNovelModel LocalVN = await _VNService.GetLocalAsync(vn.vn);

                    if (LocalVN.Category != (VisualNovelModel.VisualNovelCatergory)vn.status)
                    {
                        LocalVN.Category = (VisualNovelModel.VisualNovelCatergory)vn.status;
                        await _VNService.AddAsync(LocalVN);
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

            await DeleteLocalVNs(VNsToSynchronize);
        }

        private async Task DeleteLocalVNs(List<VN> VNsSynchronized)
        {
            foreach (var localVN in await _VNService.GetLocalAsync())
            {
                if (!VNsSynchronized.Any(x => x.vn == localVN.Basics.ID))
                {
                    await _VNService.DeleteLocalAsync(localVN);
                }
            }
        }

        private async Task SynchronizeVotes(List<Vote> VotesToSynchronize)
        {
            foreach (var vote in VotesToSynchronize)
            {
                if (await _VNService.CheckIfVNExistsAsync(vote.vn))
                {
                    VisualNovelModel LocalVN = await _VNService.GetLocalAsync(vote.vn);

                    if (LocalVN.Score != vote.vote)
                    {
                        LocalVN.Score = vote.vote;
                        await _VNService.AddAsync(LocalVN);
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
            var newVisualNovels = new List<VisualNovelModel>(await _VNService.GetAsync(ids.ToList()));

            newVisualNovels.ForEach(x => x.Category = (VisualNovelModel.VisualNovelCatergory)VNs.First(y => y.vn == x.Basics.ID).status);
            newVisualNovels.ForEach(x => x.Score = 0);

            await _VNService.AddAsync(newVisualNovels);

            UpdateProgess(ids.Length, " Visual Novels have been synchronized...");
        }
    }
}
