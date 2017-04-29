using MahApps.Metro.Controls.Dialogs;
using System.Security.Cryptography;
using System.Text;
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

            _User = _UserService.Get();

            if (!_UserService.Login(_User))
            {
                _LoginRequired = true;
            }
            else
            {
                _LoginRequired = _User.Username == null || !_User.SaveLogin;
            }                   

            if (!LoginRequired)
            {
                _StatusService.CurrentUser = _User.Username;
            }
        }

        private bool _LoginRequired;

        public bool LoginRequired
        {
            get { return _LoginRequired; }
            set { _LoginRequired = value; }
        }

        public bool Login(LoginDialogData loginData)
        {
            if (loginData != null && !string.IsNullOrEmpty(loginData.Username) && !string.IsNullOrEmpty(loginData.Password))
            {
                _User.Username = loginData.Username;
                _User.SaveLogin = loginData.ShouldRemember;
                _User.EncryptedPassword = ProtectedData.Protect(Encoding.UTF8.GetBytes(loginData.Password), null, DataProtectionScope.CurrentUser);

                if (_UserService.Login(_User))
                {
                    _LoginRequired = false;
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
