using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class FilterEntity
    {
        public FilterEntity()
        {
            FilterParameter = new Dictionary<FilterModel.BooleanOperations, List<TagEntity>>();
        }

        public FilterEntity(FilterModel model)
            : this()
        {
            Name = model.Name;
            
            foreach (var entry in model.FilterParameter)
            {
                FilterParameter.Add(entry.Key, entry.Value.Select(x => new TagEntity(x)).ToList());
            }
        }

        public string Name { get; set; }
        public Dictionary<FilterModel.BooleanOperations, List<TagEntity>> FilterParameter { get; set; }
    }
}
