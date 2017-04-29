using Microsoft.Practices.Unity;
using System.Windows.Controls;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Controls
{
    /// <summary>
    /// Interaction logic for ScreenshotTab.xaml
    /// </summary>
    public partial class ScreenshotTab : UserControl
    {
        [Dependency]
        public IScreenshotTabWindowModel ViewModel
        {
            set { DataContext = value; }
        }

        public ScreenshotTab()
        {
            InitializeComponent();
        }
    }
}
