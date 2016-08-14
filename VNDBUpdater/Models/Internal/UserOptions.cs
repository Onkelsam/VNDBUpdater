using VNDBUpdater.Data;

namespace VNDBUpdater.Models.Internal
{
    public class UserOptions
    {
        public SpoilerSetting SpoilerSetting { get; set; }
        public bool ShowNSFWImages { get; set; }
        public bool MinimizeToTray { get; set; }
        public string InstallFolderPath { get; set; }
        public string StretchFormat { get; set; }
        public ControlVisibility OriginalNameTab { get; set; }
        public ControlVisibility VNDBVoteTab { get; set; }
        public ControlVisibility VNDBPopularityTab { get; set; }

        public UserOptions()
        {
            SpoilerSetting = SpoilerSetting.Hide;
            ShowNSFWImages = false;
            MinimizeToTray = false;
            InstallFolderPath = null;
            StretchFormat = "Uniform";
            OriginalNameTab = ControlVisibility.Collapsed;
            VNDBVoteTab =  ControlVisibility.Visible;
            VNDBPopularityTab = ControlVisibility.Visible;
        }
    }
}
