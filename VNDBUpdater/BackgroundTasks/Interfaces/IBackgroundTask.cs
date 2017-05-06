using System;
using System.Threading.Tasks;

namespace VNDBUpdater.BackgroundTasks.Interfaces
{
    public interface IBackgroundTask
    {
        Task ExecuteTask(Action<bool> OnTaskCompleted);
    }
}
