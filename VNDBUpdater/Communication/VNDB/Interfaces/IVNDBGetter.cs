using System.Collections.Generic;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDBGetter
    {
        VisualNovelModel Get(int ID);
        IList<VisualNovelModel> Get(List<int> IDs);
        IList<VisualNovelModel> Get(string title);
        IList<VN> GetVNList();
        IList<Vote> GetVoteList();
    }
}
