using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VNDBUpdater.BackgroundTasks;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;

namespace VNDBUpdater.ViewModels
{
    public class FileIndexerViewModel : ViewModelBase
    {
        private MainViewModel _MainScreen;

        private bool _AdvancedModeActivated = false;

        private VisualNovel _SelectedVisualNovel;
        private string _SelectedFolder;
        private string _SelectedExcludedFolder;
        private string _SelectedExcludedExe;

        public FileIndexerViewModel(MainViewModel MainScreen)
        {
            _MainScreen = MainScreen;

            Commands.AddCommand("AddFolderToSearch", ExecuteAddFolderToSearch);
            Commands.AddCommand("RemoveFolderFromSearch", ExecuteRemoveFolderFromSearch, CanExecuteRemoveFolderFromSearch);
            Commands.AddCommand("SaveSettings", ExecuteSaveSettings);
            Commands.AddCommand("StartIndexing", ExecuteStartIndexing, CanExecuteStartIndexing);
            Commands.AddCommand("AddExcludedFolder", ExecuteAddExcludedFolder);
            Commands.AddCommand("RemoveExcludedFolder", ExecuteRemoveExcludedFolder, CanExecuteRemoveExcludedFolder);
            Commands.AddCommand("AddExcludedExe", ExecuteAddExecludedExe);
            Commands.AddCommand("RemoveExcludedExe", ExecuteRemoveExecludedExe, CanExecuteRemoveExecludedExe);
            Commands.AddCommand("SetExePath", ExecuteSetExePath, CanExecuteSetExePath);
            Commands.AddCommand("SetToDefault", ExecuteSetToDefault);
            Commands.AddCommand("ResetIndexedVNs", ExecuteResetIndexedVNs);
        }

        public bool AdvancedModeActivated
        {
            get { return _AdvancedModeActivated; }
            set
            {
                _AdvancedModeActivated = value;

                OnPropertyChanged(nameof(AdvancedModeActivated));
            }
        }

        public ObservableCollection<string> Folders
        {
            get { return new ObservableCollection<string>(UserHelper.CurrentUser.FileIndexerSetting.Folders.OrderBy(x => x)); }
        }

        public string SelectedFolder
        {
            get { return _SelectedFolder; }
            set
            {
                _SelectedFolder = value;

                OnPropertyChanged(nameof(SelectedFolder));
            }
        }

        public ObservableCollection<string> ExcludedFolders
        {
            get { return new ObservableCollection<string>(UserHelper.CurrentUser.FileIndexerSetting.ExcludedFolders.OrderBy(x => x)); }
        }

        public string SelectedExcludedFolder
        {
            get { return _SelectedExcludedFolder; }
            set
            {
                _SelectedExcludedFolder = value;

                OnPropertyChanged(nameof(SelectedExcludedFolder));
            }
        }

        public ObservableCollection<string> ExcludedExeNames
        {
            get { return new ObservableCollection<string>(UserHelper.CurrentUser.FileIndexerSetting.ExcludedExes.OrderBy(x => x)); }
        }

        public string SelectedExcludedExe
        {
            get { return _SelectedExcludedExe; }
            set
            {
                _SelectedExcludedExe = value;

                OnPropertyChanged(nameof(SelectedExcludedExe));
            }
        }

        public ObservableCollection<VisualNovel> NonIndexedVisualNovels
        {
            get { return new ObservableCollection<VisualNovel>(new List<VisualNovel>(LocalVisualNovelHelper.LocalVisualNovels.Where(x => x.ExePath == null || x.ExePath == "")).OrderBy(x => x.Basics.VNDBInformation.title)); }
        }

        public VisualNovel SelectedVisualNovel
        {
            get { return _SelectedVisualNovel; }
            set
            {
                _SelectedVisualNovel = value;

                OnPropertyChanged(nameof(SelectedVisualNovel));
            }
        }

        public string MinimalFolderLength
        {
            get { return UserHelper.CurrentUser.FileIndexerSetting.MinFolderLength.ToString(); }
            set
            {
                int result;

                if (Int32.TryParse(value, out result))
                {
                    if (result >= 0)
                        UserHelper.CurrentUser.FileIndexerSetting.MinFolderLength = result;
                }

                OnPropertyChanged(nameof(MinimalFolderLength));  
            }
        }

        public string MaxDeviation
        {
            get { return UserHelper.CurrentUser.FileIndexerSetting.MaxDeviation.ToString(); }
            set
            {
                int result;

                if (Int32.TryParse(value, out result))
                {
                    if (result >= 0)
                        UserHelper.CurrentUser.FileIndexerSetting.MaxDeviation = result;
                }

                OnPropertyChanged(nameof(MaxDeviation));    
            }
        }

        public void ExecuteAddFolderToSearch(object parameter)
        {
            string folder = FileHelper.GetFolderPath();

            if (!string.IsNullOrEmpty(folder) && !UserHelper.CurrentUser.FileIndexerSetting.ExcludedFolders.Any(x => x == folder))
                UserHelper.CurrentUser.FileIndexerSetting.Folders.Insert(0, folder);

            OnPropertyChanged(nameof(Folders));
        }

