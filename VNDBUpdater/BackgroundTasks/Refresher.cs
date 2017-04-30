using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.Tags;
using VNDBUpdater.Services.Traits;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.BackgroundTasks
{
    public class Refresher : TaskBase
    {
        private IVNService _VNService;
        private ITagService _TagService;
        private ITraitService _TraitService;

        public Refresher(IStatusService StatusService, IVNService VNService, ITagService TagService, ITraitService TraitService, ILoggerService LoggerService)
            : base(StatusService, LoggerService)
        {
            _VNService = VNService;
            _TagService = TagService;
            _TraitService = TraitService;
        }

        public override void Start(Action<bool> OnTaskCompleted)
        {
            CurrentTask = nameof(Refresher);

            Task.Factory.StartNew(() => Refresh(OnTaskCompleted));
        }

        private async Task Refresh(Action<bool> OnTaskCompleted)
        {
            try
            {
                PercentageCompleted = 0;
                IsRunning = true;
                CurrentStatus = nameof(Refresher) + " running.";

                _TagService.Refresh();
                _TraitService.Refresh();

                CurrentStatus = "Tags and Traits have been refreshed...";

                var updatedVNs = new List<VisualNovelModel>();

                var idSplitter = new VNIDsSplitter(_VNService.GetLocal().Select(x => x.Basics.ID).ToArray());

                idSplitter.Split();

                _TasksToDo = idSplitter.IDs.Length;

                CurrentStatus = _TasksToDo.ToString() + " need to be updated...";

                if (idSplitter.SplittingNeccessary)
                {
                    for (int round = 0; round < idSplitter.NumberOfRequests; round++)
                    {
                        var updated = await _VNService.Get(Take(idSplitter.IDs, round * idSplitter.MaxVNsPerRequest, idSplitter.MaxVNsPerRequest).ToList());

                        updatedVNs.AddRange(updated);
                        UpdateProgess(idSplitter.MaxVNsPerRequest, "Visual Novels have been updated...");
                    }

                    if (idSplitter.Remainder > 0)
                    {
                        var updated = await _VNService.Get(Take(idSplitter.IDs, idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder).ToList());

                        updatedVNs.AddRange(updated);
                        UpdateProgess(idSplitter.Remainder, "Visual Novels have been updated...");
                    }
                }
                else
                {
                    var updated = await _VNService.Get(idSplitter.IDs.ToList());

                    updatedVNs.AddRange(updated);
                    UpdateProgess(idSplitter.IDs.Length, "Visual Novels have been updated...");
                }

                var newVNs = new List<VisualNovelModel>();

                foreach (var vn in updatedVNs)
                {
                    VisualNovelModel vnToUpdate = _VNService.GetLocal(vn.Basics.ID);

                    if (vnToUpdate != null)
                    {
                        vnToUpdate.Basics = vn.Basics;
                        vnToUpdate.Characters = vn.Characters;
                    }

                    newVNs.Add(vnToUpdate);
                }

                _VNService.Add(newVNs);

                CurrentStatus = nameof(Refresher) + " finished.";
                IsRunning = false;

                OnTaskCompleted(true);
            }
            catch (Exception ex)
            {
                _Logger.Log(ex);

                CurrentStatus = nameof(Refresher) + " ran into error.";
                IsRunning = false;

                OnTaskCompleted(false);
            }
        }
    }
}
