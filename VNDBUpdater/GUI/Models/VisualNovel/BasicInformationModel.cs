using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Services.Tags;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class BasicInformationModel
    {
        public BasicInformationModel(BasicInformationEntity entity)
        {
            ID = entity.ID;
            Title = entity.Title;
            OriginalTitle = entity.OriginalTitle;
            Length = entity.Length;
            Description = entity.Description;
            Aliases = entity.Aliases;
            Release = entity.Release;
            Rating = entity.Rating;
            Popularity = entity.Popularity;

            ThumbNail = new ScreenshotModel(entity.ThumbNail);
            Screenshots = entity.Screenshots.Select(x => new ScreenshotModel(x)).ToList();
            Relations = entity.Relations.Select(x => new RelationModel(x)).ToList();
            Tags = entity.Tags.Select(x => new TagModel(x)).ToList();
        }

        public BasicInformationModel(VNInformation VNDBEntity, ITagService TagService, bool getScreenshots = true)
        {
            ID = VNDBEntity.id;
            Title = VNDBEntity.title;
            OriginalTitle = VNDBEntity.original != null ? VNDBEntity.original.ToString() : string.Empty;
            Length = VNDBEntity.length;
            Description = VNDBEntity.description;
            Aliases = VNDBEntity.aliases;
            Release = DateTime.Parse(VNDBEntity.released);
            Rating = VNDBEntity.rating;
            Popularity = VNDBEntity.popularity;
            Screenshots = VNDBEntity.screens.Select(x => new ScreenshotModel(x.image, x.nsfw, x.height, x.width)).ToList();

            ThumbNail = DownloadScreenshot(new ScreenshotModel(VNDBEntity.image, VNDBEntity.image_nsfw, 0, 0));
            Relations = VNDBEntity.relations.Select(x => new RelationModel(x)).ToList();
            Tags = VNDBEntity.tags.Select(x => new TagModel(x, TagService)).ToList();
        }

        public int ID
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public string OriginalTitle
        {
            get;
            private set;
        }

        public int? Length
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public DateTime Release
        {
            get;
            private set;
        }

        public string Aliases
        {
            get;
            private set;
        }

        public double Rating
        {
            get;
            private set;
        }

        public double Popularity
        {
            get;
            private set;
        }

        public ScreenshotModel ThumbNail
        {
            get;
            private set;
        }
        public List<ScreenshotModel> Screenshots
        {
            get;
            private set;
        }

        public List<RelationModel> Relations
        {
            get;
            private set;
        }

        public List<TagModel> Tags
        {
            get;
            private set;
        }

        private ScreenshotModel DownloadScreenshot(ScreenshotModel screenshot)
        {
            string ImageFolder = @"Resources\Thumbnails";
            string CurrentPath = AppDomain.CurrentDomain.BaseDirectory;

            if (!string.IsNullOrEmpty(screenshot.Path))
            {
                string newPath = Path.Combine(CurrentPath, ImageFolder, ID.ToString());
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
                return new ScreenshotModel(string.Empty, false, 0, 0);
            }            
        }
    }
}
