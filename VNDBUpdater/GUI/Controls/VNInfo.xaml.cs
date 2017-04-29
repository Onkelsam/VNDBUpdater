using Microsoft.Practices.Unity;
using System.Windows.Controls;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Controls
{
    /// <summary>
    /// Interaction logic for VNInfo.xaml
    /// </summary>
    public partial class VNInfo : UserControl
    {
        [Dependency]
        public IVisualNovelInfoWindowModel ViewModel
        {
            set { DataContext = value; }
        }

        public VNInfo()
        {
            InitializeComponent();
        }
    }
}
