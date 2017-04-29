using MahApps.Metro;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private ITask _CurrentTask;
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

            _User = _UserService.Get();

            _AvailableFilters = new ObservableCollection<FilterModel>(_FilterService.Get().ToList());

            _FilterService.SubscribeToTAdded(OnFilterAdded);
            _FilterService.SubscribeToTDeleted(OnFilterDeleted);

            _UserService.SubscribeToTUpdated(OnUserUpdated);
        }

        private void OnUserUpdated(UserModel User)
        {
            _User = User;

            OnPropertyChanged(nameof(ImageDimension));
        }

        private void OnFilterAdded(FilterModel filter)
        {
            _AvailableFilters.Add(filter);

            OnPropertyChanged(nameof(AvailableFilters));
        }

        private void OnFilterDeleted(FilterModel filter)
        {
            _AvailableFilters.Remove(filter);

            OnPropertyChanged(nameof(AvailableFilters));
        }

        private ObservableCollection<FilterModel> _AvailableFilters;

        public ObservableCollection<FilterModel> AvailableFilters
        {
            get { return _AvailableFilters; }
            set
            {
                _AvailableFilters = value;

                OnPropertyChanged(nameof(AvailableFilters));
            }
        }

        public int ImageDimension
        {
            get { return _User.GUI.ImageDimension; }
            set
            {
                _User.GUI.ImageDimension = value;

                _UserService.Update(_User);
            }
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

        private ICommand _OpenOptions;

        public ICommand OpenOptions
        {
            get
            {
                return _OpenOptions ??
                    (_OpenOptions = new RelayCommand(x => _WindowHandler.Open(_UnityContainer.Resolve<Options>())));
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
                    (_AddFilter = new RelayCommand(x => _WindowHandler.Open(_UnityContainer.Resolve<CreateFilter>()), x => true));
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

        private ICommand _RefreshVisualNovels;

        private ICommand _DeleteFilter;

        public ICommand DeleteFilter
        {
            get
            {
                return _DeleteFilter ??
                    (_DeleteFilter = new RelayCommand((parameter) => { _FilterService.Delete(parameter as FilterModel); }));
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

        public ICommand RefreshVisualNovels
        {
            get
            {
                return _RefreshVisualNovels ??
                    (_RefreshVisualNovels = new RelayCommand(x =>
                    {
                        _CurrentTask = _BackgroundTaskFactory.CreateRefresherTask();
                        _CurrentTask.Start((successfull) => { });
                    }, x => !_StatusService.TaskIsRunning));
            }
        }

        private ICommand _SynchronizeWithVNDB;

        public ICommand SynchronizeWithVNDB
        {
            get
            {
                return _SynchronizeWithVNDB ??
                    (_SynchronizeWithVNDB = new RelayCommand(x =>
                    {
                        _CurrentTask = _BackgroundTaskFactory.CreateSynchronizerTask();
                        _CurrentTask.Start((successfull) => { });
                    }, x => !_StatusService.TaskIsRunning));
            }
        }
    }
}
