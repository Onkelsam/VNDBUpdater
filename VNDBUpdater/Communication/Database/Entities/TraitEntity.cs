using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Data;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class TraitEntity
    {
        public TraitEntity()
        {
            ParentTraits = new List<TraitEntity>();
        }

        public TraitEntity(TraitModel model)
        {
            ID = model.ID;
            Name = model.Name;
            Description = model.Description;
            Spoiler = model.Spoiler;
            ParentTraits = model.ParentTraits?.Select(x => new TraitEntity(x)).ToList();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SpoilerLevel Spoiler { get; set; }
        public List<TraitEntity> ParentTraits { get; set; }
    }
}
