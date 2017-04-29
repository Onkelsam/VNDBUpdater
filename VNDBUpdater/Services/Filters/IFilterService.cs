using System;
using System.Collections.Generic;
using VNDBUpdater.GUI.Models.VisualNovel;
using static VNDBUpdater.GUI.Models.VisualNovel.FilterModel;

namespace VNDBUpdater.Services.Filters
{
    public interface IFilterService : IServiceBase<FilterModel>
    {
        IList<FilterModel> Get();
        FilterModel Get(string name);
        void Add(FilterModel model);
        void Delete(FilterModel model);

        void AddTagToFilter(FilterModel model, BooleanOperations operation, TagModel tag);
        void RemoveTagFromFilter(FilterModel model, string tagname);
        bool VNShouldBeFilteredOut(FilterModel model, VisualNovelModel vn);

        void ApplyFilter(FilterModel model);
        void ResetFilters();

        void SubscribeToFilterApply(Action<FilterModel> onFilterApply);
        void SubscribeToFilterReset(Action onFilterReset);
    }
}
