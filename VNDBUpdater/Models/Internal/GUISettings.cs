﻿using VNDBUpdater.Data;

namespace VNDBUpdater.Models.Internal
{
    public class GUISettings
    {
        public VisualNovelCatergory SelectedVNCategory { get; set; }
        public TagCategory SelectedTagCategory { get; set; }
        public SubTabs SelectedSubTab { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public GUISettings()
        {
            SelectedVNCategory = VisualNovelCatergory.Unknown;
            SelectedTagCategory = TagCategory.All;
            SelectedSubTab = SubTabs.Screenshots;
            Height = Constants.InitialHeight;
            Width = Constants.InitialWidth;
        }
    }
}
