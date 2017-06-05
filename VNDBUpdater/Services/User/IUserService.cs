using System;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.User
{
    public interface IUserService
    {
        Task<UserModel> GetAsync();
        Task UpdateAsync(UserModel model);

        Task<bool> LoginAsync(UserModel model);

        event EventHandler<UserModel> OnUpdated;
        event EventHandler<UserModel> OnDeleted;
    }
}
