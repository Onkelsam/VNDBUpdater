using Microsoft.Practices.Unity;
using System.Windows.Controls;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Controls
{
    /// <summary>
    /// Interaction logic for TagTab.xaml
    /// </summary>
    public partial class TagTab : UserControl
    {
        [Dependency]
        public ITagTabWIndowModel ViewModel
        {
            set { DataContext = value; }
        }

        public TagTab()
        {
            InitializeComponent();
        }
    }
}
