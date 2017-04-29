using VNDBUpdater.Communication.Database.Entities;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IFilterRepository : IRepository<FilterEntity>
    {
        void Delete(string name);
        FilterEntity Get(string name);
    }
}
