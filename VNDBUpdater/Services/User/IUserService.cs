using System;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.User
{
    public interface IUserService
    {
        Task<UserModel> Get();
        Task Update(UserModel model);

        Task<bool> Login(UserModel model);

        event EventHandler<UserModel> OnUpdated;
        event EventHandler<UserModel> OnDeleted;
    }
}
