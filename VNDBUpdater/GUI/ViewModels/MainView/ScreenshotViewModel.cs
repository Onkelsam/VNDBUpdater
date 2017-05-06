using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.VN;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class ScreenshotViewModel : ViewModelBase, IScreenshotTabWindowModel
    {
        private UserModel _User;

        private IVisualNovelsGridWindowModel _VisualNovelsGridModel;
        private IUserService _UserService;
        private IVNService _VNService;

        public ScreenshotViewModel(IVisualNovelsGridWindowModel VisualNovelsGridModel, IUserService UserService, IVNService VNService)
            : base()
        {
            _VisualNovelsGridModel = VisualNovelsGridModel;
            _VisualNovelsGridModel.PropertyChanged += OnSelectedVisualNovelPropertyChanged;

            _UserService = UserService;
            _VNService = VNService;

            _UserService.OnUpdated += OnUserUpdated;

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            OnUserUpdated(this, await _UserService.Get());
        }

        private void OnUserUpdated(object sender, UserModel User)
        {
            _User = User;

            OnPropertyChanged(nameof(Screenshots));
        }

        public List<ScreenshotModel> Screenshots
        {
            get { return _VisualNovelsGridModel.SelectedVisualNovel?.Basics != null
                    ? _User.Settings.ShowNSFWImages 
                        ? _VisualNovelsGridModel.SelectedVisualNovel.Basics.Screenshots 
                        : _VisualNovelsGridModel.SelectedVisualNovel.Basics.Screenshots.Where(x => x.NSFW == false).ToList() 
                    : new List<ScreenshotModel>(); }
        }

        private void OnSelectedVisualNovelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_VisualNovelsGridModel.SelectedVisualNovel))
            {
                if (_VisualNovelsGridModel.SelectedVisualNovel?.Basics?.Screenshots != null)
                {
                    if (_VisualNovelsGridModel.SelectedVisualNovel.Basics.Screenshots.Any(x => x.Path.Contains("https://")))
                    {
                        Task.Factory.StartNew(() => _VNService.DownloadImages(_VisualNovelsGridModel.SelectedVisualNovel));
                    }
                    else
                    {
                        OnPropertyChanged(nameof(Screenshots));
                    }
                }
            }
        }
    }
}
