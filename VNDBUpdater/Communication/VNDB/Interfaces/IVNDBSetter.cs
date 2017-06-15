using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDBSetter
    {
        Task AddToVNListAsync(VisualNovelModel model);
        Task AddToScoreListAsync(VisualNovelModel model);
        Task RemoveFromVNListAsync(VisualNovelModel model);
        Task RemoveFromScoreListAsync(VisualNovelModel model);
    }
}
