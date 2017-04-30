using System;
using VNDBUpdater.GUI.Models;

namespace VNDBUpdater.Services.Status
{
    public class StatusService : IStatusService
    {
        private StatusModel _CurrentStatus = new StatusModel();

        public event EventHandler OnUpdated = delegate { };

        public StatusService() { }

        public string CurrentUser
        {
            get { return _CurrentStatus.CurrentUser; }
            set { _CurrentStatus.CurrentUser = value; OnUpdated?.Invoke(this, null); }
        }

        public string CurrentMessage
        {
            get { return _CurrentStatus.Message; }
            set { _CurrentStatus.Message = value; OnUpdated?.Invoke(this, null); }
        }

        public string CurrentTask
        {
            get { return _CurrentStatus.TaskName; }
            set { _CurrentStatus.TaskName = value; OnUpdated?.Invoke(this, null); }
        }

        public string CurrentError
        {
            get { return _CurrentStatus.ErrorMessage; }
            set { _CurrentStatus.ErrorMessage = value; OnUpdated?.Invoke(this, null); }
        }

        public bool TaskIsRunning
        {
            get { return _CurrentStatus.TaskIsRunning; }
            set { _CurrentStatus.TaskIsRunning = value; OnUpdated?.Invoke(this, null); }
        }

        public int PercentageOfTaskCompleted
        {
            get { return _CurrentStatus.PercentageTaskCompleted; }
            set { _CurrentStatus.PercentageTaskCompleted = value; OnUpdated?.Invoke(this, null); }
        }
    }
}
