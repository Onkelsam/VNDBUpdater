using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Views
{
    /// <summary>
    /// Interaction logic for AddVisualNovels.xaml
    /// </summary>
    public partial class AddVisualNovels : MetroWindow
    {
        [Dependency]
        public IAddVisualNovelsWindowModel ViewModel
        {
            set { DataContext = value; }
        }
        public AddVisualNovels()
        {
            InitializeComponent();
        }
    }
}
