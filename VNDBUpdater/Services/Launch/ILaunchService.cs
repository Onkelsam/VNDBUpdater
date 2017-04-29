using System;

namespace VNDBUpdater.Services.Launch
{
    public interface ILaunchService
    {
        void Launch(Action<bool> onLaunchFinished);
    }
}
