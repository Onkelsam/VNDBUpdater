using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        [Dependency]
        public IMainWindowModel ViewModel
        {
            set { DataContext = value; }
        }

        public MainWindow()
        {           
            InitializeComponent();
        }
    }
}
