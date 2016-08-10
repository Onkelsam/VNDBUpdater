using VNDBUpdater.Data;

namespace VNDBUpdater.Models.Internal
{
    public class UserOptions
    {
        public SpoilerSetting SpoilerSetting { get; set; }
        public bool ShowNSFWImages { get; set; }
        public bool MinimizeToTray { get; set; }
        public string InstallFolderPath { get; set; }

        public UserOptions()
        {
            SpoilerSetting = SpoilerSetting.Hide;
            ShowNSFWImages = false;
            MinimizeToTray = false;
            InstallFolderPath = null;
        }
    }
}
