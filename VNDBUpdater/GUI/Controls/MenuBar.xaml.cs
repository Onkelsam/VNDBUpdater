using Microsoft.Practices.Unity;
using System.Windows.Controls;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Controls
{
    /// <summary>
    /// Interaction logic for MenuBar.xaml
    /// </summary>
    public partial class MenuBar : UserControl
    {
        [Dependency]
        public IMenuBarWindowModel ViewModel
        {
            set { DataContext = value; }
        }

        public MenuBar()
        {
            InitializeComponent();
        }
    }
}
