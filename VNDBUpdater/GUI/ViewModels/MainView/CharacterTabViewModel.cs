using CodeKicker.BBCode;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.Traits;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class CharacterTabViewModel : ViewModelBase, ICharacterTabWindowModel
    {
        private UserModel _User;
        private CharacterInformationModel _SelectedCharacter;        

        private IUserService _UserService;
        private ITraitService _TraitService;

        private IVisualNovelsGridWindowModel _VisualNovelsGridModel;        

        public CharacterTabViewModel(IVisualNovelsGridWindowModel VisualNovelsGridModel, IUserService UserService, ITraitService TraitService)
            : base()
        {
            _VisualNovelsGridModel = VisualNovelsGridModel;
            _VisualNovelsGridModel.PropertyChanged += OnSelectedVisualNovelPropertyChanged;

            _UserService = UserService;
            _TraitService = TraitService;

            _UserService.OnUpdated += OnUserUpdated;

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            OnUserUpdated(this, await _UserService.GetAsync());
        }

        private void OnUserUpdated(object sender, UserModel User)
        {
            _User = User;

            OnPropertyChanged(nameof(Traits));
        }

        public List<CharacterInformationModel> Characters
        {
            get
            {
                if (_VisualNovelsGridModel.SelectedVisualNovel?.Characters != null)
                {
                    if (_VisualNovelsGridModel.SelectedVisualNovel.Characters.Any())
                    {
                        SelectedCharacter = _VisualNovelsGridModel.SelectedVisualNovel.Characters.First();
                    }
                    else
                    {
                        _SelectedCharacter = null;
                        ResetCharacter();
                    }

                    return _VisualNovelsGridModel.SelectedVisualNovel.Characters;
                }
                else
                {
                    _SelectedCharacter = null;
                    ResetCharacter();

                    return null;
                }
            }
        }

        public CharacterInformationModel SelectedCharacter
        {
            get { return _SelectedCharacter; }
            set
            {
                if (value != null)
                {
                    _SelectedCharacter = value;

                    ResetCharacter();
                }
            }
        }

        public string CharacterName
        {
            get { return _SelectedCharacter != null ? _SelectedCharacter.Name : "Character Name"; }
        }

        public Dictionary<TraitModel, string> Traits
        {
            get { return _SelectedCharacter != null ? ConvertTraits(_SelectedCharacter.Traits) : new Dictionary<TraitModel, string>(); }
        }

        public string Description
        {
            get { return _SelectedCharacter?.Description != null ? BBCode.ToHtml(SelectedCharacter.Description) : string.Empty; }
        }

        private Dictionary<TraitModel, string> ConvertTraits(List<TraitModel> traits)
        {
            var TraitsWithParent = new Dictionary<TraitModel, List<string>>();

            foreach (var trait in traits)
            {
                if (_TraitService.Show(_User.Settings.SpoilerSetting, trait.Spoiler))
                {
                    TraitModel parenttrait = _TraitService.GetLastParentTrait(trait);

                    if (TraitsWithParent.Keys.Any(x => x.Name == parenttrait.Name))
                    {
                        TraitsWithParent[TraitsWithParent.Keys.First(x => x.Name == parenttrait.Name)].Add(trait.Name);
                    }
                    else
                    {
                        TraitsWithParent.Add(parenttrait, new List<string>() { trait.Name });
                    }
                }
            }    

            return TraitsWithParent
                .OrderBy(x => x.Key.Name)
                .ToDictionary(x => x.Key, y => string.Join(", ", y.Value.OrderBy(z => z).ToList()));
        }

        private void ResetCharacter()
        {
            OnPropertyChanged(nameof(SelectedCharacter));
            OnPropertyChanged(nameof(CharacterName));
            OnPropertyChanged(nameof(Traits));
            OnPropertyChanged(nameof(Description));
        }

        private void OnSelectedVisualNovelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_VisualNovelsGridModel.SelectedVisualNovel))
            {              
                OnPropertyChanged(nameof(Characters));
            }
        }
    }
}