        public void ExecuteRemoveFolderFromSearch(object parameter)
        {
            if (UserHelper.CurrentUser.FileIndexerSetting.Folders.Any(x => x == SelectedFolder))
                UserHelper.CurrentUser.FileIndexerSetting.Folders.Remove(UserHelper.CurrentUser.FileIndexerSetting.Folders.Where(x => x == SelectedFolder).Select(x => x).First());

            OnPropertyChanged(nameof(Folders));
        }

        public bool CanExecuteRemoveFolderFromSearch(object paramter)
        {
            return SelectedFolder == null ? false : true;
        }

        public void ExecuteSaveSettings(object parameter)
        {
            UserHelper.CurrentUser.SaveUser();
        }

        public void ExecuteStartIndexing(object parameter)
        {
            var Indexer = new FileIndexer();
            Indexer.Start(_MainScreen, this);
        }

        public bool CanExecuteStartIndexing(object parameter)
        {
            if (UserHelper.CurrentUser.FileIndexerSetting.Folders.Any() && 
                FileIndexer.Status != System.Threading.Tasks.TaskStatus.Running &&
                Refresher.Status != System.Threading.Tasks.TaskStatus.Running &&
                Synchronizer.Status != System.Threading.Tasks.TaskStatus.Running)
                return true;
            else
                return false;
        }

        public void ExecuteAddExcludedFolder(object parameter)
        {
            string folder = FileHelper.GetFolderPath();

            if (!string.IsNullOrEmpty(folder) && !UserHelper.CurrentUser.FileIndexerSetting.ExcludedFolders.Any(x => x == folder))
                UserHelper.CurrentUser.FileIndexerSetting.ExcludedFolders.Insert(0, folder);

            OnPropertyChanged(nameof(ExcludedFolders));
        }

        public void ExecuteRemoveExcludedFolder(object parameter)
        {
            if (UserHelper.CurrentUser.FileIndexerSetting.ExcludedFolders.Any(x => x == SelectedExcludedFolder))
                UserHelper.CurrentUser.FileIndexerSetting.ExcludedFolders.Remove(UserHelper.CurrentUser.FileIndexerSetting.ExcludedFolders.Where(x => x == SelectedExcludedFolder).Select(x => x).First());

            OnPropertyChanged(nameof(ExcludedFolders));
        }

        public bool CanExecuteRemoveExcludedFolder(object parameter)
        {
            return SelectedExcludedFolder == null ? false : true;
        }

        public void ExecuteAddExecludedExe(object parameter)
        {
            string exe = Microsoft.VisualBasic.Interaction.InputBox("Enter exe name (without extension):", "Exclude Exe", string.Empty);

            if (!string.IsNullOrEmpty(exe))
            {
                if (!UserHelper.CurrentUser.FileIndexerSetting.ExcludedExes.Any(x => x.ToLower().Trim() == exe.ToLower().Trim()))
                    UserHelper.CurrentUser.FileIndexerSetting.ExcludedExes.Insert(0, exe);
            }

            OnPropertyChanged(nameof(ExcludedExeNames));       
        }

        public void ExecuteRemoveExecludedExe(object paramter)
        {
            if (UserHelper.CurrentUser.FileIndexerSetting.ExcludedExes.Any(x => x == _SelectedExcludedExe))
                UserHelper.CurrentUser.FileIndexerSetting.ExcludedExes.Remove(UserHelper.CurrentUser.FileIndexerSetting.ExcludedExes.Where(x => x == SelectedExcludedExe).Select(x => x).First());

            OnPropertyChanged(nameof(ExcludedExeNames));
        }

        public bool CanExecuteRemoveExecludedExe(object parameter)
        {
            return SelectedExcludedExe == null ? false : true;
        }

        public void ExecuteSetExePath(object parameter)
        {
            _SelectedVisualNovel.ExePath = FileHelper.GetExePath();

            OnPropertyChanged(nameof(NonIndexedVisualNovels));
        }

        public bool CanExecuteSetExePath(object parameter)
        {
            return _SelectedVisualNovel != null ? true : false;
        }

        public void ExecuteSetToDefault(object paramter)
        {
            UserHelper.CurrentUser.FileIndexerSetting.SetDefault();

            OnPropertyChanged(nameof(ExcludedExeNames));
            OnPropertyChanged(nameof(MinimalFolderLength));
            OnPropertyChanged(nameof(MaxDeviation));
        }

        public void ExecuteResetIndexedVNs(object parameter)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "", MessageBoxButton.YesNo);

            if (dialogResult == MessageBoxResult.Yes)
            {
                foreach (var vn in LocalVisualNovelHelper.LocalVisualNovels)
                    vn.ExePath = string.Empty;

                OnPropertyChanged(nameof(NonIndexedVisualNovels));
            }
        }
    }
}
