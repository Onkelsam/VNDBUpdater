using System.Collections.Generic;
using VNDBUpdater.Data;

namespace VNDBUpdater.Services
{
    public interface ITagsAndTraits<T>
    {
        IList<T> Get();
        bool Show(SpoilerSetting UserSetting, SpoilerLevel TSpoiler);
        void Refresh();
    }
}
