using System.Collections.Generic;
using System.Threading.Tasks;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IRepository<T>
    {
        Task Add(T entity);
        Task<IList<T>> Get();
        Task<T> Get(int ID);
        Task Delete(int ID);
    }
}
