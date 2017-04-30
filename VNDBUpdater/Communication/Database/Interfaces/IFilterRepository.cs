using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IFilterRepository : IRepository<FilterModel>
    {
        Task Delete(string name);
        Task<FilterModel> Get(string name);
    }
}
