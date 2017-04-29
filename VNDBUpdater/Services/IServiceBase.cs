using System;

namespace VNDBUpdater.Services
{
    public interface IServiceBase<T>
    {
        void SubscribeToTAdded(Action<T> onTAdded);
        void SubscribeToTUpdated(Action<T> onTUpdated);
        void SubscribeToTDeleted(Action<T> onTDeleted);
    }
}
