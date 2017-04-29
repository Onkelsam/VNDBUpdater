using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class UserEntity
    {
        public string Username { get; set; }
        public byte[] EncryptedPassword { get; set; }
        public bool SaveLogin { get; set; }
        public UserOptionsEntity Settings { get; set; }
        public GUISettingsEntity GUI { get; set; }
        public FileIndexerSettingsEntity FileIndexerSettings { get; set; }

        public UserEntity()
        {
            Settings = new UserOptionsEntity();
            GUI = new GUISettingsEntity();
            FileIndexerSettings = new FileIndexerSettingsEntity();
        }

        public UserEntity(UserModel model)
        {
            Username = model.Username;
            EncryptedPassword = model.EncryptedPassword;
            SaveLogin = model.SaveLogin;

            Settings = new UserOptionsEntity(model.Settings);
            GUI = new GUISettingsEntity(model.GUI);
            FileIndexerSettings = new FileIndexerSettingsEntity(model.FileIndexerSetting);
        }
    }
}
