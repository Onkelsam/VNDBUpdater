using VNDBUpdater.BackgroundTasks.Interfaces;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.Tags;
using VNDBUpdater.Services.Traits;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.BackgroundTasks.Factory
{
    public class BackgroundTaskFactory : ITaskFactory
    {
        private IStatusService _StatusService;
        private IVNService _VNService;
        private IUserService _UserService;
        private ITagService _TagService;
        private ITraitService _TraitService;
        private ILoggerService _LoggerService;

        public BackgroundTaskFactory(IStatusService StatusService, IVNService VNService, IUserService UserService, ITagService TagService, ITraitService TraitService, ILoggerService LoggerService)
        {
            _StatusService = StatusService;
            _VNService = VNService;
            _UserService = UserService;
            _TagService = TagService;
            _TraitService = TraitService;
            _LoggerService = LoggerService;
        }

        public ITask CreateStartUpTask()
        {
            return new StartUp(_StatusService, _TagService, _TraitService, _LoggerService);
        }

        public ITask CreateSynchronizerTask()
        {
            return new Synchronizer(_StatusService, _VNService, _LoggerService);
        }

        public ITask CreateRefresherTask()
        {
            return new Refresher(_StatusService, _VNService, _TagService, _TraitService, _LoggerService);
        }

        public ITask CreateFileIndexerTask()
        {
            return new FileIndexer(_StatusService, _VNService, _UserService, _LoggerService);
        }
    }
}
