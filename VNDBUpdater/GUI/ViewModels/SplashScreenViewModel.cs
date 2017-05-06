using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.Launch;
using VNDBUpdater.Services.Login;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.Version;

namespace VNDBUpdater.GUI.ViewModels
{
    public class SplashScreenViewModel : ViewModelBase, ISplashScreenWindowModel
    {
        private IStatusService _StatusService;
        private ILoginService _LoginService;
        private ILaunchService _LaunchService;
        private IVersionService _VersionService;

        private IDialogCoordinator _DialogCoordinator; 

        public SplashScreenViewModel(IStatusService StatusService, ILaunchService LaunchService, ILoginService LoginService, IDialogCoordinator dialogCoordinator, IVersionService VersionService)
            : base()
        {
            _StatusService = StatusService;
            _LaunchService = LaunchService;
            _LoginService = LoginService;
            _VersionService = VersionService;

            _DialogCoordinator = dialogCoordinator;

            _StatusService.OnUpdated += OnStatusUpdated;

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            LoginRequired = await _LoginService.CheckLoginStatus();

            if (!LoginRequired)
            {
                _LaunchService.Launch(OnLaunchComplete);
            }
        }

        private void OnStatusUpdated(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(TaskRunning));
            OnPropertyChanged(nameof(PercentageTaskCompleted));
        }

        private Window _SplashScreen;

        public Window SplashScreen
        {
            get { return _SplashScreen; }
            set { _SplashScreen = value; }
        }

        private bool _LoginRequired;

        public bool LoginRequired
        {
            get { return _LoginRequired; }
            private set { _LoginRequired = value; OnPropertyChanged(nameof(LoginRequired)); }
        }

        public string Title
        {
            get { return "VNDB Updater"; }
        }

        public string Version
        {
            get { return "Version: " + _VersionService.CurrentVersion; }
        }

        public bool TaskRunning
        {
            get { return _StatusService.TaskIsRunning; }
        }

        public int PercentageTaskCompleted
        {
            get { return _StatusService.PercentageOfTaskCompleted; }
        }

        public string Status
        {
            get { return _StatusService.CurrentMessage; }
        }

        private ICommand _Login;

        public ICommand Login
        {
            get
            {
                return _Login ?? (_Login = new RelayCommand(
                     async x =>
                     {
                         await _DialogCoordinator
                            .ShowLoginAsync(this, "Login needed", "Please login using you VNDB login data...", new LoginDialogSettings() { RememberCheckBoxVisibility = Visibility.Visible })
                            .ContinueWith(async y => await ExecuteLogin(y.Result));
                     }));
            }
        }

        private async Task ExecuteLogin(LoginDialogData answer)
        {
            bool loginsuccessfull = await _LoginService.Login(answer);

            if (loginsuccessfull)
            {
                _LaunchService.Launch(OnLaunchComplete);
                LoginRequired = false;

                OnPropertyChanged(nameof(LoginRequired));
            }
            else
            {
                _StatusService.CurrentMessage = "Login failed...";
            }
        }

        private void OnLaunchComplete(bool successfull)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _SplashScreen.DialogResult = successfull;
                _SplashScreen.Close();
            });
        }
    }
}
