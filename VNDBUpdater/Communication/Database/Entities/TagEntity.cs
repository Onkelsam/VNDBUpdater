using VNDBUpdater.Data;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class TagEntity
    {
        public TagEntity()
        {
        }

        public TagEntity(TagModel model)
        {
            ID = model.ID;
            Name = model.Name;
            Description = model.Description;
            Category = model.Category;
            Spoiler = model.Spoiler;
            Score = model.Score;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }       
        public TagModel.TagCategory Category { get; set; }
        public SpoilerLevel Spoiler { get; set; }
        public double Score { get; set; }
    }
}
