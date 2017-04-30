using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace VNDBUpdater.Services.Login
{
    public interface ILoginService
    {
        bool LoginRequired { get; }

        Task<bool> Login(LoginDialogData loginData);
    }
}
