using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VNDBUpdater.BackgroundTasks;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.Models.Internal;
using VNDBUpdater.Views;

namespace VNDBUpdater.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private AsyncObservableCollection<VisualNovel> _VisualNovelsInGrid;
        private AsyncObservableCollection<Tag> _TagsInGrid;
        private VisualNovel _SelectedVisualNovel;
        private List<Filter> _AvailableFilters;
        private Filter _AppliedFilter;
        private WindowHandler _WindowHandler;

        private int _SelectedVisualNovelTab;
        private int _SelectedTagTab;

        private VNScreenshot _SelectedScreenshot;
        private CharacterInformation _SelectedCharacter;

        private int _CurrentPendingTasks;
        private int _CompletedPendingTasks;

        private static object _SyncLock = new object(); 

        public MainViewModel()
            : base()
        {
            _VisualNovelsInGrid = new AsyncObservableCollection<VisualNovel>();
            _TagsInGrid = new AsyncObservableCollection<Tag>();
            _SelectedVisualNovel = new VisualNovel();
            _AvailableFilters = new List<Filter>();
            _AppliedFilter = new Filter();
            _WindowHandler = new WindowHandler();

            _Commands.AddCommand("RefreshVisualNovels", ExecuteRefreshVisualNovels, CanExecuteVNDBOperations);
            _Commands.AddCommand("NextScreenshot", ExecuteNextScreenshot, VisualNovelSelected);
            _Commands.AddCommand("NextCharacter", ExecuteNextCharacter, VisualNovelSelected);
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
            _Commands.AddCommand("SetCustomScore", ExecuteSetCustomScore, CanExecuteVisualNovelChanges);
            _Commands.AddCommand("ShowMainWindow", ExecuteShowMainWindow);
            _Commands.AddCommand("StateChanged", ExecuteStateChanged);

            _VisualNovelsInGrid.CollectionChanged += OnCollectionChanged;            
            _TagsInGrid.CollectionChanged += OnCollectionChanged;

            BindingOperations.EnableCollectionSynchronization(_VisualNovelsInGrid, _SyncLock);
            BindingOperations.EnableCollectionSynchronization(_TagsInGrid, _SyncLock);
        }

        public AsyncObservableCollection<VisualNovel> VisualNovelsInGrid
        {
            get { return _VisualNovelsInGrid; }
            set { _VisualNovelsInGrid = value; }
        }

        public AsyncObservableCollection<Tag> TagsInGrid
        {
            get { return _TagsInGrid; }
            set { _TagsInGrid = value; }
        }

        public List<int> Scores
        {
            get { return Constants.PossibleScores; }
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
                _SelectedCharacter = null;
                _SelectedScreenshot = null;

                UpdateTagGrid();

                if (VisualNovelSelected(null))
                {
                    ExecuteNextCharacter(null);
                    ExecuteNextScreenshot(null);
                }
                
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

        public VNScreenshot SelectedScreenshot
        {
            get { return _SelectedScreenshot; }
            set
            {
                _SelectedScreenshot = value;

                OnPropertyChanged(nameof(SelectedScreenshot));
            }
        }

        public CharacterInformation SelectedCharacter
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
                if (StartUp.Status == TaskStatus.Running)
                    return StartUp.StatusString;
                else
                    return VNDBCommunication.StatusString + " " + Synchronizer.StatusString + " " + FileIndexer.StatusString + " " + Refresher.StatusString;
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

        public string Title
        {
            get
            {
                if (VersionHelper.NewVersionAvailable())
                    return "VNDBUpdater " + VersionHelper.CurrentVersion + "  New Version available! Check the 'About'-window.";
                else
                    return "VNDBUpdater " + VersionHelper.CurrentVersion;
            }
        }

        public void ExecuteRefreshVisualNovels(object parameter)
        {            
            Tag.RefreshTags();
            Trait.RefreshTraits();

            var refresher = new Refresher();
            refresher.Start(this);
        }

        public bool CanExecuteVNDBOperations(object parameter)
        {
            if (Synchronizer.Status == TaskStatus.Running ||
                UserHelper.CurrentUser.EncryptedPassword == null ||
                VNDBCommunication.Status == VNDBCommunicationStatus.NotLoggedIn ||
                VNDBCommunication.Status == VNDBCommunicationStatus.Error ||
                FileIndexer.Status == TaskStatus.Running ||
                Refresher.Status == TaskStatus.Running ||
                StartUp.Status == TaskStatus.Running)
                return false;
            else
                return true;
        }

        public void ExecuteUpdateVisualNovel(object parameter)
        {
            _SelectedVisualNovel.Update();
            OnPropertyChanged(nameof(SelectedVisualNovel));
        }

        public void ExecuteNextScreenshot(object parameter)
        {
            SelectedScreenshot = _SelectedVisualNovel.GetNextScreenshot(SelectedScreenshot);
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
            _WindowHandler.Open(new AddVisualNovels(LocalVisualNovelHelper.LocalVisualNovels), OnWindowAddVisualNovels_Closing);
        }

        private void OnWindowAddVisualNovels_Closing(object sender, CancelEventArgs e)
        {
            (sender as AddVisualNovels).Closing -= OnWindowAddVisualNovels_Closing;
            UpdateVisualNovelGrid();
        }

        public void ExecuteDeleteVisualNovel(object parameter)
        {
            if (_SelectedVisualNovel.Delete())
            {
                LocalVisualNovelHelper.RemoveVisualNovel(_SelectedVisualNovel);
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
            UpdateVisualNovelGrid();
        }

        public void ExecuteSetScore(object parameter)
        {
            _SelectedVisualNovel.SetScore((int)parameter * 10);
            UpdateVisualNovelGrid();
        }

        public void ExecuteCloseWindow(object parameter)
        {
            try
            {
                StartUp.Cancel();
                Refresher.Cancel();
                Synchronizer.Cancel();
                FileIndexer.Cancel();
                RedisCommunication.SaveRedis();
                RedisCommunication.Dispose();
                VNDBCommunication.Dispose();
                _WindowHandler.CloseAllWindows();
                EventLogger.LogInformation(nameof(MainViewModel), "Shutdown successfull.");                
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                EventLogger.LogError(nameof(MainViewModel), ex);
                EventLogger.LogInformation(nameof(MainViewModel), "Shutdown failed.");
            }            
        }

        public void ExecuteSetExePath(object parameter)
        {
            var visualNovel = parameter as VisualNovel;
            visualNovel.SetExePath();
        }

        public void ExecuteAddFilter(object parameter)
        {
            _WindowHandler.Open(new CreateFilter(), OnWindowAddAndEditFilter_Closing);
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
            _WindowHandler.Open(new About());
        }

        public void ExecuteSynchronizeWithVNDB(object parameter)
        {
            Synchronizer.Cancel();
            var BackgroundSynchronizer = new Synchronizer();
            BackgroundSynchronizer.Start(this);
        }

        public void ExecuteResetFilters(object parameter)
        {
            foreach (var vn in LocalVisualNovelHelper.LocalVisualNovels)
                vn.IsFilteredOut = false;

            UpdateVisualNovelGrid();
        }

        public void ExecuteSearchOnGoogle(object parameter)
        {
            _SelectedVisualNovel.SearchOnGoolge(parameter.ToString());
        }

        public void ExecuteApplyFilter(object parameter)
        {
            var filter = parameter as Filter;
            var editedVNs = new List<VisualNovel>();

            foreach (var vn in LocalVisualNovelHelper.LocalVisualNovels)
            {
                vn.IsFilteredOut = filter.ShouldVNBeFilteredOut(vn);
                editedVNs.Add(vn);
            }

            LocalVisualNovelHelper.AddVisualNovels(editedVNs);

            UpdateVisualNovelGrid();
        }

        public void ExecuteOpenOptions(object parameter)
        {
            _WindowHandler.Open(new Options(this));
        }
        
        private void UpdateTagGrid()
        {
            TagsInGrid.Clear();

            if (_SelectedVisualNovel != null)
            {
                if (_SelectedVisualNovel.Basics.ConvertedTags != null)
                {
                    foreach (var tag in _SelectedVisualNovel.Basics.ConvertedTags)
                        if (tag.Category == (TagCategory)SelectedTagTab || (TagCategory)SelectedTagTab == TagCategory.All)
                            if (tag.ShowTag())
                                TagsInGrid.Add(tag);
                }
            }
        }

        public void UpdateVisualNovelGrid()
        {
            VisualNovelsInGrid.Clear();

            foreach (var visualNovel in LocalVisualNovelHelper.LocalVisualNovels.Where(x => x.Category == (VisualNovelCatergory)SelectedVisualNovelTab && x.IsFilteredOut == false))
                VisualNovelsInGrid.Add(visualNovel);
        }

        public void ExecuteSetCustomScore(object parameter)
        {
            string score = Microsoft.VisualBasic.Interaction.InputBox("Enter custom score:", "Score", string.Empty).Replace(',','.');
            double scoreResult;

            if (double.TryParse(score, NumberStyles.Any, CultureInfo.InvariantCulture, out scoreResult))
            {
                int result = (int)(scoreResult * 10);

                if (result < 0 || result > 100)
                    return;

                _SelectedVisualNovel.SetScore(result);

                UpdateVisualNovelGrid();
            }
        }

        public void ExecuteShowMainWindow(object parameter)
        {
            _WindowHandler.Show(parameter);         
        }

        public void ExecuteStateChanged(object parameter)
        {
            var window = (parameter as MainWindow);

            if (window.WindowState == WindowState.Minimized)
                _WindowHandler.Minimize(window);
        }   
    }
}
