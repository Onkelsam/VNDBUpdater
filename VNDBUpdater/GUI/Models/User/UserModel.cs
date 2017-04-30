using VNDBUpdater.Communication.Database.Entities;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class UserModel
    {
        public UserModel()
        {
            Settings = new UserOptionsModel();
            GUI = new GUISettingsModel();
            FileIndexerSetting = new FileIndexerSettingsModel();
            Username = null;
            EncryptedPassword = null;
            SaveLogin = false;
        }

        public UserModel(UserEntity entity)
        {
            Settings = new UserOptionsModel(entity.Settings);
            GUI = new GUISettingsModel(entity.GUI);
            FileIndexerSetting = new FileIndexerSettingsModel(entity.FileIndexerSettings);
            Username = entity.Username;
            EncryptedPassword = entity.EncryptedPassword;
            SaveLogin = entity.SaveLogin;
        }

        public string Username
        {
            get;
            set;
        }

        public byte[] EncryptedPassword
        {
            get;
            set;
        }

        public bool SaveLogin
        {
            get;
            set;
        }

        public UserOptionsModel Settings
        {
            get;
            set;
        }

        public GUISettingsModel GUI
        {
            get;
            set;
        }

        public FileIndexerSettingsModel FileIndexerSetting
        {
            get;
            private set;
        }
    }
}
