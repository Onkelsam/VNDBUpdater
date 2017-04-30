using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VNDBUpdater.Data;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Helper;
using VNDBUpdater.Services.Launch;
using VNDBUpdater.Services.Login;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.GUI.ViewModels
{
    class OptionsViewModel : ViewModelBase, IOptionsWindowModel
    {
        private UserModel _User;

        private IUserService _UserService;
        private IStatusService _StatusService;
        private ILoginService _LoginService;
        private ILaunchService _LaunchService;

        private IDialogCoordinator _DialogCoordinator;     
          


        public OptionsViewModel(IDialogCoordinator dialogCoordinator, IUserService UserService, IStatusService StatusService, ILoginService LoginService, ILaunchService LaunchService)
            : base()
        {
            _DialogCoordinator = dialogCoordinator;
            _StatusService = StatusService;
            _UserService = UserService;
            _LoginService = LoginService;
            _LaunchService = LaunchService;

            _User = UserService.Get(); 

            _SaveLogin = _User.SaveLogin;
            _SpoilerLevel = _User.Settings.SpoilerSetting;
            _ShowNSFWImages = _User.Settings.ShowNSFWImages;
            _MinimizeToTray = _User.Settings.MinimizeToTray;
        }

        public string Username
        {
            get { return _User.Username; }
        }

        private bool _SaveLogin;

        public bool SaveLogin
        {
            get { return _SaveLogin; }
            set
            {
                _SaveLogin = value;

                OnPropertyChanged(nameof(SaveLogin));
            }
        }

        public List<string> SpoilerLevels
        {
            get { return Enum.GetNames(typeof(SpoilerSetting)).Select(x => ExtensionMethods.GetAttributeOfType<DescriptionAttribute>((SpoilerSetting)Enum.Parse(typeof(SpoilerSetting), x))).Select(x => x.Description).ToList(); }
        }

        private SpoilerSetting _SpoilerLevel;

        public string SpoilerLevel
        {
            get { return _SpoilerLevel.GetAttributeOfType<DescriptionAttribute>().Description.ToString(); }
            set
            {
                _SpoilerLevel = value.GetEnumValueFromDescription<SpoilerSetting>();

                OnPropertyChanged(nameof(SpoilerLevel));
            }
        }

        private bool _ShowNSFWImages;

        public bool ShowNSFWImages
        {
            get { return _ShowNSFWImages; }
            set
            {
                _ShowNSFWImages = value;

                OnPropertyChanged(nameof(ShowNSFWImages));
            }
        }

        private bool _MinimizeToTray;

        public bool MinimizeToTray
        {
            get { return _MinimizeToTray; }
            set
            {
                _MinimizeToTray = value;

                OnPropertyChanged(nameof(MinimizeToTray));
            }
        }

        private string _Status;

        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;

                OnPropertyChanged(nameof(Status));
            }
        }

        private ICommand _Login;

        private LoginDialogSettings _LoginDialog = new LoginDialogSettings()
        {
            RememberCheckBoxVisibility = System.Windows.Visibility.Visible,            
        };

        public ICommand Login
        {
            get
            {
                return _Login ?? (_Login = new RelayCommand(
                     x =>
                    {
                         _DialogCoordinator.ShowLoginAsync(this, "Warning", "This will delete all local data! Are you sure you want to procede? Press ESC to cancel.", _LoginDialog).ContinueWith(async y => await ExecuteLogin(y.Result));
                    }));
            }
        }

        private ICommand _SaveSettings;

        public ICommand SaveSettings
        {
            get
            {
                return _SaveSettings ??
                  (_SaveSettings = new RelayCommand(ExecuteSaveSettings));
            }
        }

        public void ExecuteSaveSettings(object parameter)
        {
            _User.Settings.MinimizeToTray = _MinimizeToTray;
            _User.Settings.SpoilerSetting = _SpoilerLevel;
            _User.Settings.ShowNSFWImages = _ShowNSFWImages;
            _User.SaveLogin = _SaveLogin;

            _UserService.Update(_User);
        }

        public async Task ExecuteLogin(LoginDialogData answer)
        {
            bool loginSuccessfull = await _LoginService.Login(answer);

            if (loginSuccessfull)
            {
                _LaunchService.Launch((successfull)  => {
                    Status = successfull ? "Login successfull..." : "Login failed...";
                });
            }
            else
            {
                Status = "Login failed...";
            }
        }
    }
}
