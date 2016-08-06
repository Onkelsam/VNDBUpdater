using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNUpdater.Data;

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
                client.DownloadFile(Constants.TagsDownloadLink, Constants.TagsZipFileName);
                FileHelper.Decompress(new FileInfo(Constants.TagsZipFileName));
            }

            FileHelper.DeleteFile(Constants.TagsZipFileName);
            TagHelper.ResetTags();
        }

        public static List<Tag> FindMatchingTagsForVisualNovel(VNInformation rawInfo)
        {
            var TagList = new List<Tag>();

            foreach (var tag in rawInfo.tags)
            {
                Tag foundTag = TagHelper.LocalTags.FirstOrDefault(x => x.ID == tag[0]);

                if (foundTag != null)
                {
                    foundTag.Score = tag[1];
                    TagList.Add(foundTag);
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
