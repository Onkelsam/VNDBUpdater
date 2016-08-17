using CodeKicker.BBCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.ValueConverters;

namespace VNDBUpdater.Models.Internal
{
    public class BasicInformation
    {
        public VNInformation VNDBInformation { get; set; }

        [JsonIgnore]
        public List<Tag> ConvertedTags { get; private set; }
        
        [JsonIgnore]
        public string Length
        {
            get { return VNDBInformation.length == null ? Constants.VNLengthMapper[0] : Constants.VNLengthMapper[VNDBInformation.length]; }
        }

        [JsonIgnore]
        public string Description
        {
            get { return VNDBInformation.description != null ? BBCode.ToHtml(VNDBInformation.description) : string.Empty; }
        }

        [JsonIgnore]
        public List<VNScreenshot> Screenshots
        {
            get { return UserHelper.CurrentUser.Settings.ShowNSFWImages == true ? VNDBInformation.screens : VNDBInformation.screens.Where(x => x.nsfw == false).Select(x => x).ToList(); }
        }

        [JsonIgnore]
        public string MainThumb
        {
            get
            {
                if (!UserHelper.CurrentUser.Settings.ShowNSFWImages && VNDBInformation.image_nsfw)
                    return Constants.DirectoryPath + Constants.NSFWImage;
                else
                    return VNDBInformation.image;
            }
        }

        [JsonIgnore]
        public Dictionary<string, string> DetailedInformation
        {
            get
            {
                var DetailedInfo = new Dictionary<string, string>()
                {
                    { "Aliases: \t", VNDBInformation.aliases },
                    { "Length: \t", Length },
                    { "Release: \t", VNDBInformation.released },
                    { "Relations: \t", ConvertRelations(VNDBInformation.relations) },
                    { "Playtime: \t", ConvertTimeSpan(LocalVisualNovelHelper.GetVisualNovel(VNDBInformation.id).Playtime) },
                };

                return DetailedInfo;
            }
        }

        public BasicInformation()
        {
            VNDBInformation = new VNInformation();            
        }

        public BasicInformation(VNInformation basics)
        {
            VNDBInformation = basics;
            ConvertedTags = Tag.FindMatchingTagsForVisualNovel(basics);
        }

        private string ConvertTimeSpan(TimeSpan ts)
        {
            if (ts == null)
                return string.Empty;
            else
                return string.Format("{0} day{1}, {2} hour{3}, {4} minute{5}",
                                      ts.Days,
                                      ts.Days == 1 ? "" : "s",
                                      ts.Hours,
                                      ts.Hours == 1 ? "" : "s",
                                      ts.Minutes,
                                      ts.Minutes == 1 ? "" : "s");
        }

        private string ConvertRelations(List<Relation> relations)
        {
            if (relations != null)
            {
                var sb = new StringBuilder();

                foreach (var relation in Constants.RelationsMapper)
                {
                    if (relations.Any(x => x.relation == relation.Key))
                    {
                        sb.Append(relation.Value + ":");

                        foreach (var rel in relations.Where(x => x.relation == relation.Key))
                            sb.Append(Environment.NewLine + "\t" + rel.title);

                        sb.AppendLine();
                    }
                }

                return sb.ToString();
            }
            else
                return string.Empty;
        }
    }
}
