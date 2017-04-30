using CodeKicker.BBCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.CustomClasses.Collections;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.GUI.ViewModels
{
    class AddVisualNovelsViewModel : ViewModelBase, IAddVisualNovelsWindowModel
    {
        private IVNService _VNService;

        public AddVisualNovelsViewModel(IVNService VNService)
            : base()
        {
            _VNService = VNService;

            _SelectedVisualNovel = new VisualNovelModel();
            _FoundVisualNovels = new AsyncObservableCollection<VisualNovelModel>();                 

            _FoundVisualNovels.CollectionChanged += OnCollectionChanged;            
        }

        private List<VisualNovelModel> ExistingVisualNovels
        {
            get { return _VNService.GetLocal().ToList(); }
        }

        private AsyncObservableCollection<VisualNovelModel> _FoundVisualNovels;

        public AsyncObservableCollection<VisualNovelModel> FoundVisualNovels
        {
            get { return _FoundVisualNovels; }
            set
            {
                _FoundVisualNovels = value;

                OnPropertyChanged(nameof(FoundVisualNovels));
            }
        }

        private VisualNovelModel _SelectedVisualNovel;

        public VisualNovelModel SelectedVisualNovel
        {
            get { return _SelectedVisualNovel; }
            set
            {
                _SelectedVisualNovel = value;

                OnPropertyChanged(nameof(SelectedVisualNovel));
                OnPropertyChanged(nameof(ThumbNail));
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value.Trim();

                OnPropertyChanged(nameof(Title));
            }
        }

        public string ThumbNail
        {
            get { return _SelectedVisualNovel?.Basics?.ThumbNail.Path; }
        }    
        
        public string Description
        {
            get { return _SelectedVisualNovel?.Basics?.Description != null ? BBCode.ToHtml(_SelectedVisualNovel.Basics.Description) : string.Empty; }
        }

        public List<string> Categories
        {
            get { return Enum.GetNames(typeof(VisualNovelModel.VisualNovelCatergory)).ToList(); }
        }

        private ICommand _Fetch;

        public ICommand Fetch
        {
            get {
                return _Fetch ??
                  (_Fetch = new RelayCommand(async _ => await ExecuteFetchVisualNovels(null)));
            }
        }

        private ICommand _AddVN;

        public ICommand AddVN
        {
            get
            {
                return _AddVN ??
                    (_AddVN = new RelayCommand(async (paramter) => await ExecuteAddVN(paramter), x => _SelectedVisualNovel != null));
            }
        }

        private async Task ExecuteAddVN(object parameter)
        {
            var category = (VisualNovelModel.VisualNovelCatergory)Enum.Parse(typeof(VisualNovelModel.VisualNovelCatergory), parameter.ToString(), true);

            var newVN = await _VNService.Get(_SelectedVisualNovel.Basics.ID);

            newVN.Category = category;

            _VNService.Add(newVN);
            _VNService.SetVNList(newVN);

            FoundVisualNovels.Remove(_SelectedVisualNovel);
        }

        public async Task ExecuteFetchVisualNovels(object paramter)
        {
            var newVNs = await _VNService.Get(_Title);

            FoundVisualNovels = new AsyncObservableCollection<VisualNovelModel>(newVNs.Where(x => !ExistingVisualNovels.Any(y => y.Basics.ID == x.Basics.ID)));
        }
    }    
}
