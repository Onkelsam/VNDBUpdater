using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        public event EventHandler<VisualNovelModel> OnAdded = delegate { };
        public event EventHandler<VisualNovelModel> OnUpdated = delegate { };
        public event EventHandler<VisualNovelModel> OnDeleted = delegate { };
        public event EventHandler OnRefreshed = delegate { };

        public VNService(IVNRepository VNRepository, IVNDBSetter VNDBSetter, IVNDBGetter VNDBGetter)
        {
            _VNRepository = VNRepository;
            _VNDBSetter = VNDBSetter;
            _VNDBGetter = VNDBGetter;
        }

        public async Task Add(VisualNovelModel model)
        {
            await _VNRepository.Add(model);

            OnAdded?.Invoke(this, model);
        }

        public async Task Add(IList<VisualNovelModel> models)
        {
            models.ToList().ForEach(async x => await _VNRepository.Add(x));

            OnRefreshed?.Invoke(this, null);
        }

        public async Task<IList<VisualNovelModel>> GetLocal()
        {
            return await _VNRepository.Get();
        }

        public async Task<VisualNovelModel> GetLocal(int ID)
        {
            return await _VNRepository.Get(ID);
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

        public async Task SetVNList(VisualNovelModel model)
        {
            await _VNDBSetter.AddToVNList(model);
        }

        public async Task<bool> VNExists(int ID)
        {
            return await _VNRepository.VisualNovelExists(ID);
        }

        public async Task Delete(VisualNovelModel model)
        {
            await _VNRepository.Delete(model.Basics.ID);
            await _VNDBSetter.RemoveFromVNList(model);
            await _VNDBSetter.RemoveFromScoreList(model);

            OnDeleted?.Invoke(this, model);
        }

        public async Task CreateWalkthrough(VisualNovelModel model)
        {
            if (InstallationPathExists(model) && !WalkthroughAvailable(model))
            {
                CreateFile(model.FolderPath + _WalkthroughFileName);

                OpenWalkthrough(model);

                await _VNRepository.Add(model);
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

        public async Task SetCategory(VisualNovelModel model, VisualNovelModel.VisualNovelCatergory category)
        {
            model.Category = category;

            await _VNRepository.Add(model);
            await _VNDBSetter.AddToVNList(model);

            OnUpdated?.Invoke(this, model);
        }

        public async Task SetExePath(VisualNovelModel model, string path)
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

                await _VNRepository.Add(model);
            }
        }

        public async Task SetScore(VisualNovelModel model, int score)
        {
            if (score < 0 || score > 100)
            {
                return;
            }

            model.Score = score;

            if (model.Score == 0)
            {
                await _VNDBSetter.RemoveFromScoreList(model);
            }
            else
            {
                await _VNDBSetter.AddToScoreList(model);
            }

            await _VNRepository.Add(model);
        }

        public void Start(VisualNovelModel model)
        {
            if (InstallationPathExists(model))
            {
                Process.Start(model.ExePath);
            }
        }

        public async Task AddToPlayTime(VisualNovelModel model, TimeSpan timeToAdd)
        {
            model.PlayTime += timeToAdd;

            await _VNRepository.Add(model);

            OnUpdated?.Invoke(this, model);
        }

        public async Task Update(VisualNovelModel model)
        {
            VisualNovelModel updatedVisualNovel = await _VNDBGetter.Get(model.Basics.ID);

            model.Basics = updatedVisualNovel.Basics;
            model.Characters = updatedVisualNovel.Characters;

            await _VNRepository.Add(model);

            OnUpdated?.Invoke(this, model);
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

        public async Task DownloadImages(VisualNovelModel model)
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

            await _VNRepository.Add(model);

            OnUpdated?.Invoke(this, model);
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
