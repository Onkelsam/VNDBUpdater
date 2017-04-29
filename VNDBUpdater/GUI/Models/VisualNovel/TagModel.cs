using System;
using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Data;
using VNDBUpdater.Services.Tags;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class TagModel
    {
        public TagModel(TagsLookUp RawData)
        {
            ID = RawData.id;
            Name = RawData.name;
            Description = RawData.description;
            Category = (TagCategory)Enum.Parse(typeof(TagCategory), RawData.cat);
        }

        public TagModel(TagEntity entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            Description = entity.Description;
            Score = entity.Score;
            Category = entity.Category;
            Spoiler = entity.Spoiler;
        }

        public TagModel(List<double> tag, ITagService TagService)
        {
            TagModel foundTag = TagService.Get().FirstOrDefault(x => x.ID == tag[0]);

            if (foundTag != null)
            {
                ID = foundTag.ID;
                Category = foundTag.Category;
                Description = foundTag.Description;
                Name = foundTag.Name;
                Score = tag[1];
                Spoiler = (SpoilerLevel)Enum.Parse(typeof(SpoilerLevel), tag[2].ToString(), true);
            }
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

        public double Score
        {
            get;
            private set;
        }

        public TagCategory Category
        {
            get;
            private set;
        }        

        public SpoilerLevel Spoiler
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Name;
        }

        public enum TagCategory : byte
        {
            All = 0,
            cont,
            ero,
            tech
        };
    }
}
