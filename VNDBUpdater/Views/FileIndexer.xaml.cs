using System.Windows;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.Views
{
    /// <summary>
    /// Interaction logic for FileIndexer.xaml
    /// </summary>
    public partial class FileIndexer : Window
    {
        public FileIndexer(MainViewModel ViewModel)
        {
            InitializeComponent();

            DataContext = new FileIndexerViewModel(ViewModel);
        }
    }
}
