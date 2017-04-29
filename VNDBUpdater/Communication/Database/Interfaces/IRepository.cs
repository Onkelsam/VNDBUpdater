using System.Collections.Generic;

namespace VNDBUpdater.Communication.Database.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T entity);
        IList<T> Get();
        T Get(int ID);
        void Delete(int ID);
    }
}
