using System;
using System.Collections.Generic;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;

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

        public void Add(UserEntity entity)
        {
            _RedisService.WriteEntity<UserEntity>(_UserKey, entity);
        }

        public void Delete(int ID)
        {
            throw new NotImplementedException();
        }

        public IList<UserEntity> Get()
        {
            throw new NotImplementedException();
        }

        public UserEntity Get(int ID)
        {
            UserEntity user = _RedisService.ReadEntity<UserEntity>(_UserKey);

            return user ?? new UserEntity();
        }
    }
}
