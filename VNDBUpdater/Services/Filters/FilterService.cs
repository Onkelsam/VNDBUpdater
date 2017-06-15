using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;
using static VNDBUpdater.GUI.Models.VisualNovel.FilterModel;

namespace VNDBUpdater.Services.Filters
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _FilterRepository;

        public event EventHandler<FilterModel> OnAdded = delegate { };
        public event EventHandler<FilterModel> OnUpdated = delegate { };
        public event EventHandler<FilterModel> OnDeleted = delegate { };
        public event EventHandler<FilterModel> OnApplied = delegate { };
        public event EventHandler OnReset = delegate { };

        public FilterService(IFilterRepository filterRepository)
        {
            _FilterRepository = filterRepository;
        }

        public async Task AddAsync(FilterModel model)
        {
            await _FilterRepository.AddAsync(model);

            OnAdded?.Invoke(this, model);
        }

        public void AddTagToFilter(FilterModel model, FilterModel.BooleanOperations operation, TagModel tag)
        {
            foreach (var entry in model.FilterParameter)
            {
                if (entry.Value.Any(x => x.Name == tag.Name))
                {
                    return;
                }
            }

            model.FilterParameter[operation].Add(tag);
        }

        public async Task DeleteAsync(FilterModel model)
        {
            await _FilterRepository.DeleteAsync(model.Name);

            OnDeleted?.Invoke(this, model);
        }

        public async Task<IList<FilterModel>> GetAsync()
        {
            return await _FilterRepository.GetAsync();
        }

        public async Task<FilterModel> GetAsync(string name)
        {
            return await _FilterRepository.GetAsync(name);
        }

        public void RemoveTagFromFilter(FilterModel model, string tagname)
        {
            foreach (var entry in model.FilterParameter)
            {
                if (entry.Value.Any(x => x.Name == tagname))
                {
                    entry.Value.Remove(entry.Value.First(x => x.Name == tagname));
                }
            }
        }

        public bool VNShouldBeFilteredOut(Dictionary<BooleanOperations, List<TagModel>> filter, List<int> tagIDs)
        {
            var containsNOTTags = filter.ContainsKey(BooleanOperations.NOT) 
                ? tagIDs.Any(x => filter[BooleanOperations.NOT].Any(y => y.ID == x)) 
                : false;

            if (containsNOTTags)
            {
                return true;
            }

            var containsORTags = filter.ContainsKey(BooleanOperations.OR)
                ? tagIDs.Any(x => filter[BooleanOperations.OR].Any(y => y.ID == x))
                : false;

            if (containsORTags)
            {
                return false;
            }

            if (filter.ContainsKey(BooleanOperations.AND))
            {
                if (filter[BooleanOperations.AND].Any())
                {
                    var containsAllANDTags = filter[BooleanOperations.AND]
                        .Select(x => x.ID)
                        .Intersect(tagIDs.Select(x => x))
                        .Count() == filter[BooleanOperations.AND].Count;

                    if (containsAllANDTags)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void ResetFilters()
        {
            OnReset?.Invoke(this, null);
        }

        public void ApplyFilter(FilterModel model)
        {
            OnApplied?.Invoke(this, model);
        }
    }
}
