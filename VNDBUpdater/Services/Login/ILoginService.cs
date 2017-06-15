using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace VNDBUpdater.Services.Login
{
    public interface ILoginService
    {
        Task<bool> GetIsLoggedInAsync();

        Task<bool> LoginAsync(LoginDialogData loginData);
    }
}
