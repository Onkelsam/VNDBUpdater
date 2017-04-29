using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Communication.Database.Entities;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class FilterModel
    {
        public FilterModel()
        {
            FilterParameter = new Dictionary<BooleanOperations, List<TagModel>>()
            {
                { BooleanOperations.AND, new List<TagModel>() },
                { BooleanOperations.OR, new List<TagModel>() },
                { BooleanOperations.NOT, new List<TagModel>() },
            };
        }

        public FilterModel(FilterEntity entity)
            : this()
        {
            Name = entity.Name;
            
            foreach (var entry in entity.FilterParameter)
            {
                FilterParameter[entry.Key].AddRange(entry.Value.Select(x => new TagModel(x)).ToList());
            }
        }

        public enum BooleanOperations
        {
            AND = 0,
            OR,
            NOT,
        };

        public string Name
        {
            get;
            set;
        }

        public Dictionary<BooleanOperations, List<TagModel>> FilterParameter
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
