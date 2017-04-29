using System.Collections.Generic;

namespace VNDBUpdater.Communication.VNDB.Entities
{
    public class VNInformation
    {
        public List<string> platforms { get; set; }
        public int votecount { get; set; }
        public string released { get; set; }
        public List<string> orig_lang { get; set; }
        public int? length { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public double rating { get; set; }
        public string description { get; set; }
        public VNLinks links { get; set; }
        public List<List<double>> tags { get; set; }
        public List<string> languages { get; set; }
        public int id { get; set; }
        public string aliases { get; set; }
        public double popularity { get; set; }
        public object original { get; set; }
        public List<VNScreenshot> screens { get; set; }
        public bool image_nsfw { get; set; }
        public List<Relation> relations { get; set; }
    }
}
