using VNDBUpdater.Data;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class UserOptionsEntity
    {
        public SpoilerSetting SpoilerSetting { get; set; }
        public bool ShowNSFWImages { get; set; }
        public bool MinimizeToTray { get; set; }
        public string InstallFolderPath { get; set; }

        public UserOptionsEntity()
        {
            ShowNSFWImages = false;
            MinimizeToTray = false;
            InstallFolderPath = null;
            SpoilerSetting = SpoilerSetting.Hide;
        }

        public UserOptionsEntity(UserOptionsModel model)
        {
            SpoilerSetting = model.SpoilerSetting;
            ShowNSFWImages = model.ShowNSFWImages;
            MinimizeToTray = model.MinimizeToTray;
            InstallFolderPath = model.InstallFolderPath;
        }
    }
}
