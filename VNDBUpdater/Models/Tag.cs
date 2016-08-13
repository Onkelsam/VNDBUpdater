using CodeKicker.BBCode;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;

namespace VNDBUpdater.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }
        public TagCategory Category { get; set; }
        public double Score { get; private set; }
        public SpoilerLevel Spoiler { get; private set; }

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
                var foundTag = TagHelper.LocalTags.FirstOrDefault(x => x.ID == tag[0]);               

                if (foundTag != null)
                {                    
                    foundTag.Score = tag[1];
                    foundTag.Spoiler = ExtensionMethods.ParseEnum<SpoilerLevel>(tag[2].ToString());

                    var newTag = new Tag()
                    {
                        Category = foundTag.Category,
                        Description = BBCode.ToHtml(foundTag.Description),
                        ID = foundTag.ID,
                        Name = foundTag.Name,
                        Score = foundTag.Score,
                        Spoiler = foundTag.Spoiler
                    };

                    TagList.Add(newTag);
                }
            }

            return TagList;
        }

        public bool ShowTag()
        {
            return (int)UserHelper.CurrentUser.Settings.SpoilerSetting >= (int)Spoiler;
        }

        public static List<Tag> GetTagsForVN(VisualNovel vn, TagCategory currentCategory)
        {
            var tags = new List<Tag>();

            if (vn != null)
            {
                if (vn.Basics != null)
                {
                    if (vn.Basics.ConvertedTags != null)
                    {
                        foreach (var tag in vn.Basics.ConvertedTags)
                            if (tag.Category == currentCategory || currentCategory == TagCategory.All)
                                if (tag.ShowTag())
                                    tags.Add(tag);
                    }
                }                
            }
            
            return tags;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
