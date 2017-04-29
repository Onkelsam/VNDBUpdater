using System;

namespace VNDBUpdater.BackgroundTasks.Interfaces
{
    public interface ITask
    {
        void Start(Action<bool> OnTaskCompleted);
    }
}
