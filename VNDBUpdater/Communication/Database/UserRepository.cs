using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database
{
    class UserRepository : IUserRepository
    {
        private IRedis _RedisService;

        private const string _UserKey = "User";

        public UserRepository(IRedis redisService)
        {
            _RedisService = redisService;
        }

        public async Task AddAsync(UserModel model)
        {
            await _RedisService.WriteAsync(_UserKey, new UserEntity(model));
        }

        public async Task DeleteAsync(int ID)
        {
            await _RedisService.DeleteKeyAsync(_UserKey + ID);
        }

        public async Task<IList<UserModel>> GetAsync()
        {
            var users = new List<UserEntity>();

            foreach (var user in await _RedisService.GetAllKeysAsync(_UserKey))
            {
                users.Add(await _RedisService.ReadAsync<UserEntity>(user));
            }

            return users.Select(x => new UserModel(x)).ToList();
        }

        public async Task<UserModel> GetAsync(int ID)
        {
            var entity = await _RedisService.ReadAsync<UserEntity>(_UserKey);

            return entity == null 
                ? new UserModel() 
                : new UserModel(entity);
        }
    }
}
