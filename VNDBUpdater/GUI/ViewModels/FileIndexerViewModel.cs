using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VNDBUpdater.BackgroundTasks.Interfaces;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.CustomClasses.Collections;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.Dialogs;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.GUI.ViewModels
{
    public class FileIndexerViewModel : ViewModelBase, IFileIndexerWindowModel
    {
        private UserModel _User;
        private SynchronizationContext _Context = SynchronizationContext.Current;

        private ITaskFactory _BackgroundTaskFactory;

        private IDialogCoordinator _DialogCoordinator;

        private IVNService _VNService;
        private IUserService _UserService;
        private IStatusService _StatusService;
        private IDialogService _DialogService;

        public FileIndexerViewModel(IMenuBarWindowModel MenuBar, IDialogCoordinator dialogCoordniator, ITaskFactory taskFactory, IVNService VNService, IUserService UserService, IStatusService StatusService, IDialogService DialogService)
        {
            _DialogCoordinator = dialogCoordniator;
            _BackgroundTaskFactory = taskFactory;
            _VNService = VNService;
            _UserService = UserService;
            _StatusService = StatusService;
            _DialogService = DialogService;

            _VNService.OnAdded += OnVisualNovelAdded;
            _VNService.OnDeleted += OnVisualNovelDeleted;
            _VNService.OnUpdated += OnVisualNovelUpdated;
            _VNService.OnRefreshed += OnVisualNovelsRefreshed;            

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            _User = await _UserService.GetAsync();

            OnPropertyChanged(nameof(Folders));
            OnPropertyChanged(nameof(ExcludedFolders));
            OnPropertyChanged(nameof(ExcludedExeNames));
            OnPropertyChanged(nameof(MinimalFolderLength));
            OnPropertyChanged(nameof(MaxDeviation));

            var vns = await _VNService.GetLocalAsync();

            NonIndexedVisualNovels = new AsyncObservableCollection<VisualNovelModel>(vns.Where(x => string.IsNullOrEmpty(x.ExePath)).OrderBy(x => x.Basics.Title), _Context);
        }

        private void OnVisualNovelAdded(object sender, VisualNovelModel model)
        {
            if (!_VNService.CheckIfInstallationPathExists(model) && !NonIndexedVisualNovels.Any(x => x.Basics.ID == model.Basics.ID))
            {
                NonIndexedVisualNovels.Add(model);
            }
        }

        private void OnVisualNovelDeleted(object sender, VisualNovelModel model)
        {
            if (NonIndexedVisualNovels.Any(x => x.Basics.ID == model.Basics.ID))
            {
                NonIndexedVisualNovels.Remove(model);
            }
        }

        private void OnVisualNovelUpdated(object sender, VisualNovelModel model)
        {
            if (_VNService.CheckIfInstallationPathExists(model) && NonIndexedVisualNovels.Any(x => x.Basics.ID == model.Basics.ID))
            {
                NonIndexedVisualNovels.Remove(model);
            }
            else if (!_VNService.CheckIfInstallationPathExists(model) && !NonIndexedVisualNovels.Any(x => x.Basics.ID == model.Basics.ID))
            {
                NonIndexedVisualNovels.Add(model);
            }
        }

        private async void OnVisualNovelsRefreshed(object sender, EventArgs e)
        {
            var vns = await _VNService.GetLocalAsync();

            NonIndexedVisualNovels = new AsyncObservableCollection<VisualNovelModel>(vns.Where(x => string.IsNullOrEmpty(x.ExePath)).OrderBy(x => x.Basics.Title), _Context);
        }

        private bool _AdvancedModeActivated = false;

        public bool AdvancedModeActivated
        {
            get { return _AdvancedModeActivated; }
            set { AdvancedModeActivated = value;  OnPropertyChanged(nameof(AdvancedModeActivated)); }
        }

        public ObservableCollection<string> Folders
        {
            get { return new ObservableCollection<string>(_User.FileIndexerSetting.Folders.OrderBy(x => x)); }
        }

        public ObservableCollection<string> ExcludedFolders
        {
            get { return new ObservableCollection<string>(_User.FileIndexerSetting.ExcludedFolders.OrderBy(x => x)); }
        }

        public ObservableCollection<string> ExcludedExeNames
        {
            get { return new ObservableCollection<string>(_User.FileIndexerSetting.ExcludedExes.OrderBy(x => x)); }
        }

        private AsyncObservableCollection<VisualNovelModel> _NonIndexedVisualNovels;

        public AsyncObservableCollection<VisualNovelModel> NonIndexedVisualNovels
        {
            get { return _NonIndexedVisualNovels; }
            private set { _NonIndexedVisualNovels = value;  OnPropertyChanged(nameof(NonIndexedVisualNovels)); }
        }

        private VisualNovelModel _SelectedVisualNovel;

        public VisualNovelModel SelectedVisualNovel
        {
            get { return _SelectedVisualNovel; }
            set { _SelectedVisualNovel = value;  OnPropertyChanged(nameof(SelectedVisualNovel)); }
        }

        private string _SelectedItem;

        public string SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged(nameof(SelectedItem)); }
        }

        public string MinimalFolderLength
        {
            get { return _User.FileIndexerSetting.MinFolderLength.ToString(); }
            set
            {
                int result;

                if (Int32.TryParse(value, out result))
                {
                    if (result >= 0)
                    {
                        _User.FileIndexerSetting.MinFolderLength = result;
                        _UserService.UpdateAsync(_User);

                        OnPropertyChanged(nameof(MinimalFolderLength));
                    }                        
                }                                
            }
        }

        public string MaxDeviation
        {
            get { return _User.FileIndexerSetting.MaxDeviation.ToString(); }
            set
            {
                int result;

                if (Int32.TryParse(value, out result))
                {
                    if (result >= 0)
                    {
                        _User.FileIndexerSetting.MaxDeviation = result;
                        _UserService.UpdateAsync(_User);

                        OnPropertyChanged(nameof(MaxDeviation));
                    }                        
                }                
            }
        }

        private ICommand _Add;

        public ICommand Add
        {
            get
            {
                return _Add ??
                    (_Add = new RelayCommand(async (parameter) =>
                    {
                        switch (parameter.ToString())
                        {
                            case ("FoldersToSearch"):
                                _User.FileIndexerSetting.Folders.Add(_DialogService.GetPathToFolder());
                                await _UserService.UpdateAsync(_User);
                                break;
                            case ("Excluded"):
                                _User.FileIndexerSetting.ExcludedFolders.Add(_DialogService.GetPathToFolder());
                                await _UserService.UpdateAsync(_User);
                                break;
                            default:
                                await _DialogCoordinator.ShowInputAsync(this, "Select Exe", "Enter exe without extension.").ContinueWith(y => AddExe(y.Result));
                                break;
                        }
                        OnPropertyChanged("");
                    }));                
            }
        }

        private async Task AddExe(string exe)
        {
            if (!string.IsNullOrEmpty(exe))
            {
                _User.FileIndexerSetting.ExcludedExes.Add(exe);
                await _UserService.UpdateAsync(_User);
            }
        }

        private ICommand _Remove;

        public ICommand Remove
        {
            get
            {
                return _Remove ??
                    (_Remove = new RelayCommand(async (parameter) =>
                    {
                        switch (parameter.ToString())
                        {
                            case ("FoldersToSearch"):
                                _User.FileIndexerSetting.Folders.Remove(_SelectedItem);
                                await _UserService.UpdateAsync(_User);
                                break;
                            case ("Excluded"):
                                _User.FileIndexerSetting.ExcludedFolders.Remove(_SelectedItem);
                                await _UserService.UpdateAsync(_User);
                                break;
                            default:
                                _User.FileIndexerSetting.ExcludedExes.Remove(_SelectedItem);
                                await _UserService.UpdateAsync(_User);
                                break;
                        }
                        OnPropertyChanged("");
                    }, x => _SelectedItem != null));
            }
        }

        private ICommand _StartIndexing;

        public ICommand StartIndexing
        {
            get
            {
                return _StartIndexing ??
                    (_StartIndexing = new RelayCommand(async x =>
                    {
                        IBackgroundTask task = _BackgroundTaskFactory.CreateFileIndexerTask();
                        await task.ExecuteTaskAsync((successfull) => {; });
                    }, x => !_StatusService.TaskIsRunning));                    
            }
        }

        private ICommand _SetExePath;

        public ICommand SetExePath
        {
            get
            {
                return _SetExePath ??
                    (_SetExePath = new RelayCommand(async x => 
                    {
                        await _VNService.SetExePathAsync(_SelectedVisualNovel, _DialogService.GetPathToExecuteable());
                        NonIndexedVisualNovels.Remove(_SelectedVisualNovel);
                    }, x => _SelectedVisualNovel != null));
            }
        }

        private ICommand _SetToDefaulSettings;
        
        public ICommand SetToDefaultSettings
        {
            get
            {
                return _SetToDefaulSettings ??
                    (_SetToDefaulSettings = new RelayCommand(async x =>
                    {
                        _User.FileIndexerSetting.SetDefault();
                        await _UserService.UpdateAsync(_User);

                        OnPropertyChanged(nameof(ExcludedExeNames));
                        OnPropertyChanged(nameof(MinimalFolderLength));
                        OnPropertyChanged(nameof(MaxDeviation));
                    }));
            }
        }

        private ICommand _ResetIndexedVNs;

        public ICommand ResetIndexedVNs
        {
            get
            {
                return _ResetIndexedVNs ??
                    (_ResetIndexedVNs = new RelayCommand(async x =>
                    {
                        var vnsToReset = new List<VisualNovelModel>();

                        foreach (var vn in await _VNService.GetLocalAsync())
                        {
                            vn.ExePath = string.Empty;
                            vn.FolderPath = string.Empty;

                            vnsToReset.Add(vn);
                        }

                        await _VNService.AddAsync(vnsToReset);
                    }));
            }
        }

        private async Task ExecuteAddExecludedExe(string exe)
        {
            if (!string.IsNullOrEmpty(exe))
            {
                if (!_User.FileIndexerSetting.ExcludedExes.Any(x => string.Equals(x, exe)))
                {
                    _User.FileIndexerSetting.ExcludedExes.Insert(0, exe);
                    await _UserService.UpdateAsync(_User);

                    OnPropertyChanged(nameof(ExcludedExeNames));
                }
            }
        }
    }
}
