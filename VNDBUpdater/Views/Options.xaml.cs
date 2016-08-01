using System.Windows;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.Views
{
    /// <summary>
    /// Interaction logic for OptionsMenu.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options(MainViewModel ViewModel)
        {
            InitializeComponent();

            DataContext = new OptionsViewModel(ViewModel);            
        }
    }
}
