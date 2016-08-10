using Newtonsoft.Json;
using System.Collections.Generic;
using VNDBUpdater.Communication.VNDB;

namespace VNDBUpdater.Models.Internal
{
    public class CharacterInformation
    {
        public VNCharacterInformation VNDBInformation { get; set; }

        [JsonIgnore]
        public List<Trait> ConvertedTraits { get; set; }

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
