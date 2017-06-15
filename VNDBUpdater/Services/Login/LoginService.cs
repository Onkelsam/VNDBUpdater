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

        public async Task<bool> GetIsLoggedInAsync()
        {
            _User = await _UserService.GetAsync();

            bool loginRequired = true;

            if (!await _UserService.LoginAsync(_User))
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

        public async Task<bool> LoginAsync(LoginDialogData loginData)
        {
            _User = await _UserService.GetAsync();

            if (loginData != null && !string.IsNullOrEmpty(loginData.Username) && !string.IsNullOrEmpty(loginData.Password))
            {
                var newUser = new UserModel();

                newUser.Username = loginData.Username;
                newUser.SaveLogin = loginData.ShouldRemember;
                newUser.EncryptedPassword = ProtectedData.Protect(Encoding.UTF8.GetBytes(loginData.Password), null, DataProtectionScope.CurrentUser);

                if (await _UserService.LoginAsync(newUser))
                {
                    _User = newUser;
                    _StatusService.CurrentUser = _User.Username;

                    await _UserService.UpdateAsync(_User);

                    return true;
                }
                else
                {
                    await _UserService.UpdateAsync(_User);

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
