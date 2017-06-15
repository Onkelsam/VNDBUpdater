using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.Services.LaunchMonitor
{
    public class LaunchMonitorService : ILaunchMonitorService
    {
        private IVNService _VNService;
        private ILoggerService _Logger;
        private ManagementEventWatcher _Watcher;

        private VisualNovelModel _LaunchedVisualNovel;
        private Process _RelevantProcess;

        public LaunchMonitorService(IVNService VNService, ILoggerService Logger)
        {
            _VNService = VNService;
            _Logger = Logger;
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
                catch (Exception ex)
                {
                    _Logger.Log(ex);
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
                string path = Path.GetDirectoryName(((ManagementBaseObject)e.NewEvent["TargetInstance"])["ExecutablePath"].ToString()) + "\\";

                _Logger.Log("instanceLaunched: " + instanceLaunched + Environment.NewLine + "path: " + path);

                if (string.IsNullOrEmpty(instanceLaunched) || string.IsNullOrEmpty(path))
                {
                    return;
                }

                IList<VisualNovelModel> localVNs = await _VNService.GetLocalAsync();

                if (localVNs.Where(x => !string.IsNullOrEmpty(x.FolderPath)).Any(x => string.Equals(x.FolderPath, path, StringComparison.OrdinalIgnoreCase)))
                {
                    _LaunchedVisualNovel = localVNs.Where(x => !string.IsNullOrEmpty(x.FolderPath)).First(x => string.Equals(x.FolderPath, path, StringComparison.OrdinalIgnoreCase));

                    _Logger.Log("Launched vn: " + _LaunchedVisualNovel.Basics.Title);

                    var name = instanceLaunched.Split('.').First();

                    _RelevantProcess = Process.GetProcessesByName(name).First();
                    _RelevantProcess.EnableRaisingEvents = true;
                    _RelevantProcess.Exited += OnProcessEnded;
                }
            }
            catch (Exception ex)
            {
                _Logger.Log(ex);
            }
        }

        private void OnProcessEnded(object sender, EventArgs e)
        {
            _Logger.Log("Launched vn: " + _LaunchedVisualNovel.Basics.Title + " Playtime: " + _LaunchedVisualNovel.PlayTime.ToString());

            _VNService.AddToPlayTimeAsync(_LaunchedVisualNovel, DateTime.Now - (sender as Process).StartTime);
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
