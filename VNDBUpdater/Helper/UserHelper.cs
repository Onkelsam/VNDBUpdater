using VNDBUpdater.Models;

namespace VNDBUpdater.Helper
{
    public static class UserHelper
    {
        private static User _CurrentUser;

        private static object Lock = new object();

        public static User CurrentUser
        {
            get
            {
                lock (Lock)
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
}
