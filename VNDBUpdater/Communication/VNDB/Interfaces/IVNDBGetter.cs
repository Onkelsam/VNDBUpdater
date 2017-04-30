using System.Collections.Generic;
using System.Threading.Tasks;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDBGetter
    {
        Task<VisualNovelModel> Get(int ID);
        Task<IList<VisualNovelModel>> Get(List<int> IDs);
        Task<IList<VisualNovelModel>> Get(string title);
        Task<IList<VN>> GetVNList();
        Task<IList<Vote>> GetVoteList();
    }
}
