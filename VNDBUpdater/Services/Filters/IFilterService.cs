﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;
using static VNDBUpdater.GUI.Models.VisualNovel.FilterModel;

namespace VNDBUpdater.Services.Filters
{
    public interface IFilterService : IServiceBase<FilterModel>
    {
        Task<IList<FilterModel>> GetAsync();
        Task<FilterModel> GetAsync(string name);
        Task AddAsync(FilterModel model);
        Task DeleteAsync(FilterModel model);

        void AddTagToFilter(FilterModel model, BooleanOperations operation, TagModel tag);
        void RemoveTagFromFilter(FilterModel model, string tagname);
        bool VNShouldBeFilteredOut(Dictionary<BooleanOperations, List<TagModel>> filter, List<int> tagIDs);

        void ApplyFilter(FilterModel model);
        void ResetFilters();

        event EventHandler<FilterModel> OnApplied;
        event EventHandler OnReset;
    }
}
