using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.User
{
    public interface IUserService : IServiceBase<UserModel>
    {
        UserModel Get();
        void Add(UserModel model);
        void Update(UserModel model);

        Task<bool> Login(UserModel model);
    }
}
