using System;
using System.Threading.Tasks;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.Tags;
using VNDBUpdater.Services.Traits;

namespace VNDBUpdater.BackgroundTasks
{
    public class StartUp : TaskBase
    {
        private ITagService _TagService;
        private ITraitService _TraitService;

        public StartUp(IStatusService StatusService, ITagService TagService, ITraitService TraitService, ILoggerService LoggerService)
            : base(StatusService, LoggerService)
        {
            _TagService = TagService;
            _TraitService = TraitService;
        }

        public override void Start(Action<bool> OnTaskCompleted)
        {
            CurrentTask = nameof(StartUp);

            Task.Factory.StartNew(() => StartUpProgram(OnTaskCompleted));
        }

        private void StartUpProgram(Action<bool> OnTaskCompleted)
        {
            try
            {
                IsRunning = true;
                CurrentStatus = nameof(StartUp) + " running.";

                PercentageCompleted = 0;

                _TagService.Refresh();

                PercentageCompleted = 40;

                _TraitService.Refresh();
                                                                    
                PercentageCompleted = 100;

                CurrentStatus = nameof(StartUp) + " finished.";
                IsRunning = false;

                OnTaskCompleted(true);                
            }
            catch (Exception ex)
            {
                _Logger.Log(ex);

                CurrentStatus = nameof(StartUp) + " faulted.";
                IsRunning = false;

                OnTaskCompleted(false);
            }
        }
    }
}
