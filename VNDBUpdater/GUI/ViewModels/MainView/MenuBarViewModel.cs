using MahApps.Metro;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using VNDBUpdater.BackgroundTasks.Interfaces;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.Theme;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.GUI.Views;
using VNDBUpdater.Services.Filters;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class MenuBarViewModel : ViewModelBase, IMenuBarWindowModel
    {
        private UserModel _User;

        private IBackgroundTask _CurrentTask;
        private ITaskFactory _BackgroundTaskFactory;
        
        private IFilterService _FilterService;
        private IUserService _UserService;
        private IVNService _VNService;
        private IStatusService _StatusService;

        private IUnityContainer _UnityContainer;

        public MenuBarViewModel(ITaskFactory taskFactory, IFilterService FilterService, IUserService UserService, IVNService VNService, IStatusService StatusService, IUnityContainer UnityContainer)
            : base()
        {
            _BackgroundTaskFactory = taskFactory;
            _FilterService = FilterService;
            _UserService = UserService;
            _VNService = VNService;
            _StatusService = StatusService;

            _UnityContainer = UnityContainer;

            _FilterService.OnAdded += OnFilterAdded;
            _FilterService.OnDeleted += OnFilterDeleted;

            _UserService.OnUpdated += OnUserUpdated;

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            OnUserUpdated(this, await _UserService.GetAsync());
            _AvailableFilters = new ObservableCollection<FilterModel>(await _FilterService.Get());            
        }

        private void OnUserUpdated(object sender, UserModel User)
        {
            _User = User;
        }

        private void OnFilterAdded(object sender, FilterModel filter)
        {
            _AvailableFilters.Add(filter);

            OnPropertyChanged(nameof(AvailableFilters));
        }

        private void OnFilterDeleted(object sender, FilterModel filter)
        {
            _AvailableFilters.Remove(filter);

            OnPropertyChanged(nameof(AvailableFilters));
        }

        private ObservableCollection<FilterModel> _AvailableFilters;

        public ObservableCollection<FilterModel> AvailableFilters
        {
            get { return _AvailableFilters; }
            set { _AvailableFilters = value; OnPropertyChanged(nameof(AvailableFilters)); }
        }

        public List<AccentColorMenuData> AccentColors
        {
            get
            {
                return ThemeManager
                    .Accents
                    .Select(x => new AccentColorMenuData(_UserService) { Name = x.Name, ColorBrush = x.Resources["AccentColorBrush"] as Brush })
                    .ToList();
            }
        }

        public List<AppThemeMenuData> AppThemes
        {
            get
            {
                return ThemeManager
                    .AppThemes
                    .Select(x => new AppThemeMenuData(_UserService) { Name = x.Name, BorderColorBrush = x.Resources["BlackColorBrush"] as Brush, ColorBrush = x.Resources["WhiteColorBrush"] as Brush })
                    .ToList();
            }
        }

        private ICommand _AddVisualNovels;

        public ICommand AddVisualNovels
        {
            get
            {
                return _AddVisualNovels ??
                    (_AddVisualNovels = new RelayCommand(x => _WindowHandler.Open(_UnityContainer.Resolve<AddVisualNovels>()), x => true));
            }
        }

        private ICommand _OpenFileIndexer;

        public ICommand OpenFileIndexer
        {
            get
            {
                return _OpenFileIndexer ??
                    (_OpenFileIndexer = new RelayCommand(x => _WindowHandler.Open(_UnityContainer.Resolve<FileIndexer>())));
            }
        }

        private ICommand _About;

        public ICommand About
        {
            get
            {
                return _About ??
                    (_About = new RelayCommand(x => _WindowHandler.Open(_UnityContainer.Resolve<About>())));
            }
        }

        private ICommand _AddFilter;

        public ICommand AddFilter
        {
            get
            {
                return _AddFilter ??
                    (_AddFilter = new RelayCommand(x => _WindowHandler.Open(_UnityContainer.Resolve<CreateFilter>())));
            }
        }

        private ICommand _ApplyFilter;

        public ICommand ApplyFilter
        {
            get
            {
                return _ApplyFilter ??
                    (_ApplyFilter = new RelayCommand((parameter) => _FilterService.ApplyFilter(parameter as FilterModel)));
            }
        }        

        private ICommand _DeleteFilter;

        public ICommand DeleteFilter
        {
            get
            {
                return _DeleteFilter ??
                    (_DeleteFilter = new RelayCommand(async (parameter) => { await _FilterService.Delete(parameter as FilterModel); }));
            }
        }

        private ICommand _ResetFilters;

        public ICommand ResetFilters
        {
            get
            {
                return _ResetFilters ??
                    (_ResetFilters = new RelayCommand(x => _FilterService.ResetFilters()));
            }
        }

        private ICommand _RefreshVisualNovels;

        public ICommand RefreshVisualNovels
        {
            get
            {
                return _RefreshVisualNovels ??
                    (_RefreshVisualNovels = new RelayCommand(async x =>
                    {
                        _CurrentTask = _BackgroundTaskFactory.CreateRefresherTask();
                        await _CurrentTask.ExecuteTask((successfull) => { });
                    }, x => !_StatusService.TaskIsRunning));
            }
        }

        private ICommand _SynchronizeWithVNDB;

        public ICommand SynchronizeWithVNDB
        {
            get
            {
                return _SynchronizeWithVNDB ??
                    (_SynchronizeWithVNDB = new RelayCommand(async x =>
                    {
                        _CurrentTask = _BackgroundTaskFactory.CreateSynchronizerTask();
                        await _CurrentTask.ExecuteTask((successfull) => { });
                    }, x => !_StatusService.TaskIsRunning));
            }
        }
    }
}
