using MahApps.Metro.Controls.Dialogs;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.Services.Login
{
    public class LoginService : ILoginService
    {
        private UserModel _User;

        private IUserService _UserService;
        private IStatusService _StatusService;

        public LoginService(IUserService UserService, IStatusService StatusService)
        {
            _UserService = UserService;
            _StatusService = StatusService;
        }

        public async Task<bool> CheckLoginStatus()
        {
            _User = await _UserService.Get();

            bool loginRequired = true;

            if (!await _UserService.Login(_User))
            {
                loginRequired = true;
            }
            else
            {
                loginRequired = _User.Username == null || !_User.SaveLogin;
            }

            if (!loginRequired)
            {
                _StatusService.CurrentUser = _User.Username;
            }

            return loginRequired;
        }

        public async Task<bool> Login(LoginDialogData loginData)
        {
            if (loginData != null && !string.IsNullOrEmpty(loginData.Username) && !string.IsNullOrEmpty(loginData.Password))
            {                
                _User.Username = loginData.Username;
                _User.SaveLogin = loginData.ShouldRemember;
                _User.EncryptedPassword = ProtectedData.Protect(Encoding.UTF8.GetBytes(loginData.Password), null, DataProtectionScope.CurrentUser);

                if (await _UserService.Login(_User))
                {
                    _StatusService.CurrentUser = _User.Username;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
