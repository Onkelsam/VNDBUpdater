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

        public override async Task ExecuteTaskAsync(Action<bool> OnTaskCompleted)
        {
            _OnTaskCompleted = OnTaskCompleted;

            await Task.Factory.StartNew(async () => await Start(StartUpProgram));
        }

        private async Task StartUpProgram()
        {
            PercentageCompleted = 0;

            await _TagService.RefreshAsync();

            PercentageCompleted = 40;

            await _TraitService.RefreshAsync();

            PercentageCompleted = 100;
        }
    }
}
