using MahApps.Metro.Controls.Dialogs;

namespace VNDBUpdater.Services.Login
{
    public interface ILoginService
    {
        bool LoginRequired { get; }

        bool Login(LoginDialogData loginData);
    }
}
