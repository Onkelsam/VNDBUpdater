using Microsoft.Practices.Unity;
using System.Windows.Controls;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.Controls
{
    /// <summary>
    /// Interaction logic for CharacterTab.xaml
    /// </summary>
    public partial class CharacterTab : UserControl
    {
        [Dependency]
        public ICharacterTabWindowModel ViewModel
        {
            set { DataContext = value; }
        }

        public CharacterTab()
        {
            InitializeComponent();
        }
    }
}
