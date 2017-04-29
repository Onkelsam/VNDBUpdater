using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Views
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : MetroWindow
    {
        [Dependency]
        public ISplashScreenWindowModel ViewModel
        {
            set
            {
                DataContext = value;
                value.SplashScreen = this;
            }
        }

        public SplashScreen()
        {
            InitializeComponent();
        }
    }
}
