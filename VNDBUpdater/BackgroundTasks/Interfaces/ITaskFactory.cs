namespace VNDBUpdater.BackgroundTasks.Interfaces
{
    public interface ITaskFactory
    {
        ITask CreateStartUpTask();
        ITask CreateSynchronizerTask();
        ITask CreateRefresherTask();
        ITask CreateFileIndexerTask();
    }
}
