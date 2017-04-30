using CodeKicker.BBCode;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.Tags;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class TagTabViewModel : ViewModelBase, ITagTabWIndowModel
    {
        private UserModel _User;

        private IVisualNovelsGridWindowModel _VisualNovelsGridModel;

        private IUserService _UserService;
        private ITagService _TagService;

        public TagTabViewModel(IVisualNovelsGridWindowModel VisualNovelsGridModel, IUserService UserService, ITagService TagService)
        {
            _VisualNovelsGridModel = VisualNovelsGridModel;
            _VisualNovelsGridModel.PropertyChanged += OnSelectedVisualNovelPropertyChanged;

            _UserService = UserService;
            _TagService = TagService;
            _User = _UserService.Get();

            _UserService.SubscribeToTUpdated(OnUserUpdated);
        }

        private void OnUserUpdated(UserModel User)
        {
            _User = User;

            OnPropertyChanged(nameof(AllTags));
            OnPropertyChanged(nameof(ContentTags));
            OnPropertyChanged(nameof(SexualTags));
            OnPropertyChanged(nameof(TechnicalTags));
        }

        public List<TagModel> AllTags
        {
            get { return _VisualNovelsGridModel.SelectedVisualNovel?.Basics != null ? _VisualNovelsGridModel.SelectedVisualNovel.Basics.Tags.Where(x => _TagService.Show(_User.Settings.SpoilerSetting, x.Spoiler)).OrderByDescending(x => x.Score).ToList() : new List<TagModel>() ; }
        }

        public List<TagModel> ContentTags
        {
            get { return _VisualNovelsGridModel.SelectedVisualNovel?.Basics != null ? _VisualNovelsGridModel.SelectedVisualNovel.Basics.Tags.Where(x => x.Category == TagModel.TagCategory.cont && _TagService.Show(_User.Settings.SpoilerSetting, x.Spoiler)).OrderByDescending(x => x.Score).ToList() : new List<TagModel>(); }
        }

        public List<TagModel> SexualTags
        {
            get { return _VisualNovelsGridModel.SelectedVisualNovel?.Basics != null ? _VisualNovelsGridModel.SelectedVisualNovel.Basics.Tags.Where(x => x.Category == TagModel.TagCategory.ero && _TagService.Show(_User.Settings.SpoilerSetting, x.Spoiler)).OrderByDescending(x => x.Score).ToList() : new List<TagModel>(); }
        }

        public List<TagModel> TechnicalTags
        {
            get { return _VisualNovelsGridModel.SelectedVisualNovel?.Basics != null ? _VisualNovelsGridModel.SelectedVisualNovel.Basics.Tags.Where(x => x.Category == TagModel.TagCategory.tech && _TagService.Show(_User.Settings.SpoilerSetting, x.Spoiler)).OrderByDescending(x => x.Score).ToList() : new List<TagModel>(); }
        }

        public string Description
        {
            get { return _SelectedTag?.Description != null ? BBCode.ToHtml(_SelectedTag.Description) : string.Empty; }
        }

        private TagModel _SelectedTag;

        public TagModel SelectedTag
        {
            get { return _SelectedTag; }
            set
            {
                _SelectedTag = value;

                OnPropertyChanged(nameof(SelectedTag));
                OnPropertyChanged(nameof(Description));
            }
        }

        private void OnSelectedVisualNovelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_VisualNovelsGridModel.SelectedVisualNovel))
            {
                OnPropertyChanged(nameof(AllTags));
                OnPropertyChanged(nameof(ContentTags));
                OnPropertyChanged(nameof(SexualTags));
                OnPropertyChanged(nameof(TechnicalTags));
                OnPropertyChanged(nameof(SelectedTag));
            }
        }
    }
}
