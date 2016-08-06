using System;
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

namespace VNDBUpdater.ViewModels
{
    class OptionsViewModel : ViewModelBase
    {
        private string _Username;

        private MainViewModel MainScreen;

        public OptionsViewModel(MainViewModel ViewModel)
            : base()
        {
            MainScreen = ViewModel;

            _Username = RedisCommunication.GetUsername();

            _Commands.AddCommand("Save", ExecuteSaveCommand, CanExecuteSaveCommand);
            _Commands.AddCommand("SetInstallFolderPath", ExecuteSetInstallFolderPath, CanExecuteSetInstallFolderPath);
            _Commands.AddCommand("StartIndexing", ExecuteStartIndexing, CanExecuteStartIndexing);
        }

        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;

                OnPropertyChanged(nameof(Username));
            }
        }

        public string InstallFolderPath
        {
            get { return RedisCommunication.GetInstallFolder(); }
        }

        public void ExecuteSaveCommand(object parameter)
        {
            if (RedisCommunication.GetUserPassword() != null)
            {
                var dialogResult = MessageBox.Show("This will delete all local data! Are you sure you want to procede?", "WARNING", MessageBoxButton.YesNo);

                if (dialogResult == MessageBoxResult.No)
                    return;
            }
            
            if ((parameter as PasswordBox).SecurePassword.Length == 0)
                return;

            byte[] encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes((parameter as PasswordBox).Password), null, DataProtectionScope.CurrentUser);

            RedisCommunication.ResetDatabase();
            LocalVisualNovelHelper.ResetVisualNovels();
            MainScreen.AllVisualNovels = LocalVisualNovelHelper.LocalVisualNovels;
            MainScreen.UpdateVisualNovelGrid();

            RedisCommunication.SetUserCredentials(_Username, Convert.ToBase64String(encryptedBytes));            

            VNDBCommunication.Disconnect();
            VNDBCommunication.Connect();

            if (VNDBCommunication.Status == VNDBCommunicationStatus.LoggedIn)
            {
                Synchronizer.Cancel();
                var BackgroundSynchronizer = new Synchronizer();
                BackgroundSynchronizer.Start(MainScreen);
            }

            MainScreen.UpdateStatusText();
        }

        public void ExecuteSetInstallFolderPath(object parameter)
        {
            RedisCommunication.SetInstallFolder(FileHelper.GetFolderPath());
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
            if (_Username != null)
                if (!string.IsNullOrEmpty(_Username) && Synchronizer.Status != TaskStatus.Running && FileIndexer.Status != TaskStatus.Running && Refresher.Status != TaskStatus.Running)
                    return true;

            return false;
        }

        public void ExecuteStartIndexing(object parameter)
        {
            var indexer = new FileIndexer();
            indexer.Start(MainScreen);
        }

        public bool CanExecuteStartIndexing(object parameter)
        {
            if (Synchronizer.Status != TaskStatus.Running && FileIndexer.Status != TaskStatus.Running && Refresher.Status != TaskStatus.Running)
                return true;
            else
                return false;
        }
    }
}
