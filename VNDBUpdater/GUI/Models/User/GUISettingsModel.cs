using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.GUI.ViewModels.MainView;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class GUISettingsModel
    {
        public GUISettingsModel()
        {
            SelectedAppTheme = "BaseLight";
            SelectedAppAccent = "Blue";
            Height = 800;
            Width = 1200;
            ImageDimension = 100;
            SelectedSortingMethod = VNDatagridViewModel.SortingMethod.Title;
            SelectedSortingDirection = VNDatagridViewModel.SortingDirection.Ascending;
        }

        public GUISettingsModel(GUISettingsEntity entity)
        {
            SelectedAppTheme = entity.SelectedAppTheme;
            SelectedAppAccent = entity.SelectedAppAccent;
            SelectedSortingMethod = entity.SelectedSortingMethod;
            SelectedSortingDirection = entity.SelectedSortingDirection;

            Width = entity.Width;
            Height = entity.Height;
            ImageDimension = entity.ImageDimension;
        }

        public VNDatagridViewModel.SortingMethod SelectedSortingMethod
        {
            get;
            set;
        }

        public VNDatagridViewModel.SortingDirection SelectedSortingDirection
        {
            get;
            set;
        }

        public string SelectedAppTheme
        {
            get;
            set;
        }

        public string SelectedAppAccent
        {
            get;
            set;
        }

        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }

        public int ImageDimension
        {
            get;
            set;
        }
    }
}
