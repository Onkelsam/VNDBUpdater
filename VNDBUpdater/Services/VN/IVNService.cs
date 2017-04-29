using System;
using System.Collections.Generic;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.VN
{
    public interface IVNService : IServiceBase<VisualNovelModel>
    {
        IList<VisualNovelModel> GetLocal();
        IList<VisualNovelModel> Get(string title);
        IList<VisualNovelModel> Get(List<int> IDs);
        VisualNovelModel Get(int ID);
        VisualNovelModel GetLocal(int ID);

        IList<Communication.VNDB.Entities.VN> GetVNList();
        IList<Communication.VNDB.Entities.Vote> GetVoteList();
        void SetVNList(VisualNovelModel model);

        bool VNExists(int ID);

        void Add(VisualNovelModel model);
        void Add(IList<VisualNovelModel> models);
        void Delete(VisualNovelModel model);

        void Update(VisualNovelModel model);

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


        void SubscribeToRefreshAll(Action onRefreshed);
    }
}
