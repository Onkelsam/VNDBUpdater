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

        public async Task Add(UserModel model)
        {
            await _RedisService.WriteEntity(_UserKey, new UserEntity(model));
        }

        public async Task Delete(int ID)
        {
            await _RedisService.DeleteKey(_UserKey + ID);
        }

        public async Task<IList<UserModel>> Get()
        {
            var users = new List<UserEntity>();

            foreach (var user in await _RedisService.GetAllKeys(_UserKey))
            {
                users.Add(await _RedisService.ReadEntity<UserEntity>(user));
            }

            return users.Select(x => new UserModel(x)).ToList();
        }

        public async Task<UserModel> Get(int ID)
        {
            var entity = await _RedisService.ReadEntity<UserEntity>(_UserKey);

            return entity == null 
                ? new UserModel() 
                : new UserModel(entity);
        }
    }
}
