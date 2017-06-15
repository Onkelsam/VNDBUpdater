using System.Collections.Generic;
using System.Threading.Tasks;
using VNDBUpdater.Data;

namespace VNDBUpdater.Services
{
    public interface ITagsAndTraits<T>
    {
        IList<T> Get();
        bool Show(SpoilerSetting UserSetting, SpoilerLevel TSpoiler);
        Task RefreshAsync();
    }
}
