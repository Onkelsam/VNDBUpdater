using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Services.Traits;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class CharacterInformationModel
    {
        public CharacterInformationModel(CharacterInformationEntity entity)
        {
            Name = entity.Name;
            Description = entity.Description;
            
            Image = new ScreenshotModel(entity.Screenshot);
            Traits = entity.Traits?.Select(x => new TraitModel(x)).ToList();
        }

        public CharacterInformationModel(VNCharacterInformation VNDBEntity, ITraitService TraitService)
        {
            ID = VNDBEntity.id;
            Name = VNDBEntity.name;
            Description = VNDBEntity.description != null ? VNDBEntity.description.ToString() : string.Empty;

            Image = DownloadScreenshot(new ScreenshotModel(VNDBEntity.image, false, 0, 0));
            Traits = VNDBEntity.traits.Select(x => new TraitModel(x, TraitService)).ToList();
        }

        public int ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public ScreenshotModel Image
        {
            get;
            private set;
        }

        public List<TraitModel> Traits
        {
            get;
            private set;
        }

        private ScreenshotModel DownloadScreenshot(ScreenshotModel screenshot)
        {
            string CharacterImageFolder = @"Resources\CharacterImages";
            string CurrentPath = System.AppDomain.CurrentDomain.BaseDirectory;

            if (!string.IsNullOrEmpty(screenshot.Path))
            {
                using (var client = new WebClient())
                {
                    string newPath = Path.Combine(CurrentPath, CharacterImageFolder, ID.ToString());
                    string newImageFile = Path.Combine(newPath, screenshot.Path.Split('/').Last());

                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    else
                    {
                        return new ScreenshotModel(newImageFile, screenshot.NSFW, screenshot.Height, screenshot.Width);
                    }

                    byte[] data = client.DownloadData(screenshot.Path);

                    File.WriteAllBytes(newImageFile, data);

                    return new ScreenshotModel(newImageFile, screenshot.NSFW, screenshot.Height, screenshot.Width);
                }
            }         
            else
            {
                return new ScreenshotModel(string.Empty, false, 0, 0);
            }                   
        }
    }
}
