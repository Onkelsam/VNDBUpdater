using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Views
{
    /// <summary>
    /// Interaction logic for AddAndEditFilter.xaml
    /// </summary>
    public partial class CreateFilter : MetroWindow
    {
        [Dependency]
        public ICreateFilterWindowModel ViewModel
        {
            set { DataContext = value; }
        }
        public CreateFilter()
        {
            InitializeComponent();            
        }
    }
}
