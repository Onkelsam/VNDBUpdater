using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.VN
{
    public interface IVNService : IServiceBase<VisualNovelModel>
    {
        IList<VisualNovelModel> GetLocal();
        Task<IList<VisualNovelModel>> Get(string title);
        Task<IList<VisualNovelModel>> Get(List<int> IDs);
        Task<VisualNovelModel> Get(int ID);
        VisualNovelModel GetLocal(int ID);

        Task<IList<Communication.VNDB.Entities.VN>> GetVNList();
        Task<IList<Communication.VNDB.Entities.Vote>> GetVoteList();
        void SetVNList(VisualNovelModel model);

        bool VNExists(int ID);

        void Add(VisualNovelModel model);
        void Add(IList<VisualNovelModel> models);
        void Delete(VisualNovelModel model);

        Task Update(VisualNovelModel model);

        void Start(VisualNovelModel model);
        void OpenFolder(VisualNovelModel model);
        void ViewOnVNDB(VisualNovelModel model);
        void ViewRelationOnVNDB(VisualNovelModel model, string relationTitle);
        void SearchOnGoggle(VisualNovelModel model, string searchParam);
        void OpenWalkthrough(VisualNovelModel model);
        void CreateWalkthrough(VisualNovelModel model);
        void SetCategory(VisualNovelModel model, VisualNovelModel.VisualNovelCatergory category);
        void SetScore(VisualNovelModel model, int score);
        void SetExePath(VisualNovelModel model, string path);
        bool InstallationPathExists(VisualNovelModel model);
        bool WalkthroughAvailable(VisualNovelModel model);
        void DownloadImages(VisualNovelModel model);
        void AddToPlayTime(VisualNovelModel model, TimeSpan timeToAdd);


        void SubscribeToRefreshAll(Action onRefreshed);
    }
}
