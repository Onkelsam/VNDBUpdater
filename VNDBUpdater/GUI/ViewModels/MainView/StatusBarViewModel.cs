using System;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.Status;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class StatusBarViewModel : ViewModelBase, IStatusBarViewModel
    {
        private IStatusService _StatusService;

        public StatusBarViewModel(IStatusService StatusService)
            : base()
        {
            _StatusService = StatusService;

            _StatusService.OnUpdated += OnStatusUpdated;
        }

        private void OnStatusUpdated(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(CurrentTask));
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(PercentageTaskCompleted));
            OnPropertyChanged(nameof(TaskIsRunning));
            OnPropertyChanged(nameof(ErrorOccured));
        }

        public string CurrentUser
        {
            get { return _StatusService.CurrentUser; }
        }

        public string CurrentTask
        {
            get { return _StatusService.CurrentTask; }
        }

        public string Message
        {
            get { return _StatusService.CurrentMessage; }
        }

        public string ErrorMessage
        {
            get { return _StatusService.CurrentError; }
        }

        public bool TaskIsRunning
        {
            get { return _StatusService.TaskIsRunning; }
        }

        public bool ErrorOccured
        {
            get { return !string.IsNullOrEmpty(_StatusService.CurrentError); }
        }

        public int PercentageTaskCompleted
        {
            get { return _StatusService.PercentageOfTaskCompleted; }
        }
    }
}
