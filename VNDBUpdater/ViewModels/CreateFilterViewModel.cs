using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Models;
using VNDBUpdater.Helper;
using VNDBUpdater.Communication.Database;

namespace VNDBUpdater.ViewModels
{
    class CreateFilterViewModel : ViewModelBase
    {       
        private string _SelectedTag;
        private string _SearchCommand;
        private string _FilterName;

        private Filter _Filter;

        public CreateFilterViewModel() 
            : base()
        {
            _Filter = new Filter();

            _Commands.AddCommand("AddAndOperation", ExecuteAddAndOperation, CanExecuteAddOperation);
            _Commands.AddCommand("AddOrOperation", ExecuteAddOrOperation, CanExecuteAddOperation);
            _Commands.AddCommand("AddNotOperation", ExecuteAddNotOperation, CanExecuteAddOperation);
            _Commands.AddCommand("Clear", ExecuteClearCommand, CanExecuteClearCommand);
            _Commands.AddCommand("Save", ExecuteSaveCommand, CanExecuteSaveCommand);
        }

        public List<string> Tags
        {
            get { return TagHelper.LocalTags.Select(x => x.Name).OrderBy(x => x).ToList(); }
        }

        public string SelectedTag
        {
            get { return _SelectedTag; }
            set
            {
                _SelectedTag = value;

                OnPropertyChanged(nameof(SelectedTag));
            }
        }

        public string SearchCommand
        {
            get { return _SearchCommand; }
            set
            {
                _SearchCommand = value;

                OnPropertyChanged(nameof(SearchCommand));
            }
        }

        public string FilterName
        {
            get { return _FilterName; }
            set
            {
                _FilterName = value;

                OnPropertyChanged(nameof(FilterName));
            }
        }

        public void ExecuteAddAndOperation(object parameter)
        {
            _Filter.AddAnd(new Tag { Name = _SelectedTag });
            SearchCommand = _Filter.ToDetailedString();
        }

        public void ExecuteAddOrOperation(object parameter)
        {
            _Filter.AddOr(new Tag { Name = _SelectedTag });
            SearchCommand = _Filter.ToDetailedString();
        }

        public void ExecuteAddNotOperation(object parameter)
        {
            _Filter.AddExlude(new Tag { Name = _SelectedTag });
            SearchCommand = _Filter.ToDetailedString();
        }

        public bool CanExecuteAddOperation(object parameter)
        {
            if (_SelectedTag != null)
            {
                if (_SearchCommand == null)
                    return true;

                if (!string.IsNullOrEmpty(_SelectedTag))
                    return !_Filter.ContainsTag(new Tag { Name = _SelectedTag });
                else
                    return false;
            }
            else
                return false;
        }

        public void ExecuteClearCommand(object parameter)
        {
            _SearchCommand = string.Empty;
            SearchCommand = string.Empty;
            _Filter = new Filter();
        }

        public void ExecuteSaveCommand(object parameter)
        {
            _Filter.Name = _FilterName;
            RedisCommunication.SaveFilter(_Filter);
        }

        public bool CanExecuteSaveCommand(object parameter)
        {
            if ((_SearchCommand != null) && (_FilterName != null))
                return true;
            else
                return false;
        }

        public bool CanExecuteClearCommand(object parameter)
        {
            if (_SearchCommand != null)
                return !string.IsNullOrEmpty(_SearchCommand);
            else
                return false;
        }
    }
}
