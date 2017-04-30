using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDBSetter
    {
        Task AddToVNList(VisualNovelModel model);
        Task AddToScoreList(VisualNovelModel model);
        Task RemoveFromVNList(VisualNovelModel model);
        Task RemoveFromScoreList(VisualNovelModel model);
    }
}
