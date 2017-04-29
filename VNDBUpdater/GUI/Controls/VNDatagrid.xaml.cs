using Microsoft.Practices.Unity;
using System.Windows.Controls;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Controls
{
    /// <summary>
    /// Interaction logic for VNDatagrid.xaml
    /// </summary>
    public partial class VNDatagrid : UserControl
    {
        [Dependency]
        public IVisualNovelsGridWindowModel ViewModel
        {
            set { DataContext = value; }
        }

        public VNDatagrid()
        {
            InitializeComponent();
        }
    }
}
