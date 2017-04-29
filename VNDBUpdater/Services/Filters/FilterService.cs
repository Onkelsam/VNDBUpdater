using System;
using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.Filters
{
    public class FilterService : IFilterService
    {
        private IFilterRepository _FilterRepository;

        private static event Action<FilterModel> _OnFilterAdded = delegate { };
        private static event Action<FilterModel> _OnFilterUpdated = delegate { };
        private static event Action<FilterModel> _OnFilterDeleted = delegate { };

        private static event Action<FilterModel> _OnFilterApply = delegate { };
        private static event Action _OnFilterReset = delegate { };

        public FilterService(IFilterRepository filterRepository)
        {
            _FilterRepository = filterRepository;
        }

        public void Add(FilterModel model)
        {
            _FilterRepository.Add(new FilterEntity(model));

            _OnFilterAdded?.Invoke(model);
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

        public void Delete(FilterModel model)
        {
            _FilterRepository.Delete(model.Name);

            _OnFilterDeleted?.Invoke(model);
        }

        public IList<FilterModel> Get()
        {
            return _FilterRepository.Get().Select(x => new FilterModel(x)).ToList();
        }

        public FilterModel Get(string name)
        {
            return new FilterModel(_FilterRepository.Get(name));
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

        public void SubscribeToTAdded(Action<FilterModel> onTAdded)
        {
            if (!_OnFilterAdded.GetInvocationList().Contains(onTAdded))
            {
                _OnFilterAdded += onTAdded;
            }            
        }

        public void SubscribeToTDeleted(Action<FilterModel> onTDeleted)
        {
            if (!_OnFilterDeleted.GetInvocationList().Contains(onTDeleted))
            {
                _OnFilterDeleted += onTDeleted;
            }
        }

        public void SubscribeToTUpdated(Action<FilterModel> onTUpdated)
        {
            if (!_OnFilterUpdated.GetInvocationList().Contains(onTUpdated))
            {
                _OnFilterUpdated += onTUpdated;
            }
        }

        public bool VNShouldBeFilteredOut(FilterModel model, VisualNovelModel vn)
        {
            if (vn.Basics.Tags.Any(x => model.FilterParameter[FilterModel.BooleanOperations.NOT].Any(y => y.Name == x.Name)))
            {
                return true;
            }

            if (vn.Basics.Tags.Any(x => model.FilterParameter[FilterModel.BooleanOperations.OR].Any(y => y.Name == x.Name)))
            {
                return false;
            }

            if (model.FilterParameter[FilterModel.BooleanOperations.AND].Any())
            {
                if (model.FilterParameter[FilterModel.BooleanOperations.AND].Select(x => x.Name).Intersect(vn.Basics.Tags.Select(x => x.Name)).Count() == model.FilterParameter[FilterModel.BooleanOperations.AND].Count)
                {
                    return false;
                }
            }

            return true;
        }

        public void SubscribeToFilterReset(Action onFilterReset)
        {
            if (!_OnFilterReset.GetInvocationList().Contains(onFilterReset))
            {
                _OnFilterReset += onFilterReset;
            }
        }

        public void ResetFilters()
        {
            _OnFilterReset?.Invoke();
        }

        public void ApplyFilter(FilterModel model)
        {
            _OnFilterApply?.Invoke(model);
        }

        public void SubscribeToFilterApply(Action<FilterModel> onFilterApply)
        {
            if (!_OnFilterApply.GetInvocationList().Contains(onFilterApply))
            {
                _OnFilterApply += onFilterApply;
            }
        }
    }
}
