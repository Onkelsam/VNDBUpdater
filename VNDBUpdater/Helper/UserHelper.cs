using VNDBUpdater.Communication.Database;
using VNDBUpdater.Models;

namespace VNDBUpdater.Helper
{
    public static class UserHelper
    {
        private static User _CurrentUser;

        public static User CurrentUser
        {
            private set { _CurrentUser = value; }
            get
            {
                if (_CurrentUser == null)
                {
                    _CurrentUser = new User();
                    _CurrentUser.GetUser();
                }

                return _CurrentUser;
            }
        }
    }
}
