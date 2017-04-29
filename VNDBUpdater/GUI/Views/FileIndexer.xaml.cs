using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Views
{
    /// <summary>
    /// Interaction logic for FileIndexer.xaml
    /// </summary>
    public partial class FileIndexer : MetroWindow
    {
        [Dependency]
        public IFileIndexerWindowModel ViewModel
        {
            set { DataContext = value; }
        }

        public FileIndexer()
        {
            InitializeComponent();
        }
    }
}
