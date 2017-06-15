using System;
using System.Threading.Tasks;

namespace VNDBUpdater.BackgroundTasks.Interfaces
{
    public interface IBackgroundTask
    {
        Task ExecuteTaskAsync(Action<bool> OnTaskCompleted);
    }
}
