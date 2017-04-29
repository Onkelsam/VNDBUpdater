using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : MetroWindow
    {
        [Dependency]
        public IAboutViewModel ViewModel
        {
            set { DataContext = value; }
        }
        public About()
        {
            InitializeComponent();
        }
    }
}
