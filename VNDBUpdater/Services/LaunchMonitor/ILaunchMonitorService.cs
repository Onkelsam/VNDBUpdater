using System;

namespace VNDBUpdater.Services.LaunchMonitor
{
    public interface ILaunchMonitorService : IDisposable
    {
        void StartMonitoring();
    }
}
