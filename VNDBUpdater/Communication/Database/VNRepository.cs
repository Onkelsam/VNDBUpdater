using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database
{
    class VNRepository : IVNRepository
    {
        private IRedis _RedisService;
        private ICache _CacheService;

        private const string _VisualNovelKey = "VisualNovel_";

        public VNRepository(IRedis redisService, ICache cacheService)
        {
            _RedisService = redisService;
            _CacheService = cacheService;
        }

        public async Task Add(VisualNovelModel model)
        {
            await _RedisService.WriteEntity(_VisualNovelKey + model.Basics.ID, new VisualNovelEntity(model));

            var newList = new List<VisualNovelModel>(await _CacheService.GetList("VisualNovels", async () => await GetAll()));

            if (newList.Any(x => x.Basics.ID == model.Basics.ID))
            {
                newList.Remove(newList.First(x => x.Basics.ID == model.Basics.ID));
            }
            
            newList.Add(model);

            await _CacheService.Set<IList<VisualNovelModel>>("VisualNovels", newList);
        }

        public async Task Delete(int ID)
        {
            await _RedisService.DeleteKey(_VisualNovelKey + ID);

            var newList = new List<VisualNovelModel>(await _CacheService.GetList("VisualNovels", async () => await GetAll()));

            newList.Remove(newList.First(x => x.Basics.ID == ID));

            await _CacheService.Set<IList<VisualNovelModel>>("VisualNovels", newList);
        }

        public async Task<IList<VisualNovelModel>> Get()
        {
            return await _CacheService.GetList("VisualNovels", async () => await GetAll());
        }

        public async Task<VisualNovelModel> Get(int ID)
        {
            var cache = await _CacheService.GetList("VisualNovels", async () => await GetAll());

            return cache.First(x => x.Basics.ID == ID);
        }

        public async Task<bool> VisualNovelExists(int ID)
        {
            var cache = await _CacheService.GetList("VisualNovels", async () => await GetAll());

            return cache.Any(x => x.Basics.ID == ID);
        }

        private async Task<IList<VisualNovelModel>> GetAll()
        {
            var visualNovels = new List<VisualNovelEntity>();

            foreach (string vn in await _RedisService.GetAllKeys(_VisualNovelKey + "*"))
            {
                visualNovels.Add(await _RedisService.ReadEntity<VisualNovelEntity>(vn));
            }

            return visualNovels.Select(x => new VisualNovelModel(x)).ToList();
        }
    }
}
