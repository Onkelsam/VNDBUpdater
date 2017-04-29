using System;
using System.Collections.Generic;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;

namespace VNDBUpdater.Communication.Database
{
    class FilterRepository : IFilterRepository
    {
        private IRedis _RedisService;

        private const string _FilterKey = "_Filter";
        private const string _UserKey = "User";

        public FilterRepository(IRedis redisService)
        {
            _RedisService = redisService;
        }

        public void Add(FilterEntity entity)
        {
            _RedisService.WriteEntity<FilterEntity>(_FilterKey + entity.Name, entity);
        }

        public void Delete(string name)
        {
            _RedisService.DeleteKey(_FilterKey + name);
        }

        public void Delete(int ID)
        {
        }

        public IList<FilterEntity> Get()
        {
            var filters = new List<FilterEntity>();

            foreach (string key in _RedisService.GetAllKeys(_FilterKey + "*"))
            {
                filters.Add(Get(key.Replace(_FilterKey, "")));
            }

            return filters;
        }

        public FilterEntity Get(string name)
        {
            return _RedisService.ReadEntity<FilterEntity>(_FilterKey + name);
        }

        public FilterEntity Get(int ID)
        {
            throw new NotImplementedException();
        }
    }
}
