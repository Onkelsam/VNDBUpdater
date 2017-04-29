using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;

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

        public void Add(VisualNovelEntity entity)
        {
            _RedisService.WriteEntity<VisualNovelEntity>(_VisualNovelKey + entity.Basics.ID, entity);

            var newList = new List<VisualNovelEntity>(_CacheService.GetList<VisualNovelEntity>("VisualNovels", () => GetAll()));

            if (newList.Any(x => x.Basics.ID == entity.Basics.ID))
            {
                newList.Remove(newList.First(x => x.Basics.ID == entity.Basics.ID));
            }
            
            newList.Add(entity);

            _CacheService.Set<IList<VisualNovelEntity>>("VisualNovels", newList);
        }

        public void Delete(int ID)
        {
            _RedisService.DeleteKey(_VisualNovelKey + ID);

            var newList = new List<VisualNovelEntity>(_CacheService.GetList<VisualNovelEntity>("VisualNovels", () => GetAll()));

            newList.Remove(newList.First(x => x.Basics.ID == ID));

            _CacheService.Set<IList<VisualNovelEntity>>("VisualNovels", newList);
        }

        public IList<VisualNovelEntity> Get()
        {
            return _CacheService.GetList<VisualNovelEntity>("VisualNovels", () => GetAll());
        }

        public VisualNovelEntity Get(int ID)
        {
            return _CacheService.GetList<VisualNovelEntity>("VisualNovels", () => GetAll()).First(x => x.Basics.ID == ID);
        }

        public bool VisualNovelExists(int ID)
        {
            return _CacheService.GetList<VisualNovelEntity>("VisualNovels", () => GetAll()).Any(x => x.Basics.ID == ID);
        }

        private IList<VisualNovelEntity> GetAll()
        {
            var visualNovels = new List<VisualNovelEntity>();

            foreach (string vn in _RedisService.GetAllKeys(_VisualNovelKey + "*"))
            {
                visualNovels.Add(_RedisService.ReadEntity<VisualNovelEntity>(vn));
            }

            return visualNovels;
        }
    }
}
