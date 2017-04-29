using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.CustomClasses.Collections;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Helper;
using VNDBUpdater.Services.Dialogs;
using VNDBUpdater.Services.Filters;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class VNDatagridViewModel : ViewModelBase, IVisualNovelsGridWindowModel
    {
        private UserModel _User;

        private IDialogCoordinator _DialogCoordinator;
        private IVNService _VNService;
        private IUserService _UserService;
        private IFilterService _FilterService;
        private IDialogService _DialogService;

        private Dictionary<VisualNovelModel.VisualNovelCatergory, AsyncObservableCollection<VisualNovelModel>> _TabMapper;

        private Dictionary<SortingMethod, string> _SortToPropertyMapper = new Dictionary<SortingMethod, string>()
        {
            { SortingMethod.Playtime, "PlayTime" },
            { SortingMethod.Title, "Basics.Title" },
            { SortingMethod.Release, "Basics.Release" },
            { SortingMethod.Popularity, "Basics.Popularity" },
            { SortingMethod.Rating, "Basics.Rating" },
        };

        private Dictionary<VisualNovelModel.VisualNovelCatergory, string> _CategoryToPropertyMapper = new Dictionary<VisualNovelModel.VisualNovelCatergory, string>()
        {
            { VisualNovelModel.VisualNovelCatergory.Unknown, "VisualNovelsInUnknownGroup" },
            { VisualNovelModel.VisualNovelCatergory.Dropped, "VisualNovelsInDroppedGroup" },
            { VisualNovelModel.VisualNovelCatergory.Finished, "VisualNovelsInFinishedGroup" },
            { VisualNovelModel.VisualNovelCatergory.Stalled, "VisualNovelsInStalledGroup" },
            { VisualNovelModel.VisualNovelCatergory.Playing, "VisualNovelsInPlayingGroup" },
        };

        public enum SortingMethod
        {
            [Description("Playtime")]
            Playtime = 0,
            [Description("Title")]
            Title,
            [Description("Release Date")]
            Release,
            [Description("Rating")]
            Rating,
            [Description("Popularity")]
            Popularity,
        };

        public enum SortingDirection
        {
            [Description("Ascending")]
            Ascending = 0,
            [Description("Descending")]
            Descending,
        }

        private static object _SyncLock = new object();

        public VNDatagridViewModel(IVNService VNService, IUserService UserService, IDialogCoordinator DialogCoordinator, IFilterService FilterService, IDialogService DialogService)
            : base()
        {
            _VNService = VNService;
            _UserService = UserService;
            _FilterService = FilterService;
            _DialogCoordinator = DialogCoordinator;
            _DialogService = DialogService;

            _User = _UserService.Get();

            _SelectedSortingMethod = _User.GUI.SelectedSortingMethod;
            _SelectedSortingDirection = _User.GUI.SelectedSortingDirection;

            _SelectedVisualNovel = new VisualNovelModel();

            _VisualNovels = new AsyncObservableCollection<VisualNovelModel>(_VNService.GetLocal());

            SetupVisualNovels();

            _VNService.SubscribeToTAdded(OnVisualNovelAdded);
            _VNService.SubscribeToTDeleted(OnVisualNovelDeleted);
            _VNService.SubscribeToTUpdated(OnVisualNovelUpdated);
            _VNService.SubscribeToRefreshAll(OnAllVisualNovelsRefreshed);

            _UserService.SubscribeToTUpdated(OnUserUpdated);

            _FilterService.SubscribeToFilterApply(OnFilterApplied);
            _FilterService.SubscribeToFilterReset(OnAllVisualNovelsRefreshed);
        }

        private void OnFilterApplied(FilterModel Filter)
        {
            var passed = new List<VisualNovelModel>();

            foreach (var vn in _VisualNovels)
            {
                if (!_FilterService.VNShouldBeFilteredOut(Filter, vn))
                {
                    passed.Add(vn);
                }
            }

            _VisualNovels = new AsyncObservableCollection<VisualNovelModel>(passed);

            SetupVisualNovels();
        }

        private void OnUserUpdated(UserModel User)
        {
            _User = User;

            OnPropertyChanged(nameof(ImageDimension));
            OnPropertyChanged(nameof(StackPanelDimension));
        }

        private void OnAllVisualNovelsRefreshed()
        {
            _VisualNovels = new AsyncObservableCollection<VisualNovelModel>(_VNService.GetLocal());

            SetupVisualNovels();
        }

        private void OnVisualNovelAdded(VisualNovelModel model)
        {
            _VisualNovels.Add(model);
            _TabMapper[model.Category].Add(model);            

            OnPropertyChanged(_CategoryToPropertyMapper[model.Category]);
        }

        private void OnVisualNovelDeleted(VisualNovelModel model)
        {
            _VisualNovels.Remove(model);
            _TabMapper[model.Category].Remove(model);

            OnPropertyChanged(_CategoryToPropertyMapper[model.Category]);
        }

        private void OnVisualNovelUpdated(VisualNovelModel model)
        {
            _VisualNovels.Remove(model);
            _VisualNovels.Add(model);
                        
            _TabMapper[model.Category].Remove(model);
            _TabMapper[model.Category].Add(model);

            OnPropertyChanged(_CategoryToPropertyMapper[model.Category]);
        }

        private AsyncObservableCollection<VisualNovelModel> _VisualNovels;

        public AsyncObservableCollection<VisualNovelModel> VisualNovels
        {
            get { return _VisualNovels; }
            set
            {
                _VisualNovels = value;

                OnPropertyChanged(nameof(VisualNovels));
                OnPropertyChanged(nameof(VisualNovelsInUnknownGroup));
                OnPropertyChanged(nameof(VisualNovelsInDroppedGroup));
                OnPropertyChanged(nameof(VisualNovelsInFinishedGroup));
                OnPropertyChanged(nameof(VisualNovelsInPlayingGroup));
                OnPropertyChanged(nameof(VisualNovelsInStalledGroup));
            }
        }

        public AsyncObservableCollection<VisualNovelModel> VisualNovelsInUnknownGroup
        {
            get { return GetOrderedVisualNovels(VisualNovelModel.VisualNovelCatergory.Unknown); }               
        }

        public AsyncObservableCollection<VisualNovelModel> VisualNovelsInPlayingGroup
        {
            get { return GetOrderedVisualNovels(VisualNovelModel.VisualNovelCatergory.Playing); }
        }

        public AsyncObservableCollection<VisualNovelModel> VisualNovelsInFinishedGroup
        {
            get { return GetOrderedVisualNovels(VisualNovelModel.VisualNovelCatergory.Finished); }
        }

        public AsyncObservableCollection<VisualNovelModel> VisualNovelsInStalledGroup
        {
            get { return GetOrderedVisualNovels(VisualNovelModel.VisualNovelCatergory.Stalled); }
        }

        public AsyncObservableCollection<VisualNovelModel> VisualNovelsInDroppedGroup
        {
            get { return GetOrderedVisualNovels(VisualNovelModel.VisualNovelCatergory.Dropped); }
        }

        private VisualNovelModel _SelectedVisualNovel;

        public VisualNovelModel SelectedVisualNovel
        {
            get { return _SelectedVisualNovel; }
            set
            {
                _SelectedVisualNovel = value;

                OnPropertyChanged(nameof(SelectedVisualNovel));
                OnPropertyChanged(nameof(Relations));
                OnPropertyChanged(nameof(RelationsInVisualNovelList));
            }
        }

        public int ImageDimension
        {
            get { return _User.GUI.ImageDimension; }
        }

        public int StackPanelDimension
        {
            get { return _User.GUI.ImageDimension + 60; }
        }

        public List<RelationModel> Relations
        {
            get { return _SelectedVisualNovel?.Basics?.Relations?.Where(x => !_VisualNovels.Any(y => y.Basics.ID == x.ID)).ToList(); }
        }

        public List<RelationModel> RelationsInVisualNovelList
        {
            get { return _SelectedVisualNovel?.Basics?.Relations?.Where(x => _VisualNovels.Any(y => y.Basics.ID == x.ID)).ToList(); }
        }

        public List<string> Categories
        {
            get { return Enum.GetNames(typeof(VisualNovelModel.VisualNovelCatergory)).ToList(); }
        }

        public List<string> SortingMethods
        {
            get { return Enum.GetNames(typeof(SortingMethod)).Select(x => ExtensionMethods.GetAttributeOfType<DescriptionAttribute>((SortingMethod)Enum.Parse(typeof(SortingMethod), x))).Select(x => x.Description).ToList(); }
        }

        private SortingMethod _SelectedSortingMethod;

        public string SelectedSortingMethod
        {
            get { return _SelectedSortingMethod.GetAttributeOfType<DescriptionAttribute>().Description.ToString(); }
            set
            {
                _SelectedSortingMethod = value.GetEnumValueFromDescription<SortingMethod>();

                OnPropertyChanged(nameof(SelectedSortingMethod));
                OnPropertyChanged(nameof(VisualNovelsInUnknownGroup));
                OnPropertyChanged(nameof(VisualNovelsInDroppedGroup));
                OnPropertyChanged(nameof(VisualNovelsInFinishedGroup));
                OnPropertyChanged(nameof(VisualNovelsInPlayingGroup));
                OnPropertyChanged(nameof(VisualNovelsInStalledGroup));

                _User.GUI.SelectedSortingMethod = value.GetEnumValueFromDescription<SortingMethod>();
                _UserService.Update(_User);
            }
        }

        public List<string> SortingDirections
        {
            get { return Enum.GetNames(typeof(SortingDirection)).Select(x => ExtensionMethods.GetAttributeOfType<DescriptionAttribute>((SortingDirection)Enum.Parse(typeof(SortingDirection), x))).Select(x => x.Description).ToList(); }
        }

        private SortingDirection _SelectedSortingDirection;

        public string SelectedSortingDirection
        {
            get { return _SelectedSortingDirection.GetAttributeOfType<DescriptionAttribute>().Description.ToString(); }
            set
            {
                SortingDirection direction = value.GetEnumValueFromDescription<SortingDirection>();

                _SelectedSortingDirection = direction;

                OnPropertyChanged(nameof(SelectedSortingDirection));
                OnPropertyChanged(nameof(VisualNovelsInUnknownGroup));
                OnPropertyChanged(nameof(VisualNovelsInDroppedGroup));
                OnPropertyChanged(nameof(VisualNovelsInFinishedGroup));
                OnPropertyChanged(nameof(VisualNovelsInPlayingGroup));
                OnPropertyChanged(nameof(VisualNovelsInStalledGroup));

                _User.GUI.SelectedSortingDirection = direction;
                _UserService.Update(_User);
            }
        }

        private AsyncObservableCollection<VisualNovelModel> GetOrderedVisualNovels(VisualNovelModel.VisualNovelCatergory category)
        {
            if (_SelectedSortingDirection == SortingDirection.Ascending)
            {
                return new AsyncObservableCollection<VisualNovelModel>(_TabMapper[category].OrderBy(x => x.FollowPropertyPath(_SortToPropertyMapper[_SelectedSortingMethod]).FollowPropertyPathAndGetValue(x, _SortToPropertyMapper[_SelectedSortingMethod])));
            }
            else
            {
                return new AsyncObservableCollection<VisualNovelModel>(_TabMapper[category].OrderByDescending(x => x.FollowPropertyPath(_SortToPropertyMapper[_SelectedSortingMethod]).FollowPropertyPathAndGetValue(x, _SortToPropertyMapper[_SelectedSortingMethod])));
            }            
        }

        private void SetupVisualNovels()
        {
            _TabMapper = new Dictionary<VisualNovelModel.VisualNovelCatergory, AsyncObservableCollection<VisualNovelModel>>();

            _TabMapper.Add(VisualNovelModel.VisualNovelCatergory.Unknown, new AsyncObservableCollection<VisualNovelModel>(_VisualNovels.Where(x => x.Category == VisualNovelModel.VisualNovelCatergory.Unknown)));
            _TabMapper.Add(VisualNovelModel.VisualNovelCatergory.Playing, new AsyncObservableCollection<VisualNovelModel>(_VisualNovels.Where(x => x.Category == VisualNovelModel.VisualNovelCatergory.Playing)));
            _TabMapper.Add(VisualNovelModel.VisualNovelCatergory.Dropped, new AsyncObservableCollection<VisualNovelModel>(_VisualNovels.Where(x => x.Category == VisualNovelModel.VisualNovelCatergory.Dropped)));
            _TabMapper.Add(VisualNovelModel.VisualNovelCatergory.Stalled, new AsyncObservableCollection<VisualNovelModel>(_VisualNovels.Where(x => x.Category == VisualNovelModel.VisualNovelCatergory.Stalled)));
            _TabMapper.Add(VisualNovelModel.VisualNovelCatergory.Finished, new AsyncObservableCollection<VisualNovelModel>(_VisualNovels.Where(x => x.Category == VisualNovelModel.VisualNovelCatergory.Finished)));

            foreach (var entry in _TabMapper)
            {
                _TabMapper[entry.Key].CollectionChanged += OnCollectionChanged;

                BindingOperations.EnableCollectionSynchronization(_TabMapper[entry.Key], _SyncLock);

                OnPropertyChanged(_CategoryToPropertyMapper[entry.Key]);
            }
        }

        private ICommand _StartVisualNovel;

        public ICommand StartVisualNovel
        {
            get
            {
                return _StartVisualNovel ??
                    (_StartVisualNovel = new RelayCommand(x => _VNService.Start(_SelectedVisualNovel), x => _SelectedVisualNovel != null && _VNService.InstallationPathExists(_SelectedVisualNovel)));
            }
        }

        private ICommand _OpenVisualNovelFolder;

        public ICommand OpenVisualNovelFolder
        {
            get
            {
                return _OpenVisualNovelFolder ??
                    (_OpenVisualNovelFolder = new RelayCommand(x => _VNService.OpenFolder(_SelectedVisualNovel), x => _SelectedVisualNovel != null && _VNService.InstallationPathExists(_SelectedVisualNovel)));
            }
        }

        private ICommand _SetExePath;

        public ICommand SetExePath
        {
            get
            {
                return _SetExePath ??
                    (_SetExePath = new RelayCommand(x => _VNService.SetExePath(_SelectedVisualNovel, _DialogService.GetPathToExecuteable())));
            }
        }

        private ICommand _OpenWalkthrough;

        public ICommand OpenWalkthrough
        {
            get
            {
                return _OpenWalkthrough ??
                    (_OpenWalkthrough = new RelayCommand(x => _VNService.OpenWalkthrough(_SelectedVisualNovel), x => _SelectedVisualNovel != null && _VNService.WalkthroughAvailable(_SelectedVisualNovel)));
            }
        }

        private ICommand _CreateWalkthrough;

        public ICommand CreateWalkthrough
        {
            get
            {
                return _CreateWalkthrough ??
                    (_CreateWalkthrough = new RelayCommand(x => _VNService.CreateWalkthrough(_SelectedVisualNovel), x => _SelectedVisualNovel != null && _VNService.InstallationPathExists(_SelectedVisualNovel) && !_VNService.WalkthroughAvailable(_SelectedVisualNovel)));
            }
        }

        private ICommand _ViewOnVNDB;

        public ICommand ViewOnVNDB
        {
            get
            {
                return _ViewOnVNDB ??
                    (_ViewOnVNDB = new RelayCommand(x => _VNService.ViewOnVNDB(_SelectedVisualNovel), x => _SelectedVisualNovel != null));
            }
        }

        private ICommand _UpdateVisualNovel;

        public ICommand UpdateVisualNovel
        {
            get
            {
                return _UpdateVisualNovel ??
                    (_UpdateVisualNovel = new RelayCommand(x => { _VNService.Update(_SelectedVisualNovel); OnPropertyChanged(nameof(SelectedVisualNovel)); }, x => _SelectedVisualNovel != null));
            }
        }

        private ICommand _DeleteVisualNovel;

        public ICommand DeleteVisualNovel
        {
            get
            {
                return _DeleteVisualNovel ??
                    (_DeleteVisualNovel = new RelayCommand(x => { _VNService.Delete(_SelectedVisualNovel); }, x => _SelectedVisualNovel != null));
            }
        }

        private ICommand _SearchOnline;

        public ICommand SearchOnline
        {
            get
            {
                return _SearchOnline ??
                    (_SearchOnline = new RelayCommand((parameter) => _VNService.SearchOnGoggle(_SelectedVisualNovel, parameter.ToString()), x => _SelectedVisualNovel != null));
            }
        }

        private ICommand _SetScore;

        public ICommand SetScore
        {
            get
            {
                return _SetScore ?? (_SetScore = new RelayCommand(
                     x =>
                    {
                         _DialogCoordinator.ShowInputAsync(this, "Set Score", "Enter your score:").ContinueWith(y => ExecuteSetScore(y.Result));
                    },
                    x => _SelectedVisualNovel != null));
            }
        }

        private ICommand _SetCategory;

        public ICommand SetCategory
        {
            get
            {
                return _SetCategory ??
                    (_SetCategory = new RelayCommand((parameter) => ExecuteSetCategory(parameter), x => _SelectedVisualNovel != null));
            }
        }

        private ICommand _ViewRelationOnVNDB;

        public ICommand ViewRelationOnVNDB
        {
            get
            {
                return _ViewRelationOnVNDB ??
                    (_ViewRelationOnVNDB = new RelayCommand((parameter) => _VNService.ViewRelationOnVNDB(_SelectedVisualNovel, parameter.ToString())));
            }
        }

        private ICommand _SelectVisualNovel;

        public ICommand SelectVisualNovel
        {
            get
            {
                return _SelectVisualNovel ??
                    (_SelectVisualNovel = new RelayCommand((parameter) => SelectedVisualNovel = VisualNovels.First(x => x.Basics.Title == parameter.ToString())));
            }
        }

        private void ExecuteSetCategory(object parameter)
        {
            var category = (VisualNovelModel.VisualNovelCatergory)Enum.Parse(typeof(VisualNovelModel.VisualNovelCatergory), parameter.ToString(), true);
            var oldCategory = _SelectedVisualNovel.Category;

            _VNService.SetCategory(_SelectedVisualNovel, category);

            _TabMapper[oldCategory].Remove(_SelectedVisualNovel);

            OnPropertyChanged(_CategoryToPropertyMapper[oldCategory]);
        }

        private void ExecuteSetScore(string score)
        {
            if (string.IsNullOrEmpty(score))
            {
                return;
            }

            score = score.Contains(',') ? score.Replace(',', '.') : score;

            if (score.Contains('.'))
            {
                double convertedScore;

                if (double.TryParse(score, NumberStyles.Any, CultureInfo.InvariantCulture, out convertedScore))
                {
                    _VNService.SetScore(_SelectedVisualNovel, Convert.ToInt32(10 * convertedScore)); 
                }
            }
            else
            {
                int convertedScore;

                if (int.TryParse(score, NumberStyles.Any, CultureInfo.InvariantCulture, out convertedScore))
                {
                    _VNService.SetScore(_SelectedVisualNovel, 10 * convertedScore);
                }
            }

            OnPropertyChanged(nameof(SelectedVisualNovel));
        }
    }
}
