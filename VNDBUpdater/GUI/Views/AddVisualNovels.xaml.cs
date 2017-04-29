using Microsoft.Practices.Unity;
using System.Windows;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Views
{
    /// <summary>
    /// Interaction logic for AddVisualNovels.xaml
    /// </summary>
    public partial class AddVisualNovels : Window
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
