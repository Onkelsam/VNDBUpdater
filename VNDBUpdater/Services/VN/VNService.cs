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

        public async Task AddAsync(VisualNovelModel model)
        {
            if (!await _VNRepository.CheckIfVisualNovelExistsAsync(model.Basics.ID))
            {
                await _VNRepository.AddAsync(model);

                OnAdded?.Invoke(this, model);
            }
        }

        public async Task AddAsync(IList<VisualNovelModel> models)
        {
            foreach (var model in models)
            {
                await _VNRepository.AddAsync(model);
            }

            OnRefreshed?.Invoke(this, null);
        }

        public async Task<IList<VisualNovelModel>> GetLocalAsync()
        {
            return await _VNRepository.GetAsync();
        }

        public async Task<VisualNovelModel> GetLocalAsync(int ID)
        {
            return await _VNRepository.GetAsync(ID);
        }

        public async Task<IList<VisualNovelModel>> GetAsync(string title)
        {
            return await _VNDBGetter.GetAsync(title);
        }

        public async Task<IList<VisualNovelModel>> GetAsync(List<int> IDs)
        {
            return await _VNDBGetter.GetAsync(IDs);
        }

        public async Task<VisualNovelModel> GetAsync(int ID)
        {
            return await _VNDBGetter.GetAsync(ID);
        }

        public async Task<IList<Communication.VNDB.Entities.VN>> GetVNListAsync()
        {
            return await _VNDBGetter.GetVNListAsync();
        }

        public async Task<IList<Communication.VNDB.Entities.Vote>> GetVoteListAsync()
        {
            return await _VNDBGetter.GetVoteListAsync();
        }

        public async Task SetVNListAsync(VisualNovelModel model)
        {
            await _VNDBSetter.AddToVNListAsync(model);
        }

        public async Task<bool> CheckIfVNExistsAsync(int ID)
        {
            return await _VNRepository.CheckIfVisualNovelExistsAsync(ID);
        }

        public async Task DeleteAsync(VisualNovelModel model)
        {
            await _VNRepository.DeleteAsync(model.Basics.ID);
            await _VNDBSetter.RemoveFromVNListAsync(model);
            await _VNDBSetter.RemoveFromScoreListAsync(model);

            OnDeleted?.Invoke(this, model);
        }

        public async Task DeleteLocalAsync(VisualNovelModel model)
        {
            await _VNRepository.DeleteAsync(model.Basics.ID);

            OnDeleted?.Invoke(this, model);
        }

        public async Task CreateWalkthroughAsync(VisualNovelModel model)
        {
            if (CheckIfInstallationPathExists(model) && !CheckIfWalkthroughExists(model))
            {
                CreateFile(model.FolderPath + _WalkthroughFileName);

                OpenWalkthrough(model);

                await _VNRepository.AddAsync(model);
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
            if (CheckIfInstallationPathExists(model))
            {
                Process.Start(model.FolderPath);
            }
        }

        public void OpenWalkthrough(VisualNovelModel model)
        {
            if (CheckIfWalkthroughExists(model))
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

        public async Task SetCategoryAsync(VisualNovelModel model, VisualNovelModel.VisualNovelCatergory category)
        {
            model.Category = category;

            await _VNRepository.AddAsync(model);
            await _VNDBSetter.AddToVNListAsync(model);

            OnUpdated?.Invoke(this, model);
        }

        public async Task SetExePathAsync(VisualNovelModel model, string path)
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

                await _VNRepository.AddAsync(model);
            }
        }

        public async Task SetScoreAsync(VisualNovelModel model, int score)
        {
            if (score < 0 || score > 100)
            {
                return;
            }

            model.Score = score;

            if (model.Score == 0)
            {
                await _VNDBSetter.RemoveFromScoreListAsync(model);
            }
            else
            {
                await _VNDBSetter.AddToScoreListAsync(model);
            }

            await _VNRepository.AddAsync(model);
        }

        public void Start(VisualNovelModel model)
        {
            if (CheckIfInstallationPathExists(model))
            {
                Process.Start(model.ExePath);
            }
        }

        public async Task AddToPlayTimeAsync(VisualNovelModel model, TimeSpan timeToAdd)
        {
            model.PlayTime += timeToAdd;

            await _VNRepository.AddAsync(model);

            OnUpdated?.Invoke(this, model);
        }

        public async Task UpdateAsync(VisualNovelModel model)
        {
            VisualNovelModel updatedVisualNovel = await _VNDBGetter.GetAsync(model.Basics.ID);

            model.Basics = updatedVisualNovel.Basics;
            model.Characters = updatedVisualNovel.Characters;

            await _VNRepository.AddAsync(model);

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

        public bool CheckIfInstallationPathExists(VisualNovelModel model)
        {
            return !string.IsNullOrEmpty(model.ExePath) ? File.Exists(model.ExePath) : false;
        }

        public bool CheckIfWalkthroughExists(VisualNovelModel model)
        {
            return !string.IsNullOrEmpty(model.FolderPath) ? File.Exists(model.FolderPath + _WalkthroughFileName) : false;
        }

        public async Task DownloadImagesAsync(VisualNovelModel model)
        {            
            var newScreenshots = new List<ScreenshotModel>(model.Basics.Screenshots);
            var newCharimages = new List<ScreenshotModel>(model.Characters.Select(x => x.Image));

            model.Basics.Screenshots.Clear();

            foreach (var screenshot in newScreenshots)
            {
                model.Basics.Screenshots.Add(await UpdateImagesAsync(screenshot, model.Basics.ID, "Screenshots"));
            }
            foreach (var character in model.Characters)
            {
                character.Image = await UpdateImagesAsync(character.Image, model.Basics.ID, "CharacterImages");
            }

            await _VNRepository.AddAsync(model);

            OnUpdated?.Invoke(this, model);
        }

        private async Task<ScreenshotModel> UpdateImagesAsync(ScreenshotModel screenshot, int visualNovelId, string path)
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
                    File.WriteAllBytes(newImageFile, await client.DownloadDataTaskAsync(screenshot.Path));

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
