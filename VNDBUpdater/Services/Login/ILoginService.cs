using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace VNDBUpdater.Services.Login
{
    public interface ILoginService
    {
        Task<bool> CheckLoginStatus();

        Task<bool> Login(LoginDialogData loginData);
    }
}
