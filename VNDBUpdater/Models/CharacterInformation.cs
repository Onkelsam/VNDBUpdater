using System.Collections.Generic;
using VNDBUpdater.Communication.VNDB;

namespace VNDBUpdater.Models
{
    public class CharacterInformation : VNCharacterInformation
    {
        public List<Trait> ConvertedTraits { get; set; }

        public CharacterInformation(VNCharacterInformation charinfo)
        {
            if (charinfo != null)
            {
                aliases = charinfo.aliases;
                birthday = charinfo.birthday;
                bloodt = charinfo.bloodt;
                description = charinfo.description;
                gender = charinfo.gender;
                id = charinfo.id;
                image = charinfo.image;
                name = charinfo.name;
                original = charinfo.original;
                traits = charinfo.traits;
                vns = charinfo.vns;
                ConvertedTraits = Trait.FindMatchingTraitsForCharacter(charinfo);
            }
        }
    }
}
