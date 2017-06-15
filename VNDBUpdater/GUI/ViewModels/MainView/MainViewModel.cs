using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VNDBUpdater.Data;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.GUI.Views;
using VNDBUpdater.Helper;
using VNDBUpdater.Services.Launch;
using VNDBUpdater.Services.Login;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.Version;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class MainViewModel : ViewModelBase, IMainWindowModel
    {
        private UserModel _User;

        private IUserService _UserService;
        private IVersionService _VersionService;
        private ILoginService _LoginService;
        private ILaunchService _LaunchService;

        private IDialogCoordinator _DialogCoordinator;

        public MainViewModel(IUserService UserService, IVersionService VersionService, IDialogCoordinator DialogCoordinator, ILoginService LoginService, ILaunchService LaunchService)
            : base()
        {
            _UserService = UserService;
            _VersionService = VersionService;
            _LoginService = LoginService;
            _LaunchService = LaunchService;

            _DialogCoordinator = DialogCoordinator;

            _UserService.OnUpdated += OnUserUpdated;

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            OnUserUpdated(this, await _UserService.GetAsync());
        }

        private void OnUserUpdated(object sender, UserModel User)
        {
            _User = User;
        }

        private IVisualNovelsGridWindowModel _VisualNovelsGrid;

        [Dependency]
        public IVisualNovelsGridWindowModel VisualNovelsGrid
        {
            get { return _VisualNovelsGrid; }
            set { _VisualNovelsGrid = value; }
        }

        private IMenuBarWindowModel _Menu;

        [Dependency]
        public IMenuBarWindowModel Menu
        {
            get { return _Menu; }
            set { _Menu = value; }
        }

        private IVisualNovelInfoWindowModel _VisualNovelInfo;

        [Dependency]
        public IVisualNovelInfoWindowModel VisualNovelInfo
        {
            get { return _VisualNovelInfo; }
            set { _VisualNovelInfo = value; }
        }

        private IScreenshotTabWindowModel _ScreenshotTab;

        [Dependency]
        public IScreenshotTabWindowModel ScreenshotTab
        {
            get { return _ScreenshotTab; }
            set { _ScreenshotTab = value; }
        }

        private ICharacterTabWindowModel _CharacterTab;

        [Dependency]
        public ICharacterTabWindowModel CharacterTab
        {
            get { return _CharacterTab; }
            set { _CharacterTab = value; }
        }

        private ITagTabWIndowModel _TagTab;

        [Dependency]
        public ITagTabWIndowModel TagTab
        {
            get { return _TagTab; }
            set { _TagTab = value; }
        }

        private IStatusBarViewModel _StatsuBar;

        [Dependency]
        public IStatusBarViewModel StatusBar
        {
            get { return _StatsuBar; }
            set { _StatsuBar = value; }
        }

        public double Height
        {
            get { return _User.GUI.Height; }
            set { _User.GUI.Height = value; _UserService.UpdateAsync(_User); }
        }

        public double Width
        {
            get { return _User.GUI.Width; }
            set { _User.GUI.Width = value; _UserService.UpdateAsync(_User); }
        }

        public int ImageDimension
        {
            get { return _User.GUI.ImageDimension; }
            set { _User.GUI.ImageDimension = value; _UserService.UpdateAsync(_User); }
        }

        public bool NewVersionAvailable
        {
            get { return _VersionService.NewVersionAvailable; }
        }

        public string Username
        {
            get { return _User.Username; }
        }

        public bool SaveLogin
        {
            get { return _User.SaveLogin; }
            set { _User.SaveLogin = value; OnPropertyChanged(nameof(SaveLogin)); }
        }

        public List<string> SpoilerLevels
        {
            get { return Enum.GetNames(typeof(SpoilerSetting))
                    .Select(x => ExtensionMethods.GetAttributeOfType<DescriptionAttribute>((SpoilerSetting)Enum.Parse(typeof(SpoilerSetting), x)))
                    .Select(x => x.Description).ToList(); }
        }

        public string SpoilerLevel
        {
            get { return _User.Settings.SpoilerSetting.GetAttributeOfType<DescriptionAttribute>().Description.ToString(); }
            set { _User.Settings.SpoilerSetting = value.GetEnumValueFromDescription<SpoilerSetting>(); OnPropertyChanged(nameof(SpoilerLevel)); }
        }

        public bool ShowNSFWImages
        {
            get { return _User.Settings.ShowNSFWImages; }
            set { _User.Settings.ShowNSFWImages = value; OnPropertyChanged(nameof(ShowNSFWImages)); }
        }

        public bool MinimizeToTray
        {
            get { return _User.Settings.MinimizeToTray; }
            set { _User.Settings.MinimizeToTray = value; OnPropertyChanged(nameof(MinimizeToTray)); }
        }

        private string _Status;

        public string Status
        {
            get { return _Status; }
            set { _Status = value; OnPropertyChanged(nameof(Status)); }
        }

        private ICommand _CloseWindow;

        public ICommand CloseWindow
        {
            get
            {
                return _CloseWindow ??
                    (_CloseWindow = new RelayCommand(ExecuteCloseWindow));
            }
        }

        private ICommand _ShowMainWindow;

        public ICommand ShowMainWindow
        {
            get
            {
                return _ShowMainWindow ??
                    (_ShowMainWindow = new RelayCommand((parameter) => _WindowHandler.Show(parameter)));
            }
        }

        private ICommand _MinimizeMainWindow;

        public ICommand MinimizeMainWindow
        {
            get
            {
                return _MinimizeMainWindow ??
                    (_MinimizeMainWindow = new RelayCommand(ExecuteStateChanged));
            }
        }

        private ICommand _Login;

        public ICommand Login
        {
            get
            {
                return _Login ?? (_Login = new RelayCommand(
                     async x =>
                     {
                         await _DialogCoordinator.ShowLoginAsync(this, "Warning", "This will delete all local data! Are you sure you want to procede? Press ESC to cancel.", new LoginDialogSettings() { RememberCheckBoxVisibility = System.Windows.Visibility.Visible })
                            .ContinueWith(async y => await ExecuteLogin(y.Result));
                     }));
            }
        }

        private ICommand _OpenGitHubLink;

        public ICommand OpenGitHubLink
        {
            get
            {
                return _OpenGitHubLink ??
                    (_OpenGitHubLink = new RelayCommand(x => _VersionService.OpenLinkToNewestVersion()));
            }
        }

        private ICommand _SaveSettings;

        public ICommand SaveSettings
        {
            get
            {
                return _SaveSettings ??
                  (_SaveSettings = new RelayCommand(async x => await _UserService.UpdateAsync(_User)));
            }
        }

        public void ExecuteCloseWindow(object parameter)
        {
            try
            {
                _WindowHandler.CloseAll();
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
            finally
            {
                Application.Current.Shutdown();
            }
        }

        public async Task ExecuteLogin(LoginDialogData answer)
        {
            bool loginSuccessfull = await _LoginService.LoginAsync(answer);

            if (loginSuccessfull)
            {
                _LaunchService.Launch((successfull) => {
                    Status = successfull
                        ? "Login successfull..."
                        : "Login failed...";
                });
            }
            else
            {
                Status = "Login failed or aborted...";
            }
        }

        private void ExecuteStateChanged(object parameter)
        {
            var window = (parameter as MainWindow);

            if (window.WindowState == WindowState.Minimized)
            {
                _WindowHandler.Minimize(window);
            }
        }
    }
}
