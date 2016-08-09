using System.Windows;
using VNDBUpdater.BackgroundTasks;
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

            ShowInTaskbar = false;
            
            var Startup = new StartUp();
            Startup.Start((DataContext as MainViewModel));    
        }
    }
}
