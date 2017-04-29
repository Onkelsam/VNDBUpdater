using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class CharacterInformationEntity
    {
        public CharacterInformationEntity()
        {
            Screenshot = new ScreenshotEntity();
            Traits = new List<TraitEntity>();
        }

        public CharacterInformationEntity(CharacterInformationModel model)
        {
            ID = model.ID;
            Name = model.Name;
            Description = model.Description;
            Screenshot = new ScreenshotEntity(model.Image);
            Traits = model.Traits?.Select(x => new TraitEntity(x)).ToList();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ScreenshotEntity Screenshot { get; set; }
        public List<TraitEntity> Traits { get; set; }
    }
}
