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

        public async Task AddAsync(FilterModel model)
        {
            await _RedisService.WriteAsync(_FilterKey + model.Name, new FilterEntity(model));
        }

        public async Task DeleteAsync(string name)
        {
            await _RedisService.DeleteKeyAsync(_FilterKey + name);
        }

        public async Task DeleteAsync(int ID)
        {
            await _RedisService.DeleteKeyAsync(_FilterKey + ID);
        }

        public async Task<IList<FilterModel>> GetAsync()
        {
            var filters = new List<FilterModel>();

            foreach (string key in await _RedisService.GetAllKeysAsync(_FilterKey + "*"))
            {
                filters.Add(await GetAsync(key.Replace(_FilterKey, "")));
            }

            return filters;
        }

        public async Task<FilterModel> GetAsync(string name)
        {
            var entity = await _RedisService.ReadAsync<FilterEntity>(_FilterKey + name);

            return new FilterModel(entity);
        }

        public async Task<FilterModel> GetAsync(int ID)
        {
            var entity = await _RedisService.ReadAsync<FilterEntity>(_FilterKey + ID);

            return new FilterModel(entity);
        }
    }
}
