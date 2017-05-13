using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.Communication.VNDB.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.User
{
    public class UserService : IUserService
    {
        private IRedis _RedisConnection;
        private IVNDB _VNDBConnection;

        private IUserRepository _UserRepository;

        public event EventHandler<UserModel> OnUpdated = delegate { };
        public event EventHandler<UserModel> OnDeleted = delegate { };

        public UserService(IRedis RedisConnection, IUserRepository userRepository, IVNDB VNDBConnection)
        {
            _RedisConnection = RedisConnection;
            _UserRepository = userRepository;
            _VNDBConnection = VNDBConnection;
        }

        public async Task Update(UserModel model)
        {
            await _UserRepository.Add(model);

            OnUpdated?.Invoke(this, model);
        }

        public async Task<UserModel> Get()
        {
            return await _UserRepository.Get(0);
        }

        public async Task<bool> Login(UserModel model)
        {
            UserModel existingUser = await Get();

            if (string.IsNullOrEmpty(existingUser.Username))
            {
                return await CreateNewUser(model);
            }
            else if (existingUser.Username == model.Username && Unprotect(existingUser.EncryptedPassword, null, DataProtectionScope.CurrentUser) == Unprotect(model.EncryptedPassword, null, DataProtectionScope.CurrentUser))
            {
                return await LoginExistingUser(model);
            }
            else if (existingUser.Username == model.Username)
            {
                return await CreateNewUser(model);
            }
            else
            {
                return await ReplaceExistingUser(model);
            }            
        }

        private async Task<bool> CreateNewUser(UserModel model)
        {
            await Update(model);
            await _VNDBConnection.Reconnect();

            return _VNDBConnection.LoggedIn;
        }

        private async Task<bool> LoginExistingUser(UserModel model)
        {
            if (!_VNDBConnection.LoggedIn)
            {
                await _VNDBConnection.Reconnect();
            }

            await Update(model);

            return _VNDBConnection.LoggedIn;
        }

        private async Task<bool> ReplaceExistingUser(UserModel model)
        {
            await _RedisConnection.Reset();
            _RedisConnection.Reconnect();

            await Update(model);
            await _VNDBConnection.Reconnect();

            return _VNDBConnection.LoggedIn;
        }

        private string Unprotect(byte[] encryptedPassword, string entropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] clearBytes = ProtectedData.Unprotect(encryptedPassword, null, scope);

            return Encoding.UTF8.GetString(clearBytes);
        }
    }
}
