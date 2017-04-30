using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.Communication.VNDB.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.VN
{
    public class VNService : IVNService
    {
        private IVNRepository _VNRepository;
        private IVNDBSetter _VNDBSetter;
        private IVNDBGetter _VNDBGetter;

        private const string _WalkthroughFileName = "walkthrough.txt";
        private const string _VNDBVNLink = "https://vndb.org/v";
        private const string _GoogleLink = "https://www.google.de/#q=";

        private static event Action<VisualNovelModel> _OnVisualNovelAdded = delegate { };
        private static event Action<VisualNovelModel> _OnVisualNovelDeleted = delegate { };
        private static event Action<VisualNovelModel> _OnVisualNovelUpdated = delegate { };

        private static event Action _OnRefreshAll = delegate { };

        public VNService(IVNRepository VNRepository, IVNDBSetter VNDBSetter, IVNDBGetter VNDBGetter)
        {
            _VNRepository = VNRepository;
            _VNDBSetter = VNDBSetter;
            _VNDBGetter = VNDBGetter;
        }

        public void Add(VisualNovelModel model)
        {
            _VNRepository.Add(new VisualNovelEntity(model));

            _OnVisualNovelAdded?.Invoke(model);
        }

        public void Add(IList<VisualNovelModel> models)
        {
            models.ToList().ForEach(x => _VNRepository.Add(new VisualNovelEntity(x)));

            _OnRefreshAll?.Invoke();
        }

        public IList<VisualNovelModel> GetLocal()
        {
            return _VNRepository.Get().Select(x => new VisualNovelModel(x)).ToList();
        }

        public VisualNovelModel GetLocal(int ID)
        {
            return new VisualNovelModel(_VNRepository.Get(ID));
        }

        public async Task<IList<VisualNovelModel>> Get(string title)
        {
            return await _VNDBGetter.Get(title);
        }

        public async Task<IList<VisualNovelModel>> Get(List<int> IDs)
        {
            return await _VNDBGetter.Get(IDs);
        }

        public async Task<VisualNovelModel> Get(int ID)
        {
            return await _VNDBGetter.Get(ID);
        }

        public async Task<IList<Communication.VNDB.Entities.VN>> GetVNList()
        {
            return await _VNDBGetter.GetVNList();
        }

        public async Task<IList<Communication.VNDB.Entities.Vote>> GetVoteList()
        {
            return await _VNDBGetter.GetVoteList();
        }

        public void SetVNList(VisualNovelModel model)
        {
            _VNDBSetter.AddToVNList(model);
        }

        public bool VNExists(int ID)
        {
            return _VNRepository.VisualNovelExists(ID);
        }

        public void Delete(VisualNovelModel model)
        {
            _VNRepository.Delete(model.Basics.ID);
            _VNDBSetter.RemoveFromVNList(model);
            _VNDBSetter.RemoveFromScoreList(model);

            _OnVisualNovelDeleted?.Invoke(model);
        }

        public void CreateWalkthrough(VisualNovelModel model)
        {
            if (InstallationPathExists(model) && !WalkthroughAvailable(model))
            {
                CreateFile(model.FolderPath + _WalkthroughFileName);

                OpenWalkthrough(model);

                _VNRepository.Add(new VisualNovelEntity(model));
            }
        }

        private void CreateFile(string path)
        {
            if (!File.Exists(path))
            {
                using (var fs = File.Create(path)) { };
            }
        }

        public void OpenFolder(VisualNovelModel model)
        {
            if (InstallationPathExists(model))
            {
                Process.Start(model.FolderPath);
            }
        }

        public void OpenWalkthrough(VisualNovelModel model)
        {
            if (WalkthroughAvailable(model))
            {
                Process.Start(model.FolderPath + _WalkthroughFileName);
            }
        }

        public void SearchOnGoggle(VisualNovelModel model, string searchParam)
        {
            if (!string.IsNullOrEmpty(model.Basics.OriginalTitle))
            {
                Process.Start(_GoogleLink + model.Basics.OriginalTitle + "+" + searchParam);
            }
            else
            {
                Process.Start(_GoogleLink + model.Basics.Title + "+" + searchParam);
            }
        }

        public void SetCategory(VisualNovelModel model, VisualNovelModel.VisualNovelCatergory category)
        {
            model.Category = category;

            _VNRepository.Add(new VisualNovelEntity(model));
            _VNDBSetter.AddToVNList(model);

            _OnVisualNovelUpdated?.Invoke(model);
        }

        public void SetExePath(VisualNovelModel model, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                model.ExePath = path;

                if (Path.GetExtension(Path.GetFileName(model.ExePath)).ToLower() == ".exe")
                {
                    model.FolderPath = !string.IsNullOrEmpty(model.ExePath) ? model.ExePath.Replace(Path.GetFileName(model.ExePath), "") : string.Empty;
                }
                else
                {
                    model.FolderPath = model.ExePath;
                }

                _VNRepository.Add(new VisualNovelEntity(model));
            }
        }

        public void SetScore(VisualNovelModel model, int score)
        {
            if (score < 0 || score > 100)
            {
                return;
            }

            model.Score = score;

            if (model.Score == 0)
            {
                _VNDBSetter.RemoveFromScoreList(model);
            }
            else
            {
                _VNDBSetter.AddToScoreList(model);
            }

            _VNRepository.Add(new VisualNovelEntity(model));
        }

        public void Start(VisualNovelModel model)
        {
            if (InstallationPathExists(model))
            {
                Process.Start(model.ExePath);
            }
        }

        public void AddToPlayTime(VisualNovelModel model, TimeSpan timeToAdd)
        {
            model.PlayTime += timeToAdd;

            _VNRepository.Add(new VisualNovelEntity(model));

            _OnVisualNovelUpdated?.Invoke(model);
        }

        public async Task Update(VisualNovelModel model)
        {
            VisualNovelModel updatedVisualNovel = await _VNDBGetter.Get(model.Basics.ID);

            model.Basics = updatedVisualNovel.Basics;
            model.Characters = updatedVisualNovel.Characters;

            _VNRepository.Add(new VisualNovelEntity(model));

            _OnVisualNovelUpdated?.Invoke(model);
        }

        public void ViewOnVNDB(VisualNovelModel model)
        {
            Process.Start(_VNDBVNLink + model.Basics.ID);
        }

        public void ViewRelationOnVNDB(VisualNovelModel model, string relationTitle)
        {
            Process.Start(_VNDBVNLink + model.Basics.Relations.First(x => x.Title == relationTitle).ID);
        }

        public bool InstallationPathExists(VisualNovelModel model)
        {
            return !string.IsNullOrEmpty(model.ExePath) ? File.Exists(model.ExePath) : false;
        }

        public bool WalkthroughAvailable(VisualNovelModel model)
        {
            return !string.IsNullOrEmpty(model.FolderPath) ? File.Exists(model.FolderPath + _WalkthroughFileName) : false;
        }

        public void SubscribeToTAdded(Action<VisualNovelModel> onTAdded)
        {
            if (!_OnVisualNovelAdded.GetInvocationList().Contains(onTAdded))
            {
                _OnVisualNovelAdded += onTAdded;
            }
        }

        public void SubscribeToTUpdated(Action<VisualNovelModel> onTUpdated)
        {
            if (!_OnVisualNovelUpdated.GetInvocationList().Contains(onTUpdated))
            {
                _OnVisualNovelUpdated += onTUpdated;
            }
        }

        public void SubscribeToTDeleted(Action<VisualNovelModel> onTDeleted)
        {
            if (!_OnVisualNovelDeleted.GetInvocationList().Contains(onTDeleted))
            {
                _OnVisualNovelDeleted += onTDeleted;
            }
        }

        public void SubscribeToRefreshAll(Action onRefreshed)
        {
            if (!_OnRefreshAll.GetInvocationList().Contains(onRefreshed))
            {
                _OnRefreshAll += onRefreshed;
            }
        }

        public void DownloadImages(VisualNovelModel model)
        {            
            var newScreenshots = new List<ScreenshotModel>(model.Basics.Screenshots);
            var newCharimages = new List<ScreenshotModel>(model.Characters.Select(x => x.Image));

            model.Basics.Screenshots.Clear();

            foreach (var screenshot in newScreenshots)
            {
                model.Basics.Screenshots.Add(UpdateImages(screenshot, model.Basics.ID, "Screenshots"));
            }
            foreach (var character in model.Characters)
            {
                character.Image = UpdateImages(character.Image, model.Basics.ID, "CharacterImages");
            }

            _VNRepository.Add(new VisualNovelEntity(model));

            _OnVisualNovelUpdated?.Invoke(model);
        }

        private ScreenshotModel UpdateImages(ScreenshotModel screenshot, int visualNovelId, string path)
        {
            string ImageFolder = @"Resources\" + path;
            string CurrentFolder = AppDomain.CurrentDomain.BaseDirectory;

            if (!string.IsNullOrEmpty(screenshot.Path))
            {
                string newPath = Path.Combine(CurrentFolder, ImageFolder, visualNovelId.ToString());
                string newImageFile = Path.Combine(newPath, screenshot.Path.Split('/').Last());

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (File.Exists(newImageFile))
                {
                    return new ScreenshotModel(newImageFile, screenshot.NSFW, screenshot.Height, screenshot.Width);
                }

                using (var client = new WebClient())
                {
                    File.WriteAllBytes(newImageFile, client.DownloadData(screenshot.Path));

                    return new ScreenshotModel(newImageFile, screenshot.NSFW, screenshot.Height, screenshot.Width);
                }
            }
            else
            {
                return new ScreenshotModel();
            }
        }
    }
}
