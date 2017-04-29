using System;
using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Data;
using VNDBUpdater.Services.Traits;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class TraitModel
    {
        public TraitModel(TraitsLookUp RawData)
        {
            ID = RawData.id;
            Name = RawData.name;
            Description = RawData.description;
            ParentTraits = new List<TraitModel>();
        }

        public TraitModel(TraitEntity entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            Description = entity.Description;

            ParentTraits = entity.ParentTraits?.Select(x => new TraitModel(x)).ToList();
            Spoiler = entity.Spoiler;
        }

        public TraitModel(List<int> trait, ITraitService TraitService)
        {
            TraitModel foundTrait = TraitService.Get().FirstOrDefault(x => x.ID == trait[0]);

            if (foundTrait != null)
            {
                ID = foundTrait.ID;
                Name = foundTrait.Name;
                Description = foundTrait.Description;
                Spoiler = (SpoilerLevel)Enum.Parse(typeof(SpoilerLevel), trait[1].ToString(), true);
                ParentTraits = foundTrait.ParentTraits;
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

        public List<TraitModel> ParentTraits
        {
            get;
            private set;
        }

        public SpoilerLevel Spoiler
        {
            get;
            private set;
        }
    }
}
