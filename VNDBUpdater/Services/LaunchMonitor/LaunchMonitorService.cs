using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.Services.LaunchMonitor
{
    public class LaunchMonitorService : ILaunchMonitorService
    {
        private IVNService _VNService;
        private ManagementEventWatcher _Watcher;

        private VisualNovelModel _LaunchedVisualNovel;
        private Process _RelevantProcess;

        public LaunchMonitorService(IVNService VNService)
        {
            _VNService = VNService;
        }

        public void StartMonitoring()
        {
            if (_Watcher == null)
            {
                try
                {
                    var query = new WqlEventQuery("__InstanceCreationEvent", new TimeSpan(0, 0, 1), "TargetInstance isa 'Win32_Process'");

                    _Watcher = new ManagementEventWatcher(query);
                    _Watcher.EventArrived += new EventArrivedEventHandler(OnProcessLaunched);

                    _Watcher.Start();
                }
                catch (Exception)
                {
                }
                finally
                {
                    _Watcher.Dispose();
                }
            }
        }
        
        private async void OnProcessLaunched(object sender, EventArrivedEventArgs e)
        {
            try
            {
                string instanceLaunched = ((ManagementBaseObject)e.NewEvent["TargetInstance"])["Name"].ToString();
                string path = ((ManagementBaseObject)e.NewEvent["TargetInstance"])["ExecutablePath"].ToString();

                if (string.IsNullOrEmpty(instanceLaunched) || string.IsNullOrEmpty(path))
                {
                    return;
                }

                var localVNs = await _VNService.GetLocal();

                if (localVNs.Where(x => !string.IsNullOrEmpty(x.ExePath)).Any(x => string.Equals(x.ExePath, path, StringComparison.OrdinalIgnoreCase)))
                {
                    _LaunchedVisualNovel = localVNs.Where(x => !string.IsNullOrEmpty(x.ExePath)).First(x => string.Equals(x.ExePath, path, StringComparison.OrdinalIgnoreCase));

                    _RelevantProcess = Process.GetProcessesByName(instanceLaunched.Replace(".exe", "")).First();
                    _RelevantProcess.EnableRaisingEvents = true;
                    _RelevantProcess.Exited += OnProcessEnded;
                }
            }
            catch { return; } // Ignore...
        }

        private void OnProcessEnded(object sender, EventArgs e)
        {
            _VNService.AddToPlayTime(_LaunchedVisualNovel, DateTime.Now - (sender as Process).StartTime);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Watcher.Dispose();
                _RelevantProcess?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
