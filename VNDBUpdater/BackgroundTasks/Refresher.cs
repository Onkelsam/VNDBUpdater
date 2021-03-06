﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.BackgroundTasks
{
    public class Refresher : TaskBase
    {
        private IVNService _VNService;

        public Refresher(IStatusService StatusService, IVNService VNService, ILoggerService LoggerService)
            : base(StatusService, LoggerService)
        {
            _VNService = VNService;
        }

        public override async Task ExecuteTaskAsync(Action<bool> OnTaskCompleted)
        {
            _OnTaskCompleted = OnTaskCompleted;

            await Task.Factory.StartNew(async () => await Start(Refresh));
        }

        private async Task Refresh()
        {
            var updatedVNs = new List<VisualNovelModel>();

            foreach (var vn in await GetUpdatedVNs(await _VNService.GetLocalAsync()))
            {
                VisualNovelModel vnToUpdate = await _VNService.GetLocalAsync(vn.Basics.ID);

                if (vnToUpdate != null)
                {
                    vnToUpdate.Basics = vn.Basics;
                    vnToUpdate.Characters = vn.Characters;

                    updatedVNs.Add(vnToUpdate);
                }
            }

            await _VNService.AddAsync(updatedVNs);
        }

        private async Task<List<VisualNovelModel>> GetUpdatedVNs(IEnumerable<VisualNovelModel> localVNs)
        {
            var updatedVNs = new List<VisualNovelModel>();
            var idSplitter = new VNIDsSplitter(localVNs.Select(x => x.Basics.ID).ToArray());

            idSplitter.Split();

            _TasksToDo = idSplitter.IDs.Length;

            CurrentStatus = _TasksToDo.ToString() + " need to be updated...";

            if (idSplitter.SplittingNeccessary)
            {
                for (int round = 0; round < idSplitter.NumberOfRequests; round++)
                {
                    var updated = await _VNService.GetAsync(Take(idSplitter.IDs, round * idSplitter.MaxVNsPerRequest, idSplitter.MaxVNsPerRequest).ToList());

                    updatedVNs.AddRange(updated);
                    UpdateProgess(idSplitter.MaxVNsPerRequest, "Visual Novels have been updated...");
                }

                if (idSplitter.Remainder > 0)
                {
                    var updated = await _VNService.GetAsync(Take(idSplitter.IDs, idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder).ToList());

                    updatedVNs.AddRange(updated);
                    UpdateProgess(idSplitter.Remainder, "Visual Novels have been updated...");
                }
            }
            else
            {
                var updated = await _VNService.GetAsync(idSplitter.IDs.ToList());

                updatedVNs.AddRange(updated);
                UpdateProgess(idSplitter.IDs.Length, "Visual Novels have been updated...");
            }

            return updatedVNs;
        }
    }
}
