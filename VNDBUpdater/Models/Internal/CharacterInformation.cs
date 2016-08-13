using CodeKicker.BBCode;
using Newtonsoft.Json;
using System.Collections.Generic;
using VNDBUpdater.Communication.VNDB;

namespace VNDBUpdater.Models.Internal
{
    public class CharacterInformation
    {
        public VNCharacterInformation VNDBInformation { get; set; }

        [JsonIgnore]
        public List<Trait> ConvertedTraits { get; private set; }

        public string Description
        {
            get { return VNDBInformation.description != null ? BBCode.ToHtml(VNDBInformation.description.ToString()) : string.Empty; }
        }

        public CharacterInformation()
        {
            VNDBInformation = new VNCharacterInformation();
        }

        public CharacterInformation(VNCharacterInformation charinfo)
        {
            VNDBInformation = charinfo;
            ConvertedTraits = Trait.FindMatchingTraitsForCharacter(charinfo);
        }
    }
}
