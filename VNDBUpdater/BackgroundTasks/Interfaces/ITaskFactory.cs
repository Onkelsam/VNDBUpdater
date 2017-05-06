namespace VNDBUpdater.BackgroundTasks.Interfaces
{
    public interface ITaskFactory
    {
        IBackgroundTask CreateStartUpTask();
        IBackgroundTask CreateSynchronizerTask();
        IBackgroundTask CreateRefresherTask();
        IBackgroundTask CreateFileIndexerTask();
    }
}
