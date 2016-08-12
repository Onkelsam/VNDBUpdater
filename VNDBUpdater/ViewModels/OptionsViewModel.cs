using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VNDBUpdater.BackgroundTasks;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;

namespace VNDBUpdater.ViewModels
{
    class OptionsViewModel : ViewModelBase
    {
        private User _User;
        private MainViewModel _MainScreen;

        public OptionsViewModel(MainViewModel ViewModel)
            : base()
        {
            _MainScreen = ViewModel;

            _User = UserHelper.CurrentUser;

            _Commands.AddCommand("Login", ExecuteLoginCommand, CanExecuteSaveCommand);
            _Commands.AddCommand("SetInstallFolderPath", ExecuteSetInstallFolderPath, CanExecuteSetInstallFolderPath);
            _Commands.AddCommand("StartIndexing", ExecuteStartIndexing, CanExecuteStartIndexing);
            _Commands.AddCommand("Save", ExecuteSaveSettings);
        }

        public string Username
        {
            get { return _User.Username; }
            set
            {
                _User.Username = value;

                OnPropertyChanged(nameof(Username));
            }
        }

        public List<string> SpoilerLevels
        {
            get { return Enum.GetNames(typeof(SpoilerSetting)).ToList(); }
        }

        public string SpoilerLevel
        {
            get { return _User.Settings.SpoilerSetting.ToString(); }
            set
            {
                _User.Settings.SpoilerSetting = ExtensionMethods.ParseEnum<SpoilerSetting>(value);

                OnPropertyChanged(nameof(SpoilerLevel));
            }
        }

        public bool ShowNSFWImages
        {
            get { return _User.Settings.ShowNSFWImages; }
            set
            {
                _User.Settings.ShowNSFWImages = value;

                OnPropertyChanged(nameof(ShowNSFWImages));
            }
        }

        public bool MinimizeToTray
        {
            get { return _User.Settings.MinimizeToTray; }
            set
            {
                _User.Settings.MinimizeToTray = value;

                OnPropertyChanged(nameof(MinimizeToTray));
            }
        }

        public string InstallFolderPath
        {
            get { return _User.Settings.InstallFolderPath; }
            set
            {
                _User.Settings.InstallFolderPath = value;

                OnPropertyChanged(nameof(InstallFolderPath));
            }
        }

        public string LoginState
        {
            get { return VNDBCommunication.StatusString; }
        }

        public bool Fill
        {
            get { return _User.Settings.StretchFormat == ImageFormat.Fill ? true : false; }
            set
            {
                if (value)
                    _User.Settings.StretchFormat = ImageFormat.Fill;
                else
                    _User.Settings.StretchFormat = ImageFormat.Uniform;
            }
        }       

        public bool OriginalNameVisible
        {
            get { return _User.Settings.OriginalNameTab == ColumnVisibility.Visible ? true : false; }
            set
            {
                if (value)
                    _User.Settings.OriginalNameTab = ColumnVisibility.Visible;
                else
                    _User.Settings.OriginalNameTab = ColumnVisibility.Collapsed;
            }
        }

        public bool VNDBVoteVisible
        {
            get { return _User.Settings.VNDBVoteTab == ColumnVisibility.Visible ? true : false; }
            set
            {
                if (value)
                    _User.Settings.VNDBVoteTab = ColumnVisibility.Visible;
                else
                    _User.Settings.VNDBVoteTab = ColumnVisibility.Collapsed;
            }
        }

        public bool VNDBPopularityVisible
        {
            get { return _User.Settings.VNDBPopularityTab == ColumnVisibility.Visible ? true : false; }
            set
            {
                if (value)
                    _User.Settings.VNDBPopularityTab = ColumnVisibility.Visible;
                else
                    _User.Settings.VNDBPopularityTab = ColumnVisibility.Collapsed;
            }
        }

        public void ExecuteLoginCommand(object parameter)
        {
            if (UserHelper.CurrentUser.EncryptedPassword != null)
            {
                var dialogResult = MessageBox.Show("This will delete all local data! Are you sure you want to procede?", "WARNING", MessageBoxButton.YesNo);

                if (dialogResult == MessageBoxResult.No)
                    return;
            }
            
            if ((parameter as PasswordBox).SecurePassword.Length == 0)
                return;

            _User.EncryptedPassword = ProtectedData.Protect(Encoding.UTF8.GetBytes((parameter as PasswordBox).Password), null, DataProtectionScope.CurrentUser);

            Login();
        }

        public void ExecuteSetInstallFolderPath(object parameter)
        {
            _User.Settings.InstallFolderPath = FileHelper.GetFolderPath();
            _User.SaveUser();

            FileHelper.ResetFolders();

            OnPropertyChanged(nameof(InstallFolderPath));
        }

        public bool CanExecuteSetInstallFolderPath(object parameter)
        {
            if (FileIndexer.Status != TaskStatus.Running)
                return true;
            else
                return false;
        }

        public bool CanExecuteSaveCommand(object parameter)
        {
            if (_User.Username != null)
                if (!string.IsNullOrEmpty(_User.Username) && Synchronizer.Status != TaskStatus.Running && FileIndexer.Status != TaskStatus.Running && Refresher.Status != TaskStatus.Running)
                    return true;

            return false;
        }

        public void ExecuteStartIndexing(object parameter)
        {
            var indexer = new FileIndexer();
            indexer.Start(_MainScreen);
        }

        public bool CanExecuteStartIndexing(object parameter)
        {
            if (Synchronizer.Status != TaskStatus.Running && FileIndexer.Status != TaskStatus.Running && Refresher.Status != TaskStatus.Running)
                return true;
            else
                return false;
        }

        public void ExecuteSaveSettings(object parameter)
        {
            _User.SaveUser();
            (parameter as Window).Close();
        }

        private void Login()
        {
            RedisCommunication.Reconnect();
            _User.SaveUser();

            LocalVisualNovelHelper.RefreshVisualNovels();
            _MainScreen.UpdateVisualNovelGrid();
            
            VNDBCommunication.Reconnect();

            if (VNDBCommunication.Status == VNDBCommunicationStatus.LoggedIn)
            {
                Synchronizer.Cancel();
                var BackgroundSynchronizer = new Synchronizer();
                BackgroundSynchronizer.Start(_MainScreen);
            }

            _MainScreen.UpdateStatusText();
            OnPropertyChanged(nameof(LoginState));
        }
    }
}
