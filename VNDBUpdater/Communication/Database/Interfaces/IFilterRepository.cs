using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IFilterRepository : IRepository<FilterModel>
    {
        Task DeleteAsync(string name);
        Task<FilterModel> GetAsync(string name);
    }
}
