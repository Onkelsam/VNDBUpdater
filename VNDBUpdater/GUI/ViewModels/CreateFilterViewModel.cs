using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.Filters;
using VNDBUpdater.Services.Tags;

namespace VNDBUpdater.GUI.ViewModels
{
    class CreateFilterViewModel : ViewModelBase, ICreateFilterWindowModel
    {
        private IFilterService _FilterService;
        private ITagService _TagService;

        private FilterModel _Filter;
        private Dictionary<FilterModel.BooleanOperations, ObservableCollection<TagModel>> _BoolToTagMapper;

        public CreateFilterViewModel(IFilterService FilterService, ITagService TagService) 
            : base()
        {
            _FilterService = FilterService;

            _Filter = new FilterModel();
            _TagService = TagService;
            _Tags = _TagService.Get().Select(x => x).OrderBy(x => x.Name).ToList();

            _BoolToTagMapper = new Dictionary<FilterModel.BooleanOperations, ObservableCollection<TagModel>>();

            _BoolToTagMapper.Add(FilterModel.BooleanOperations.AND, new ObservableCollection<TagModel>());
            _BoolToTagMapper.Add(FilterModel.BooleanOperations.OR, new ObservableCollection<TagModel>());
            _BoolToTagMapper.Add(FilterModel.BooleanOperations.NOT, new ObservableCollection<TagModel>());

            foreach (var entry in _BoolToTagMapper)
            {
                _BoolToTagMapper[entry.Key].CollectionChanged += OnCollectionChanged;
            }
        }

        private List<TagModel> _Tags;

        public List<TagModel> Tags
        {
            get { return _Tags; }
        }

        private TagModel _SelectedTag;

        public TagModel SelectedTag
        {
            get { return _SelectedTag; }
            set
            {
                _SelectedTag = value;

                OnPropertyChanged(nameof(SelectedTag));
            }
        }

        public ObservableCollection<TagModel> IncludedWithAnd
        {
            get { return _BoolToTagMapper[FilterModel.BooleanOperations.AND]; }
        }

        public ObservableCollection<TagModel> IncludedWithOr
        {
            get { return _BoolToTagMapper[FilterModel.BooleanOperations.OR]; }
        }

        public ObservableCollection<TagModel> Excluded
        {
            get { return _BoolToTagMapper[FilterModel.BooleanOperations.NOT]; }
        }        

        private string _FilterName;

        public string FilterName
        {
            get { return _FilterName; }
            set
            {
                _FilterName = value;

                OnPropertyChanged(nameof(FilterName));
            }
        }

        private ICommand _AddToFilter;

        public ICommand AddToFilter
        {
            get
            {
                return _AddToFilter ??
                    (_AddToFilter = new RelayCommand((parameter) =>
                    {
                        var operation = (FilterModel.BooleanOperations)Enum.Parse(typeof(FilterModel.BooleanOperations), parameter.ToString());

                        _BoolToTagMapper[operation].Add(_SelectedTag);
                        _FilterService.AddTagToFilter(_Filter, operation, _SelectedTag);
                    },
                    x =>
                    {
                        if (_SelectedTag == null)
                        {
                            return false;
                        }

                        foreach (var entry in _BoolToTagMapper)
                        {
                            if (entry.Value.Any(y => y.Name == _SelectedTag.Name))
                            {
                                return false;
                            }
                        }

                        return true;
                    }));
            }
        }

        private ICommand _Clear;

        public ICommand Clear
        {
            get
            {
                return _Clear ??
                    (_Clear = new RelayCommand(
                        x => 
                        {
                            _Filter = new FilterModel();

                            foreach (var entry in _BoolToTagMapper)
                            {
                                entry.Value.Clear();
                            }
                        }
                    ));
            }
        }

        private ICommand _Save;

        public ICommand Save
        {
            get
            {
                return _Save ??
                    (_Save = new RelayCommand(
                        x => { _Filter.Name = _FilterName; _FilterService.Add(_Filter); _Filter = new FilterModel(); }, 
                        x => 
                        {
                            bool allEmpty = true;

                            foreach (var entry in _BoolToTagMapper)
                            {
                                if (entry.Value.Any())
                                {
                                    allEmpty = false;
                                }
                            }

                            return !string.IsNullOrEmpty(_FilterName) && !allEmpty;
                        }
                    ));
            }
        }

        private ICommand _RemoveTag;

        public ICommand RemoveTag
        {
            get
            {
                return _RemoveTag ??
                    (_RemoveTag = new RelayCommand(ExecuteRemoveTag));
            }
        }

        private void ExecuteRemoveTag(object parameter)
        {
            string tagName = parameter.ToString();

            _FilterService.RemoveTagFromFilter(_Filter, tagName);

            foreach (var entry in _BoolToTagMapper)
            {
                if (entry.Value.Any(x => x.Name == tagName))
                {
                    entry.Value.Remove(entry.Value.First(x => x.Name == tagName));
                    return;
                }
            }
        }
    }
}
