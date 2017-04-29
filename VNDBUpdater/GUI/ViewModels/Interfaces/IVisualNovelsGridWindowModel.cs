using System.ComponentModel;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.GUI.ViewModels.Interfaces
{
    public interface IVisualNovelsGridWindowModel
    {
        VisualNovelModel SelectedVisualNovel { get; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}
