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
            _User = await _UserService.Get();

            if (loginData != null && !string.IsNullOrEmpty(loginData.Username) && !string.IsNullOrEmpty(loginData.Password))
            {
                var newUser = new UserModel();

                newUser.Username = loginData.Username;
                newUser.SaveLogin = loginData.ShouldRemember;
                newUser.EncryptedPassword = ProtectedData.Protect(Encoding.UTF8.GetBytes(loginData.Password), null, DataProtectionScope.CurrentUser);

                if (await _UserService.Login(newUser))
                {
                    _User = newUser;
                    _StatusService.CurrentUser = _User.Username;

                    return true;
                }
                else
                {
                    await _UserService.Add(_User);

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
