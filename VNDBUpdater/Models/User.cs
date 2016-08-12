using VNDBUpdater.Communication.Database;
using VNDBUpdater.Models.Internal;

namespace VNDBUpdater.Models
{
    public class User
    {
        public string Username { get; set; }
        public byte[] EncryptedPassword { get; set; }

        public UserOptions Settings { get; set; }
        public GUISettings GUI { get; set; }

        public User()
        {
        }

        public void SaveUser()
        {
            RedisCommunication.SetUser(this);
        }

        public void GetUser()
        {
            var existingUser = RedisCommunication.GetUser();

            if (existingUser != null)
            {
                Settings = existingUser.Settings ?? new UserOptions();
                GUI = existingUser.GUI ?? new GUISettings();

                Username = existingUser.Username;
                EncryptedPassword = existingUser.EncryptedPassword;
            }
            else
            {
                Settings = new UserOptions();
                GUI = new GUISettings();
                Username = null;
                EncryptedPassword = null;
            }
        }
    }
}
