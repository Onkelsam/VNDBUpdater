using System.Collections.Generic;
using System.Linq;
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
            Image = new ScreenshotModel(VNDBEntity.image, false, 0, 0);
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
            set;
        }

        public List<TraitModel> Traits
        {
            get;
            private set;
        }
    }
}
