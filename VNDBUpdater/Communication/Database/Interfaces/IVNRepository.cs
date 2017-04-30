using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IVNRepository : IRepository<VisualNovelModel>
    {
        Task<bool> VisualNovelExists(int ID);
    }
}
