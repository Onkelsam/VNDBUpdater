using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using VNDBUpdater.BackgroundTasks;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.Views;

namespace VNDBUpdater.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private List<VisualNovel> _AllVisualNovels;

        private ProperObservableCollection<VisualNovel> _VisualNovelsInGrid;
        private ObservableCollection<Tag> _TagsInGrid;
        private List<int> _Scores = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private VisualNovel _SelectedVisualNovel;
        private List<Filter> _AvailableFilters;
        private Filter _AppliedFilter;

        private int _SelectedVisualNovelTab;
        private int _SelectedTagTab;

        private string _SelectedScreenshot;
        private string _SelectedCharacter;

        private int _CurrentPendingTasks;
        private int _CompletedPendingTasks;

        public MainViewModel()
            : base()
        {
            _AllVisualNovels = new List<VisualNovel>();
            _VisualNovelsInGrid = new ProperObservableCollection<VisualNovel>();
            _TagsInGrid = new ObservableCollection<Tag>();
            _SelectedVisualNovel = new VisualNovel();
            _AvailableFilters = new List<Filter>();
            _AppliedFilter = new Filter();

            _Commands.AddCommand("RefreshVisualNovels", ExecuteRefreshVisualNovels, CanExecuteVNDBOperations);
            _Commands.AddCommand("NextScreenshot", ExecuteNextScreenshot);
            _Commands.AddCommand("NextCharacter", ExecuteNextCharacter);
            _Commands.AddCommand("StartVisualNovel", ExecuteStartVisualNovel, CanExecuteStartVisualNovel);
            _Commands.AddCommand("OpenVisualNovelFolder", ExecuteOpenVisualNovelFolder, CanExecuteOpenVisualNovelFolder);
            _Commands.AddCommand("ViewVisualNovelOnVNDB", ExecuteViewVisualNovelOnVNDB, VisualNovelSelected);
            _Commands.AddCommand("OpenWalkthrough", ExecuteOpenWalkthrough, CanExecuteOpenWalkthrough);
            _Commands.AddCommand("AddVisualNovels", ExecuteAddVisualNovels, CanExecuteVNDBOperations);
            _Commands.AddCommand("SetCategory", ExecuteSetCategory, CanExecuteVisualNovelChanges);
            _Commands.AddCommand("DeleteVisualNovel", ExecuteDeleteVisualNovel, CanExecuteVisualNovelChanges);
            _Commands.AddCommand("CloseWindow", ExecuteCloseWindow);
            _Commands.AddCommand("SetExePath", ExecuteSetExePath, CanExecuteVisualNovelChanges);
            _Commands.AddCommand("UpdateVisualNovel", ExecuteUpdateVisualNovel, CanExecuteVisualNovelChanges);
            _Commands.AddCommand("AddFilter", ExecuteAddFilter);
            _Commands.AddCommand("ApplyFilter", ExecuteApplyFilter);
            _Commands.AddCommand("DeleteFilter", ExecuteDeleterFilter);
            _Commands.AddCommand("ResetFilters", ExecuteResetFilters);
            _Commands.AddCommand("SearchOnGoogle", ExecuteSearchOnGoogle, VisualNovelSelected);
            _Commands.AddCommand("OpenOptions", ExecuteOpenOptions);
            _Commands.AddCommand("SetScore", ExecuteSetScore, CanExecuteVisualNovelChanges);
            _Commands.AddCommand("SynchronizeWithVNDB", ExecuteSynchronizeWithVNDB, CanExecuteVNDBOperations);
            _Commands.AddCommand("About", ExecuteAboutCommand);
            _Commands.AddCommand("CreateWalkthrough", ExecuteCreateWalkthrough, CanExecuteCreateWalkthrough);

            _VisualNovelsInGrid.CollectionChanged += OnCollectionChanged;
            _TagsInGrid.CollectionChanged += OnCollectionChanged;
        }

        public List<VisualNovel> AllVisualNovels
        {
            get { return _AllVisualNovels; }
            set { _AllVisualNovels = value; }
        }

        public ProperObservableCollection<VisualNovel> VisualNovelsInGrid
        {
            get { return _VisualNovelsInGrid; }
            set { _VisualNovelsInGrid = value; }
        }

        public ObservableCollection<Tag> TagsInGrid
        {
            get { return _TagsInGrid; }
            set { _TagsInGrid = value; }
        }

        public List<int> Scores
        {
            get { return _Scores; }
        }

        public List<string> Categories
        {
            get { return Enum.GetNames(typeof(VisualNovelCatergory)).ToList(); }
        }

        public VisualNovel SelectedVisualNovel
        {
            get { return _SelectedVisualNovel; }
            set
            {
                _SelectedVisualNovel = value;

                UpdateTagGrid();
                SetInitialScreenshot();
                SetInitialCharacter();

                OnPropertyChanged(nameof(SelectedVisualNovel));
            }
        }

        public int SelectedVisualNovelTab
        {
            get { return _SelectedVisualNovelTab; }
            set
            {
                _SelectedVisualNovelTab = value;

                UpdateVisualNovelGrid();

                OnPropertyChanged(nameof(SelectedVisualNovelTab));
            }
        }

        public int SelectedTagTab
        {
            get { return _SelectedTagTab; }
            set
            {
                _SelectedTagTab = value;

                UpdateTagGrid();

                OnPropertyChanged(nameof(SelectedTagTab));
            }
        }

        public string SelectedScreenshot
        {
            get { return _SelectedScreenshot; }
            set
            {
                _SelectedScreenshot = value;

                OnPropertyChanged(nameof(SelectedScreenshot));
            }
        }

        public string SelectedCharacter
        {
            get { return _SelectedCharacter; }
            set
            {
                _SelectedCharacter = value;

                OnPropertyChanged(nameof(SelectedCharacter));
            }
        }

        public List<Filter> AvailableFilters
        {
            get { return RedisCommunication.GetFiltersFromDatabase(); }
            set
            {
                _AvailableFilters = value;

                OnPropertyChanged(nameof(AvailableFilters));
            }
        }

        public string StatusText
        {
            get
            {
                return VNDBCommunication.StatusString + " " + Synchronizer.StatusString + " " + FileIndexer.StatusString;
            }
        }

        public int CurrentPendingTasks
        {
            get { return _CurrentPendingTasks; }
            set
            {
                _CurrentPendingTasks = value;

                OnPropertyChanged(nameof(CurrentPendingTasks));
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public int CompletedPendingTasks
        {
            get { return _CompletedPendingTasks; }
            set
            {
                _CompletedPendingTasks = value;

                OnPropertyChanged(nameof(CompletedPendingTasks));
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public void AddOnUI<T>(ICollection<T> collection, T item)
        {
            Action<T> add = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(add, item);
        }

        public void GetVisualNovelsFromDatabase()
        {
            _AllVisualNovels.Clear();

            _AllVisualNovels.AddRange(VisualNovelHelper.LocalVisualNovels);

            UpdateVisualNovelGrid();
        }

        public void ExecuteRefreshVisualNovels(object parameter)
        {            
            var dialogResult = MessageBox.Show("Are you sure? This can take a long time while the program won't be responding.", "Refresh", MessageBoxButton.YesNo);

            if (dialogResult == MessageBoxResult.Yes)
            {
                Tag.RefreshTags();

                var updatedVNs = VNDBCommunication.FetchVisualNovels(_AllVisualNovels.Select(x => x.Basics.id).ToList());

                foreach (var updatedVN in updatedVNs)
                {
                    var vnToUpdate = _AllVisualNovels.Where(x => x.Basics.id == updatedVN.Basics.id).First();

                    vnToUpdate.Basics = updatedVN.Basics;
                    vnToUpdate.Characters = updatedVN.Characters;
                }

                UpdateVisualNovelGrid();
            }
        }

        public bool CanExecuteVNDBOperations(object parameter)
        {
            if (Synchronizer.Status == BackgroundTaskState.Running ||
                !RedisCommunication.UserCredentialsAvailable() ||
                VNDBCommunication.Status == VNDBCommunicationStatus.NotLoggedIn ||
                VNDBCommunication.Status == VNDBCommunicationStatus.Error ||
                FileIndexer.Status == BackgroundTaskState.Running)
                return false;
            else
                return true;
        }

        public void ExecuteUpdateVisualNovel(object parameter)
        {
            _SelectedVisualNovel.Update();
        }

        public void ExecuteNextScreenshot(object parameter)
        {
            SelectedScreenshot = _SelectedVisualNovel.NextScreenshot(SelectedScreenshot);
        }

        public void ExecuteNextCharacter(object paramter)
        {
            SelectedCharacter = _SelectedVisualNovel.NextCharacter(SelectedCharacter);
        }

        public bool CanExecuteStartVisualNovel(object paramter)
        {
            if (VisualNovelSelected(null))
                return _SelectedVisualNovel.InstallationPathExists;
            else
                return false;
        }

        public void ExecuteCreateWalkthrough(object parameter)
        {
            _SelectedVisualNovel.CreateWalkthrough();
        }

        public void ExecuteStartVisualNovel(object paramter)
        {
            _SelectedVisualNovel.StartGame();
        }

        public bool CanExecuteCreateWalkthrough(object parameter)
        {
            return VisualNovelSelected(null) && !_SelectedVisualNovel.WalkthroughAvailable && CanExecuteStartVisualNovel(null);
        }

        public bool CanExecuteOpenVisualNovelFolder(object parameter)
        {
            if (VisualNovelSelected(null))
                return _SelectedVisualNovel.InstallationPathExists;
            else
                return false;
        }

        public void ExecuteOpenVisualNovelFolder(object parameter)
        {
            _SelectedVisualNovel.OpenGameFolder();
        }

        public void ExecuteViewVisualNovelOnVNDB(object paramter)
        {
            _SelectedVisualNovel.ViewOnVNDB();
        }

        public bool CanExecuteOpenWalkthrough(object paramter)
        {
            if (VisualNovelSelected(null))
                return _SelectedVisualNovel.WalkthroughAvailable;
            else
                return false;
        }

        public void ExecuteOpenWalkthrough(object paramter)
        {
            _SelectedVisualNovel.OpenWalkthrough();
        }

        public void ExecuteAddVisualNovels(object parameter)
        {
            var window = new AddVisualNovels(_AllVisualNovels.ToList());
            window.Closing += OnWindowAddVisualNovels_Closing;
            window.Show();
        }

        private void OnWindowAddVisualNovels_Closing(object sender, CancelEventArgs e)
        {
            (sender as AddVisualNovels).Closing -= OnWindowAddVisualNovels_Closing;
            GetVisualNovelsFromDatabase();
        }

        public void ExecuteDeleteVisualNovel(object parameter)
        {
            if (_SelectedVisualNovel.Delete())
            {
                _AllVisualNovels.Remove(_AllVisualNovels.First(x => x.Basics.id == _SelectedVisualNovel.Basics.id));
                UpdateVisualNovelGrid();
            }
        }

        public bool VisualNovelSelected(object parameter)
        {
            if ((_SelectedVisualNovel != null) && (_SelectedVisualNovel.Basics != null))
                return true;
            else
                return false;
        }

        public bool CanExecuteVisualNovelChanges(object parameter)
        {
            return VisualNovelSelected(null) && CanExecuteVNDBOperations(null); 
        }

        public void UpdateStatusText()
        {
            OnPropertyChanged(nameof(StatusText));
        }

        public void ExecuteSetCategory(object parameter)
        {
            _SelectedVisualNovel.SetCategory(parameter.ToString());
            UpdateVisualNovelInDB(_SelectedVisualNovel);
            UpdateVisualNovelGrid();
        }

        public void ExecuteSetScore(object parameter)
        {
            _SelectedVisualNovel.SetScore((int)parameter);
            UpdateVisualNovelInDB(_SelectedVisualNovel);
            UpdateVisualNovelGrid();
        }

        public void ExecuteCloseWindow(object parameter)
        {
            Synchronizer.Cancel();
            FileIndexer.Cancel();
            RedisCommunication.SaveRedis();
            RedisCommunication.Dispose();
            VNDBCommunication.Dispose();
        }

        public void ExecuteSetExePath(object parameter)
        {
            var visualNovel = parameter as VisualNovel;
            visualNovel.SetExePath();

            UpdateVisualNovelInDB(visualNovel);
        }

        public void ExecuteAddFilter(object parameter)
        {
            var window = new CreateFilter();
            window.Closing += OnWindowAddAndEditFilter_Closing;
            window.Show();
        }

        private void OnWindowAddAndEditFilter_Closing(object sender, CancelEventArgs e)
        {
            (sender as CreateFilter).Closing -= OnWindowAddAndEditFilter_Closing;
            AvailableFilters = null;
        }

        public void ExecuteDeleterFilter(object parameter)
        {
            var filter = parameter as Filter;

            filter.Delete();

            AvailableFilters = RedisCommunication.GetFiltersFromDatabase();
        }

        public void ExecuteAboutCommand(object parameter)
        {
            var about = new About();
            about.Show();
        }

        public void ExecuteSynchronizeWithVNDB(object parameter)
        {
            Synchronizer.Cancel();
            var BackgroundSynchronizer = new Synchronizer();
            BackgroundSynchronizer.Start(this);
        }

        public void ExecuteResetFilters(object parameter)
        {
            GetVisualNovelsFromDatabase();
        }

        public void ExecuteSearchOnGoogle(object parameter)
        {
            _SelectedVisualNovel.SearchOnGoolge(parameter.ToString());
        }

        public void ExecuteApplyFilter(object parameter)
        {
            var filter = parameter as Filter;
            var filteredVNs = new List<VisualNovel>();

            foreach (var vn in _AllVisualNovels)
            {
                if (!filter.ShouldVNBeFilteredOut(vn))
                    filteredVNs.Add(vn);
            }

            _AllVisualNovels = filteredVNs;
            UpdateVisualNovelGrid();
        }

        public void ExecuteOpenOptions(object parameter)
        {
            var options = new Options(this);
            options.Show();
        }
        
        private void UpdateVisualNovelInDB(VisualNovel visualNovel)
        {
            if (visualNovel != null)
                RedisCommunication.AddVisualNovelToDB(visualNovel);
        }

        private void UpdateTagGrid()
        {
            TagsInGrid.Clear();

            if (_SelectedVisualNovel != null)
            {
                if (_SelectedVisualNovel.Basics != null)
                {
                    var selectedTab = (TagCategory)SelectedTagTab;

                    foreach (var tag in _SelectedVisualNovel.Basics.tags)
                    {
                        Tag foundTag = TagHelper.LocalTags.FirstOrDefault(x => x.ID == tag[0]);

                        if (foundTag != null)
                        {
                            if (foundTag.Category == selectedTab || selectedTab == TagCategory.All)
                            {
                                foundTag.Score = tag[1];
                                _TagsInGrid.Add(foundTag);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateVisualNovelGrid()
        {
            VisualNovelsInGrid.Clear();

            foreach (var visualNovel in _AllVisualNovels.Where(x => x.Category == (VisualNovelCatergory)SelectedVisualNovelTab))
                VisualNovelsInGrid.Add(visualNovel);
        }

        private void SetInitialScreenshot()
        {
            if (_SelectedVisualNovel != null)
                SelectedScreenshot = _SelectedVisualNovel.Basics.image;
            else
                SelectedScreenshot = string.Empty;
        }

        private void SetInitialCharacter()
        {
            if ((_SelectedVisualNovel != null) && (_SelectedVisualNovel.Characters != null))
            {
                if (_SelectedVisualNovel.Characters.Any(x => x.image != null))
                    SelectedCharacter = _SelectedVisualNovel.Characters[0].image;
                else
                    SelectedCharacter = _SelectedVisualNovel.Basics.image;
            }
            else
                SelectedCharacter = string.Empty;
        }
    }
}
