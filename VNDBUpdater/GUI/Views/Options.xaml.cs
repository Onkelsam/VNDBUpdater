using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.GUI.ViewModels.MainView;

namespace VNDBUpdater.GUI.Views
{
    /// <summary>
    /// Interaction logic for OptionsMenu.xaml
    /// </summary>
    public partial class Options : MetroWindow
    {
        [Dependency]
        public IOptionsWindowModel ViewModel
        {
            set { DataContext = value; }
        }

        public Options()
        {
            InitializeComponent();         
        }
    }
}
