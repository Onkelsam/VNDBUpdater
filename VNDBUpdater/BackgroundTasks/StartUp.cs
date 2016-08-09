using System;
using System.IO;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.BackgroundTasks
{
    public class StartUp : BackgroundTask
    {
        private static TaskStatus _Status = TaskStatus.WaitingToRun;

        public static TaskStatus Status
        {
            get { return _Status; }
        }

        public static string StatusString
        {
            get
            {
                switch(_Status)
                {
                    case (TaskStatus.Running):
                        return "Currently loading database.";
                    case (TaskStatus.Faulted):
                        return "Error occured while loading database. Please check the eventlog.";
                    default:
                        return string.Empty;
                }
            }
        }

        public override void Start(MainViewModel MainScreen)
        {
            if (_Status != TaskStatus.Running)
            {
                base.Start(MainScreen);

                EventLogger.LogInformation(nameof(StartUp), "Started. Version: " + VersionHelper.CurrentVersion + " New Version available: " + VersionHelper.NewVersionAvailable().ToString());                

                _Status = TaskStatus.Running;

                _BackgroundTask = new Task(StartUpProgram, _CancelToken);
                _BackgroundTask.Start();
            }
        }

        public static void Cancel()
        {
            if (_Status == TaskStatus.Running)
            {
                _CancelTokenSource.Cancel();
            }
        }

        private void StartUpProgram()
        {
            try
            {
                FileHelper.BackupDatabase();
                FileHelper.DeleteTooLargeFile(Constants.EventlogFileName, 1000000);

                if (!File.Exists(Constants.TagsJsonFileName))
                    Tag.RefreshTags();

                if (!File.Exists(Constants.TraitsJsonFileName))
                    Trait.RefreshTraits();

                LocalVisualNovelHelper.RefreshVisualNovels();
                
                _MainScreen.GetVisualNovelsFromDatabase();

                if (RedisCommunication.UserCredentialsAvailable())
                {
                    var BackgroundSynchronizer = new Synchronizer();
                    BackgroundSynchronizer.Start(_MainScreen);
                }

                _Status = TaskStatus.RanToCompletion;
                _MainScreen.UpdateStatusText();
                Cancel();

                EventLogger.LogInformation(nameof(StartUp), "finished successfully.");                
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                EventLogger.LogError(nameof(StartUp), ex);
            }
        }
    }
}
