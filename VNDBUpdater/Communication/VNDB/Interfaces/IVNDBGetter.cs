using System.Collections.Generic;
using System.Threading.Tasks;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDBGetter
    {
        Task<VisualNovelModel> GetAsync(int ID);
        Task<IList<VisualNovelModel>> GetAsync(List<int> IDs);
        Task<IList<VisualNovelModel>> GetAsync(string title);
        Task<IList<VN>> GetVNListAsync();
        Task<IList<Vote>> GetVoteListAsync();
    }
}
