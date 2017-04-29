using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class ScreenshotViewModel : ViewModelBase, IScreenshotTabWindowModel
    {
        private UserModel _User;

        private IVisualNovelsGridWindowModel _VisualNovelsGridModel;
        private IUserService _UserService;

        public ScreenshotViewModel(IVisualNovelsGridWindowModel VisualNovelsGridModel, IUserService UserService)
            : base()
        {
            _VisualNovelsGridModel = VisualNovelsGridModel;
            _VisualNovelsGridModel.PropertyChanged += OnSelectedVisualNovelPropertyChanged;

            _UserService = UserService;
            _User = _UserService.Get();

            _UserService.SubscribeToTUpdated(OnUserUpdated);
        }

        private void OnUserUpdated(UserModel User)
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
                OnPropertyChanged(nameof(Screenshots));
            }
        }
    }
}
