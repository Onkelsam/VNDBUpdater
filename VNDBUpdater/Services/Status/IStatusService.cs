using System;

namespace VNDBUpdater.Services.Status
{
    public interface IStatusService
    {
        string CurrentUser { get; set; }
        string CurrentMessage { get; set; }
        string CurrentTask { get; set; }
        string CurrentError { get; set; }
        
        bool TaskIsRunning { get; set; }
        int PercentageOfTaskCompleted { get; set; }

        event EventHandler OnUpdated;
    }
}
