using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Data;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class UserOptionsModel
    {
        public UserOptionsModel()
        {
            SpoilerSetting = SpoilerSetting.Hide;
            ShowNSFWImages = false;
            MinimizeToTray = false;
            InstallFolderPath = null;
        }

        public UserOptionsModel(UserOptionsEntity entity)
        {
            SpoilerSetting = entity.SpoilerSetting;
            ShowNSFWImages = entity.ShowNSFWImages;
            MinimizeToTray = entity.MinimizeToTray;
            InstallFolderPath = entity.InstallFolderPath;
        }

        public SpoilerSetting SpoilerSetting
        {
            get;
            set;
        }

        public bool ShowNSFWImages
        {
            get;
            set;
        }

        public bool MinimizeToTray
        {
            get;
            set;
        }

        public string InstallFolderPath
        {
            get;
            set;
        }
    }
}
