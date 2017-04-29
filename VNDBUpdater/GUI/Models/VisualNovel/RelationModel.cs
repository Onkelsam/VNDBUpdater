using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.VNDB.Entities;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class RelationModel
    {
        public RelationModel(RelationEntity entity)
        {
            ID = entity.ID;
            Relation = entity.Relation;
            Title = entity.Title;
            Original = entity.Original;
        }

        public RelationModel(Relation VNDBEntity)
        {
            ID = VNDBEntity.id;
            Relation = VNDBEntity.relation;
            Title = VNDBEntity.title;
            Original = VNDBEntity.original;
        }

        public int ID
        {
            get;
            private set;
        }

        public string Relation
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public string Original
        {
            get;
            private set;
        }
    }
}
