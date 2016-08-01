using System.IO;
using System.Windows;
using VNDBUpdater.BackgroundTasks;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            (DataContext as MainViewModel).GetVisualNovelsFromDatabase();

            if (!File.Exists(@"tags.json"))
                Models.Tag.RefreshTags();

            if (RedisCommunication.UserCredentialsAvailable())
            {
                var BackgroundSynchronizer = new Synchronizer();
                BackgroundSynchronizer.Start((DataContext as MainViewModel));
            }            
        }
    }
}
