using Microsoft.Practices.Unity;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.GUI.Views;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.Version;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class MainViewModel : ViewModelBase, IMainWindowModel
    {
        private UserModel _User;

        private IUserService _UserService;
        private IStatusService _StatusService;
        private IVersionService _VersionService;

        public MainViewModel(IUserService UserService, IStatusService StatusService, IVersionService VersionService)
            : base()
        {
            _UserService = UserService;
            _StatusService = StatusService;
            _VersionService = VersionService;

            _UserService.OnUpdated += OnUserUpdated;

            _StatusService.OnUpdated += OnStatusUpdated;

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            OnUserUpdated(this, await _UserService.Get());
        }

        private void OnUserUpdated(object sender, UserModel User)
        {
            _User = User;
        }

        private void OnStatusUpdated(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(CurrentTask));
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(PercentageTaskCompleted));
            OnPropertyChanged(nameof(TaskIsRunning));
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

        public int PercentageTaskCompleted
        {
            get { return _StatusService.PercentageOfTaskCompleted; }
        }

        public double Height
        {
            get { return _User.GUI.Height; }
            set { _User.GUI.Height = value; _UserService.Update(_User); }
        }

        public double Width
        {
            get { return _User.GUI.Width; }
            set { _User.GUI.Width = value; _UserService.Update(_User); }
        }

        public string Title
        {
            get { return _VersionService.NewVersionAvailable ? "VNDBUpdater " + _VersionService.CurrentVersion + " New Version available! Check the 'About'-Window for GitHub link." : "VNDBUpdater " + _VersionService.CurrentVersion; }                                                                         
        }

        public void ExecuteCloseWindow(object parameter)
        {
            try
            {
                _WindowHandler.CloseAll();
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Application.Current.Shutdown();
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
