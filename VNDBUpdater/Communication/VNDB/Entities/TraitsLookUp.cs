using System.Collections.Generic;

namespace VNDBUpdater.Communication.VNDB.Entities
{
    public class TraitsLookUp
    {
        public bool meta { get; set; }
        public int chars { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public List<object> parents { get; set; }
        public int id { get; set; }
        public List<object> aliases { get; set; }
    }
}
