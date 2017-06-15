using System.Collections.Generic;
using System.Threading.Tasks;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IRepository<T>
    {
        Task AddAsync(T entity);
        Task<IList<T>> GetAsync();
        Task<T> GetAsync(int ID);
        Task DeleteAsync(int ID);
    }
}
