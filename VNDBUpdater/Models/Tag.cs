using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;

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

        public static List<Tag> FindMatchingTagsForCategory(VisualNovel VN, TagCategory Category)
        {
            var TagList = new List<Tag>();

            foreach (var tag in VN.Basics.tags)
            {
                Tag foundTag = TagHelper.LocalTags.FirstOrDefault(x => x.ID == tag[0]);

                if (foundTag != null)
                {
                    if (foundTag.Category == Category || Category == TagCategory.All)
                    {
                        foundTag.Score = tag[1];
                        TagList.Add(foundTag);
                    }
                }
            }

            return TagList;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
