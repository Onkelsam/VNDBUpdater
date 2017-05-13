using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.Filters
{
    public class FilterService : IFilterService
    {
        private IFilterRepository _FilterRepository;

        public event EventHandler<FilterModel> OnAdded = delegate { };
        public event EventHandler<FilterModel> OnUpdated = delegate { };
        public event EventHandler<FilterModel> OnDeleted = delegate { };
        public event EventHandler<FilterModel> OnApplied = delegate { };
        public event EventHandler OnReset = delegate { };

        public FilterService(IFilterRepository filterRepository)
        {
            _FilterRepository = filterRepository;
        }

        public async Task Add(FilterModel model)
        {
            await _FilterRepository.Add(model);

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

        public async Task Delete(FilterModel model)
        {
            await _FilterRepository.Delete(model.Name);

            OnDeleted?.Invoke(this, model);
        }

        public async Task<IList<FilterModel>> Get()
        {
            return await _FilterRepository.Get();
        }

        public async Task<FilterModel> Get(string name)
        {
            return await _FilterRepository.Get(name);
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

        public bool VNShouldBeFilteredOut(FilterModel model, VisualNovelModel vn)
        {
            bool containsNOTTags = vn.Basics.Tags.Any(x => model.FilterParameter[FilterModel.BooleanOperations.NOT].Any(y => y.ID == x.ID));

            if (containsNOTTags)
            {
                return true;
            }

            bool containsORTags = vn.Basics.Tags.Any(x => model.FilterParameter[FilterModel.BooleanOperations.OR].Any(y => y.ID == x.ID));

            if (containsORTags)
            {
                return false;
            }

            if (model.FilterParameter[FilterModel.BooleanOperations.AND].Any())
            {
                bool containsAllANDTags = model.FilterParameter[FilterModel.BooleanOperations.AND].Select(x => x.ID).Intersect(vn.Basics.Tags.Select(x => x.ID)).Count() == model.FilterParameter[FilterModel.BooleanOperations.AND].Count;

                if (containsAllANDTags)
                {
                    return false;
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
