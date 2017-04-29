using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDBSetter
    {
        void AddToVNList(VisualNovelModel model);
        void AddToScoreList(VisualNovelModel model);
        void RemoveFromVNList(VisualNovelModel model);
        void RemoveFromScoreList(VisualNovelModel model);
    }
}
