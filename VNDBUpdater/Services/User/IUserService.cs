using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.User
{
    public interface IUserService : IServiceBase<UserModel>
    {
        Task<UserModel> Get();
        Task Add(UserModel model);
        Task Update(UserModel model);

        Task<bool> Login(UserModel model);
    }
}
