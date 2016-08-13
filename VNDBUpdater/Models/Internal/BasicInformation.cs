using CodeKicker.BBCode;
using Newtonsoft.Json;
using System.Collections.Generic;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;

namespace VNDBUpdater.Models.Internal
{
    public class BasicInformation
    {
        public VNInformation VNDBInformation { get; set; }

        [JsonIgnore]
        public List<Tag> ConvertedTags { get; private set; }
        
        public string Length
        {
            get { return VNDBInformation.length == null ? Constants.VNLengthMapper[0] : Constants.VNLengthMapper[VNDBInformation.length]; }
        }

        public string Description
        {
            get { return VNDBInformation.description != null ? BBCode.ToHtml(VNDBInformation.description) : string.Empty; }
        }

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

        public BasicInformation()
        {
            VNDBInformation = new VNInformation();            
        }

        public BasicInformation(VNInformation basics)
        {
            VNDBInformation = basics;
            ConvertedTags = Tag.FindMatchingTagsForVisualNovel(basics);
        }
    }
}
