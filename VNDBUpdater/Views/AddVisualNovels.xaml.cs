using System.Collections.Generic;
using System.Windows;
using VNDBUpdater.Models;

namespace VNDBUpdater.Views
{
    /// <summary>
    /// Interaction logic for AddVisualNovels.xaml
    /// </summary>
    public partial class AddVisualNovels : Window
    {
        public AddVisualNovels(List<VisualNovel> existingVisualNovels)
        {
            InitializeComponent();
        }
    }
}
