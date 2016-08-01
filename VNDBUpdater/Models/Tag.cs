using System.IO;
using System.Net;
using VNDBUpdater.Data;

namespace VNDBUpdater.Models
{   
    public class Tag
    {
        public string Name { get; set; }
        public double Score { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }
        public TagCategory Category { get; set; }

        public static void RefreshTags()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("http://vndb.org/api/tags.json.gz", "tags.json.gz");
                Helper.FileHelper.Decompress(new FileInfo("tags.json.gz"));
            }

            File.Delete("tags.json.gz");
            Helper.TagHelper.ResetTags();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
