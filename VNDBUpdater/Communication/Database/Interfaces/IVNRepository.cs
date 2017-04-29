using VNDBUpdater.Communication.Database.Entities;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IVNRepository : IRepository<VisualNovelEntity>
    {
        bool VisualNovelExists(int ID);
    }
}
