using System;

namespace VNDBUpdater.Services
{
    public interface IServiceBase<T>
    {
        event EventHandler<T> OnAdded;
        event EventHandler<T> OnUpdated;
        event EventHandler<T> OnDeleted;
    }
}
