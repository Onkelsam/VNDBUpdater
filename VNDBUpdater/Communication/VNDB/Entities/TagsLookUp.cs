using System.Collections.Generic;

namespace VNDBUpdater.Communication.VNDB.Entities
{
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
