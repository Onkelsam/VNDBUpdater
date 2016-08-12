using Newtonsoft.Json;
using System.Collections.Generic;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;

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
