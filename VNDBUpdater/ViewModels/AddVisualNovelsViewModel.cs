using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;

namespace VNDBUpdater.ViewModels
{
    class AddVisualNovelsViewModel : ViewModelBase
    {
        private AsyncObservableCollection<VisualNovel> _VisualNovelsToAdd;
        private VisualNovel _SelectedVisualNovel;

        private string _IDs;

        private List<VisualNovel> ExistingVisualNovels
        {
            get { return LocalVisualNovelHelper.LocalVisualNovels; }
        }
                
        public AddVisualNovelsViewModel()
            : base()
        {
            _Commands.AddCommand("FetchVisualNovels", ExecuteFetchVisualNovels, CanExecuteFetchVisualNovels);
            _Commands.AddCommand("AddVisualNovelsToDB", ExecuteAddVisualNovelsToDB, CanExecuteAddVisualNovelsToDB);
            _Commands.AddCommand("SetExePath", ExecuteSetExePath, CanExecuteSetExePath);

            _VisualNovelsToAdd = new AsyncObservableCollection<VisualNovel>();
            _SelectedVisualNovel = new VisualNovel();            

            _VisualNovelsToAdd.CollectionChanged += OnCollectionChanged;
        }

        public AsyncObservableCollection<VisualNovel> VisualNovelsToAdd
        {
            get { return _VisualNovelsToAdd; }
            set { _VisualNovelsToAdd = value; }
        }

        public VisualNovel SelectedVisualNovel
        {
            get { return _SelectedVisualNovel; }
            set
            {
                _SelectedVisualNovel = value;

                OnPropertyChanged(nameof(SelectedVisualNovel));
            }
        }

        public string IDs
        {
            get { return _IDs; }
            set
            {
                _IDs = value.Trim();

                OnPropertyChanged(nameof(IDs));
            }
        }

        public bool CanExecuteFetchVisualNovels(object paramter)
        {
            return InputValidation.IsValid;
        }

        public void ExecuteFetchVisualNovels(object paramter)
        {
            foreach (var visualNovel in VNDBCommunication.FetchVisualNovels(GetIDsAsInt()))
            {
                if (!_VisualNovelsToAdd.Any(x => x.Basics.id == visualNovel.Basics.id))
                    _VisualNovelsToAdd.Add(visualNovel);            
            }

            IDs = string.Empty;
        }

        public bool CanExecuteAddVisualNovelsToDB(object paramter)
        {
            return _VisualNovelsToAdd.Any(x => x.AlreadyExistsInDatabase == false);
        }

        public void ExecuteAddVisualNovelsToDB(object parameter)
        {
            List<VisualNovel> newVisualNovels = _VisualNovelsToAdd.Where(x => x.AlreadyExistsInDatabase == false).ToList();
            
            foreach (var visualNovel in newVisualNovels)
                visualNovel.SetCategory(VisualNovelCatergory.Unknown);

            RedisCommunication.AddVisualNovelsToDB(newVisualNovels);
        }

        public bool CanExecuteSetExePath(object paramter)
        {
            if (_SelectedVisualNovel != null)
                return true;
            else
                return false;
        }

        public void ExecuteSetExePath(object parameter)
        {
            var visualNovel = (parameter as VisualNovel);

            visualNovel.ExePath = FileHelper.GetExePath();
        }

        private List<int> GetIDsAsInt()
        {
            var IDsAsInt = new List<int>();

            foreach (string id in _IDs.Split(','))
            {
                int result;
                if (int.TryParse(id, out result))
                {
                    if ((!IDsAsInt.Contains(result) && (result > 0))) { IDsAsInt.Add(result); }
                }
            }

            return IDsAsInt;
        }
    }    
}
