using System;
using System.IO;
using System.Threading.Tasks;
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
                        return nameof(StartUp) + Constants.TaskRunning;
                    case (TaskStatus.Faulted):
                        return nameof(StartUp) + Constants.TaskFaulted;
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

                EventLogger.LogInformation(nameof(StartUp) + ":" + nameof(Start), Constants.TaskStarted + "Version: " + VersionHelper.CurrentVersion + " New Version available: " + VersionHelper.NewVersionAvailable().ToString());                

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

                if (!File.Exists(Constants.TagsJsonFileName))
                    Tag.RefreshTags();

                if (!File.Exists(Constants.TraitsJsonFileName))
                    Trait.RefreshTraits();
               
                if (UserHelper.CurrentUser.Username != null)
                {
                    var BackgroundSynchronizer = new Synchronizer();
                    BackgroundSynchronizer.Start(_MainScreen);
                }

                _Status = TaskStatus.RanToCompletion;
                _MainScreen.UpdateStatusText();

                EventLogger.LogInformation(nameof(StartUp) + ":" + nameof(StartUpProgram), Constants.TaskFinished);
            }
            catch (Exception ex)
            {
                _Status = TaskStatus.Faulted;
                EventLogger.LogError(nameof(StartUp) + ":" + nameof(StartUpProgram), ex);
            }
        }
    }
}
