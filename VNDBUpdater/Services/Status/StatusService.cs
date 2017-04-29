using System;
using System.Linq;
using VNDBUpdater.GUI.Models;

namespace VNDBUpdater.Services.Status
{
    public class StatusService : IStatusService
    {
        private StatusModel _CurrentStatus = new StatusModel();

        private event Action _OnStatusUpdated = delegate { };

        public StatusService() { }

        public string CurrentUser
        {
            get { return _CurrentStatus.CurrentUser; }
            set { _CurrentStatus.CurrentUser = value; _OnStatusUpdated?.Invoke(); }
        }

        public string CurrentMessage
        {
            get { return _CurrentStatus.Message; }
            set { _CurrentStatus.Message = value;  _OnStatusUpdated?.Invoke(); }
        }

        public string CurrentTask
        {
            get { return _CurrentStatus.TaskName; }
            set { _CurrentStatus.TaskName = value; _OnStatusUpdated?.Invoke(); }
        }

        public string CurrentError
        {
            get { return _CurrentStatus.ErrorMessage; }
            set { _CurrentStatus.ErrorMessage = value;  _OnStatusUpdated?.Invoke(); }
        }

        public bool TaskIsRunning
        {
            get { return _CurrentStatus.TaskIsRunning; }
            set { _CurrentStatus.TaskIsRunning = value;  _OnStatusUpdated?.Invoke(); }
        }

        public int PercentageOfTaskCompleted
        {
            get { return _CurrentStatus.PercentageTaskCompleted; }
            set { _CurrentStatus.PercentageTaskCompleted = value;  _OnStatusUpdated?.Invoke(); }
        }

        public void SubscribeToStatusUpdated(Action onStatusUpdated)
        {
            if (!_OnStatusUpdated.GetInvocationList().Contains(onStatusUpdated))
            {
                _OnStatusUpdated += onStatusUpdated;
            }
        }
    }
}
