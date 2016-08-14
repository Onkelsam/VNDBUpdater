using System.Windows;
using VNDBUpdater.BackgroundTasks;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
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
            FileHelper.DeleteTooLargeFile(Constants.EventlogFileName, 1000000);

            try
            {
                FileHelper.DeleteFile(Constants.DirectoryPath + @"\Resources\redis-server.pdb");
            }
            catch {; }

            InitializeComponent();

            DataContext = new MainViewModel();

            var Startup = new StartUp();
            Startup.Start((DataContext as MainViewModel));    
        }
    }
}
