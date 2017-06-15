using System;
using VNDBUpdater.BackgroundTasks.Interfaces;

namespace VNDBUpdater.Services.Launch
{
    public class LaunchService : ILaunchService
    {
        private ITaskFactory _TaskFactory;

        private Action<bool> _OnLaunchFinished;

        public LaunchService(ITaskFactory TaskFactory)
        {
            _TaskFactory = TaskFactory;
        }

        public void Launch(Action<bool> onLaunchFinished)
        {
            _OnLaunchFinished = onLaunchFinished;

            StartStartUp();
        }

        private void StartStartUp()
        {
            IBackgroundTask task = _TaskFactory.CreateStartUpTask();

            task.ExecuteTaskAsync(StartSynchronizer);
        }

        private void StartSynchronizer(bool startUpSuccessfull)
        {
            if (startUpSuccessfull)
            {
                IBackgroundTask task = _TaskFactory.CreateSynchronizerTask();

                task.ExecuteTaskAsync(_OnLaunchFinished);
            }
            else
            {
                _OnLaunchFinished(false);
            }
        }        
    }
}
