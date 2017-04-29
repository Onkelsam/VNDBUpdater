using System;
using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class BasicInformationEntity
    {
        public BasicInformationEntity()
        {
            ThumbNail = new ScreenshotEntity();
            Screenshots = new List<ScreenshotEntity>();
            Relations = new List<RelationEntity>();
            Tags = new List<TagEntity>();
        }

        public BasicInformationEntity(BasicInformationModel model)
        {
            ID = model.ID;
            Title = model.Title;
            OriginalTitle = model.OriginalTitle;
            Length = model.Length;
            Description = model.Description;
            Release = model.Release;
            Aliases = model.Aliases;
            Rating = model.Rating;
            Popularity = model.Popularity;

            ThumbNail = new ScreenshotEntity(model.ThumbNail);
            Screenshots = model.Screenshots.Select(x => new ScreenshotEntity(x)).ToList();
            Relations = model.Relations.Select(x => new RelationEntity(x)).ToList();
            Tags = model.Tags.Select(x => new TagEntity(x)).ToList();
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public int? Length { get; set; }
        public string Description { get; set; }
        public DateTime Release { get; set; }
        public string Aliases { get; set; }
        public double Rating { get; set; }
        public double Popularity { get; set; }
        public ScreenshotEntity ThumbNail { get; set; }
        public List<ScreenshotEntity> Screenshots { get; set; }
        public List<RelationEntity> Relations { get; set; }
        public List<TagEntity> Tags { get; set; }

    }
}
