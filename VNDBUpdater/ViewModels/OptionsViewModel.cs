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

        public string LoginState
        {
            get { return VNDBCommunication.StatusString; }
        }

        public bool Fill
        {
            get { return _User.Settings.StretchFormat == "Fill" ? true : false; }
            set
            {
                if (value)
                    _User.Settings.StretchFormat = "Fill";
                else
                    _User.Settings.StretchFormat = "Uniform";
            }
        }       

        public bool OriginalNameVisible
        {
            get { return _User.Settings.OriginalNameTab == Data.ControlVisibility.Visible ? true : false; }
            set
            {
                if (value)
                    _User.Settings.OriginalNameTab = Data.ControlVisibility.Visible;
                else
                    _User.Settings.OriginalNameTab = Data.ControlVisibility.Collapsed;
            }
        }

        public bool VNDBVoteVisible
        {
            get { return _User.Settings.VNDBVoteTab == Data.ControlVisibility.Visible ? true : false; }
            set
            {
                if (value)
                    _User.Settings.VNDBVoteTab = Data.ControlVisibility.Visible;
                else
                    _User.Settings.VNDBVoteTab = Data.ControlVisibility.Collapsed;
            }
        }

        public bool VNDBPopularityVisible
        {
            get { return _User.Settings.VNDBPopularityTab == Data.ControlVisibility.Visible ? true : false; }
            set
            {
                if (value)
                    _User.Settings.VNDBPopularityTab = Data.ControlVisibility.Visible;
                else
                    _User.Settings.VNDBPopularityTab = Data.ControlVisibility.Collapsed;
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

        public bool CanExecuteSaveCommand(object parameter)
        {
            if (_User.Username != null)
                if (!string.IsNullOrEmpty(_User.Username) && Synchronizer.Status != TaskStatus.Running && FileIndexer.Status != TaskStatus.Running && Refresher.Status != TaskStatus.Running)
                    return true;

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
