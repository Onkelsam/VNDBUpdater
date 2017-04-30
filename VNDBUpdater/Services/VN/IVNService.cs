using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.VN
{
    public interface IVNService : IServiceBase<VisualNovelModel>
    {
        Task<IList<VisualNovelModel>> GetLocal();
        Task<IList<VisualNovelModel>> Get(string title);
        Task<IList<VisualNovelModel>> Get(List<int> IDs);
        Task<VisualNovelModel> Get(int ID);
        Task<VisualNovelModel> GetLocal(int ID);

        Task<IList<Communication.VNDB.Entities.VN>> GetVNList();
        Task<IList<Communication.VNDB.Entities.Vote>> GetVoteList();
        Task SetVNList(VisualNovelModel model);

        Task<bool> VNExists(int ID);

        Task Add(VisualNovelModel model);
        Task Add(IList<VisualNovelModel> models);
        Task Delete(VisualNovelModel model);

        Task Update(VisualNovelModel model);

        void Start(VisualNovelModel model);
        void OpenFolder(VisualNovelModel model);
        void ViewOnVNDB(VisualNovelModel model);
        void ViewRelationOnVNDB(VisualNovelModel model, string relationTitle);
        void SearchOnGoggle(VisualNovelModel model, string searchParam);
        void OpenWalkthrough(VisualNovelModel model);
        Task CreateWalkthrough(VisualNovelModel model);
        Task SetCategory(VisualNovelModel model, VisualNovelModel.VisualNovelCatergory category);
        Task SetScore(VisualNovelModel model, int score);
        Task SetExePath(VisualNovelModel model, string path);
        bool InstallationPathExists(VisualNovelModel model);
        bool WalkthroughAvailable(VisualNovelModel model);
        Task DownloadImages(VisualNovelModel model);
        Task AddToPlayTime(VisualNovelModel model, TimeSpan timeToAdd);

        event EventHandler OnRefreshed;
    }
}
