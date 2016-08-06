using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using VNDBUpdater.Data;
using VNDBUpdater.Models;
using VNUpdater.Data;

namespace VNDBUpdater.Helper
{
    public static class TagHelper
    {
        private static List<Tag> _LocalTags;

        public static List<Tag> LocalTags
        {
            private set { _LocalTags = value; }
            get
            {
                if (_LocalTags == null)
                {
                    var rawTags = new List<TagsLookUp>();

                    if (File.Exists(Constants.TagsJsonFileName))
                        rawTags = JsonConvert.DeserializeObject<List<TagsLookUp>>(File.ReadAllText(Constants.TagsJsonFileName));

                    _LocalTags = new List<Tag>();

                    foreach (var tag in rawTags)                    
                        _LocalTags.Add(new Tag { Name = tag.name, Description = tag.description, Category = ExtensionMethods.ParseEnum<TagCategory>(tag.cat), ID = tag.id });

                    return _LocalTags;
                }                              

                return _LocalTags;
            }
        }

        public static void ResetTags()
        {
            _LocalTags = null;
        }

        public class TagsLookUp
        {
            public List<object> aliases { get; set; }
            public double vns { get; set; }
            public int id { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public string cat { get; set; }
            public List<object> parents { get; set; }
            public bool meta { get; set; }
        }
    }
}
