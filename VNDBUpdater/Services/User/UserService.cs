using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using VNDBUpdater.Communication.Database.Entities;
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

        private event Action<UserModel> _OnUserAdded = delegate { };
        private event Action<UserModel> _OnUserUpdated = delegate { };
        private event Action<UserModel> _OnUserDeleted = delegate { };

        public UserService(IRedis RedisConnection, IUserRepository userRepository, IVNDB VNDBConnection)
        {
            _RedisConnection = RedisConnection;
            _UserRepository = userRepository;
            _VNDBConnection = VNDBConnection;
        }

        public void Add(UserModel model)
        {
            _UserRepository.Add(new UserEntity(model));

            _OnUserAdded?.Invoke(model);
        }

        public void Update(UserModel model)
        {
            _UserRepository.Add(new UserEntity(model));

            _OnUserUpdated?.Invoke(model);
        }

        public UserModel Get()
        {
            return new UserModel(_UserRepository.Get(0));
        }

        public bool Login(UserModel model)
        {
            UserModel existingUser = Get();

            if (string.IsNullOrEmpty(existingUser.Username))
            {
                Add(model);

                _VNDBConnection.Reconnect();

                _OnUserAdded?.Invoke(model);

                return _VNDBConnection.LoggedIn;
            }
            else if (existingUser.Username == model.Username && Unprotect(existingUser.EncryptedPassword, null, DataProtectionScope.CurrentUser) == Unprotect(model.EncryptedPassword, null, DataProtectionScope.CurrentUser))
            {
                if (!_VNDBConnection.LoggedIn)
                {
                    _VNDBConnection.Reconnect();
                }

                Add(model);

                return _VNDBConnection.LoggedIn;
            }
            else
            {
                _RedisConnection.Reconnect();

                Add(model);

                _VNDBConnection.Reconnect();

                _OnUserAdded?.Invoke(model);

                return _VNDBConnection.LoggedIn;
            }            
        }

        public void SubscribeToTAdded(Action<UserModel> onTAdded)
        {
            if (!_OnUserAdded.GetInvocationList().Contains(onTAdded))
            {
                _OnUserAdded += onTAdded;
            }
        }

        public void SubscribeToTDeleted(Action<UserModel> onTDeleted)
        {
            if (!_OnUserDeleted.GetInvocationList().Contains(onTDeleted))
            {
                _OnUserDeleted += onTDeleted;
            }
        }

        public void SubscribeToTUpdated(Action<UserModel> onTUpdated)
        {
            if (!_OnUserUpdated.GetInvocationList().Contains(onTUpdated))
            {
                _OnUserUpdated += onTUpdated;
            }
        }

        private string Unprotect(byte[] encryptedPassword, string entropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] clearBytes = ProtectedData.Unprotect(encryptedPassword, null, scope);

            return Encoding.UTF8.GetString(clearBytes);
        }
    }
}
