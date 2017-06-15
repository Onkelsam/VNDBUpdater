using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.VN
{
    public interface IVNService : IServiceBase<VisualNovelModel>
    {
        Task<IList<VisualNovelModel>> GetLocalAsync();
        Task<IList<VisualNovelModel>> GetAsync(string title);
        Task<IList<VisualNovelModel>> GetAsync(List<int> IDs);
        Task<VisualNovelModel> GetAsync(int ID);
        Task<VisualNovelModel> GetLocalAsync(int ID);

        Task<IList<Communication.VNDB.Entities.VN>> GetVNListAsync();
        Task<IList<Communication.VNDB.Entities.Vote>> GetVoteListAsync();
        Task SetVNListAsync(VisualNovelModel model);

        Task<bool> CheckIfVNExistsAsync(int ID);

        Task AddAsync(VisualNovelModel model);
        Task AddAsync(IList<VisualNovelModel> models);
        Task DeleteAsync(VisualNovelModel model);
        Task DeleteLocalAsync(VisualNovelModel model);

        Task UpdateAsync(VisualNovelModel model);

        void Start(VisualNovelModel model);
        void OpenFolder(VisualNovelModel model);
        void ViewOnVNDB(VisualNovelModel model);
        void ViewRelationOnVNDB(VisualNovelModel model, string relationTitle);
        void SearchOnGoggle(VisualNovelModel model, string searchParam);
        void OpenWalkthrough(VisualNovelModel model);
        Task CreateWalkthroughAsync(VisualNovelModel model);
        Task SetCategoryAsync(VisualNovelModel model, VisualNovelModel.VisualNovelCatergory category);
        Task SetScoreAsync(VisualNovelModel model, int score);
        Task SetExePathAsync(VisualNovelModel model, string path);
        bool CheckIfInstallationPathExists(VisualNovelModel model);
        bool CheckIfWalkthroughExists(VisualNovelModel model);
        Task DownloadImagesAsync(VisualNovelModel model);
        Task AddToPlayTimeAsync(VisualNovelModel model, TimeSpan timeToAdd);

        event EventHandler OnRefreshed;
    }
}
