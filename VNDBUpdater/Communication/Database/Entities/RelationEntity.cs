using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class RelationEntity
    {
        public RelationEntity()
        {
        }

        public RelationEntity(RelationModel model)
        {
            ID = model.ID;
            Relation = model.Relation;
            Title = model.Title;
            Original = model.Original;
        }

        public int ID { get; set; }
        public string Relation { get; set; }
        public string Title { get; set; }
        public string Original { get; set; }
    }
}
