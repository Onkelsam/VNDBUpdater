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

        public event EventHandler<UserModel> OnAdded = delegate { };
        public event EventHandler<UserModel> OnUpdated = delegate { };
        public event EventHandler<UserModel> OnDeleted = delegate { };

        public UserService(IRedis RedisConnection, IUserRepository userRepository, IVNDB VNDBConnection)
        {
            _RedisConnection = RedisConnection;
            _UserRepository = userRepository;
            _VNDBConnection = VNDBConnection;
        }

        public async Task Add(UserModel model)
        {
            await _UserRepository.Add(model);

            OnAdded?.Invoke(this, model);
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
                await Add(model);

                await _VNDBConnection.Reconnect();

                OnAdded?.Invoke(this, model);

                return _VNDBConnection.LoggedIn;
            }
            else if (existingUser.Username == model.Username && Unprotect(existingUser.EncryptedPassword, null, DataProtectionScope.CurrentUser) == Unprotect(model.EncryptedPassword, null, DataProtectionScope.CurrentUser))
            {
                if (!_VNDBConnection.LoggedIn)
                {
                    await _VNDBConnection.Reconnect();
                }

                await Add(model);

                return _VNDBConnection.LoggedIn;
            }
            else
            {
                _RedisConnection.Reconnect();

                await Add(model);

                await _VNDBConnection.Reconnect();

                OnAdded?.Invoke(this, model);

                return _VNDBConnection.LoggedIn;
            }            
        }

        private string Unprotect(byte[] encryptedPassword, string entropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] clearBytes = ProtectedData.Unprotect(encryptedPassword, null, scope);

            return Encoding.UTF8.GetString(clearBytes);
        }
    }
}
