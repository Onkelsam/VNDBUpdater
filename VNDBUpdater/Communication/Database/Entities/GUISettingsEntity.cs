using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.MainView;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class GUISettingsEntity
    {
        public GUISettingsEntity()
        {
            Width = 1200;
            Height = 800;
            ImageDimension = 100;
            SelectedAppTheme = "BaseLight";
            SelectedAppAccent = "Blue";
            SelectedSortingMethod = VNDatagridViewModel.SortingMethod.Title;
            SelectedSortingDirection = VNDatagridViewModel.SortingDirection.Ascending;
        }

        public GUISettingsEntity(GUISettingsModel model)
        {
            SelectedAppTheme = model.SelectedAppTheme;
            SelectedAppAccent = model.SelectedAppAccent;
            Width = model.Width;
            Height = model.Height;
            ImageDimension = model.ImageDimension;
            SelectedSortingMethod = model.SelectedSortingMethod;
            SelectedSortingDirection = model.SelectedSortingDirection;
        }
        public int ImageDimension { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string SelectedAppTheme { get; set; }
        public string SelectedAppAccent { get; set; }
        public VNDatagridViewModel.SortingMethod SelectedSortingMethod { get; set; }
        public VNDatagridViewModel.SortingDirection SelectedSortingDirection { get; set; }
    }
}
