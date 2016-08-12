using VNDBUpdater.Data;

namespace VNDBUpdater.Models.Internal
{
    public class GUISettings
    {
        public VisualNovelCatergory SelectedVNCategory { get; set; }
        public TagCategory SelectedTagCategory { get; set; }

        public GUISettings()
        {
            SelectedVNCategory = VisualNovelCatergory.Unknown;
            SelectedTagCategory = TagCategory.All;
        }
    }
}
