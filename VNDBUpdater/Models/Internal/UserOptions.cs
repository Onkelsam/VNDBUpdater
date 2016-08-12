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
        public ColumnVisibility OriginalNameTab { get; set; }
        public ColumnVisibility VNDBVoteTab { get; set; }
        public ColumnVisibility VNDBPopularityTab { get; set; }

        public UserOptions()
        {
            SpoilerSetting = SpoilerSetting.Hide;
            ShowNSFWImages = false;
            MinimizeToTray = false;
            InstallFolderPath = null;
            StretchFormat = "Fill";
            OriginalNameTab = ColumnVisibility.Collapsed;
            VNDBVoteTab =  ColumnVisibility.Visible;
            VNDBPopularityTab = ColumnVisibility.Visible;
        }
    }
}
