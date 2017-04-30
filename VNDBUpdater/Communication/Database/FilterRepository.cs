using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;

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

        public async Task Add(FilterModel model)
        {
            await _RedisService.WriteEntity(_FilterKey + model.Name, new FilterEntity(model));
        }

        public async Task Delete(string name)
        {
            await _RedisService.DeleteKey(_FilterKey + name);
        }

        public async Task Delete(int ID)
        {
        }

        public async Task<IList<FilterModel>> Get()
        {
            var filters = new List<FilterModel>();

            foreach (string key in await _RedisService.GetAllKeys(_FilterKey + "*"))
            {
                filters.Add(await Get(key.Replace(_FilterKey, "")));
            }

            return filters;
        }

        public async Task<FilterModel> Get(string name)
        {
            var entity = await _RedisService.ReadEntity<FilterEntity>(_FilterKey + name);

            return new FilterModel(entity);
        }

        public async Task<FilterModel> Get(int ID)
        {
            throw new NotImplementedException();
        }
    }
}
