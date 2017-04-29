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
            ITask task = _TaskFactory.CreateStartUpTask();

            task.Start(StartSynchronizer);
        }

        private void StartSynchronizer(bool startUpSuccessfull)
        {
            if (startUpSuccessfull)
            {
                ITask task = _TaskFactory.CreateSynchronizerTask();

                task.Start(_OnLaunchFinished);
            }
            else
            {
                _OnLaunchFinished(false);
            }
        }        
    }
}
