using CodeKicker.BBCode;
using Newtonsoft.Json;
using System.Collections.Generic;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Helper;

namespace VNDBUpdater.Models.Internal
{
    public class CharacterInformation
    {
        public VNCharacterInformation VNDBInformation { get; set; }

        [JsonIgnore]
        public List<Trait> ConvertedTraits { get; private set; }

        [JsonIgnore]
        public string Description
        {
            get { return VNDBInformation.description != null ? BBCode.ToHtml(VNDBInformation.description.ToString()) : string.Empty; }
        }

        [JsonIgnore]
        public Dictionary<string, string> ConvertedTraitsSpoilerFree
        {
            get
            {
                var TraitsWithParent = new Dictionary<string, string>();

                foreach (var trait in ConvertedTraits)
                {
                    if (trait.ShowTrait())
                    {
                        var parenttrait = trait.LastParentTrait(trait);

                        if (TraitsWithParent.ContainsKey(parenttrait.Name + ":"))
                            TraitsWithParent[parenttrait.Name + ":"] += ", " + (trait.Name);
                        else
                            TraitsWithParent.Add(parenttrait.Name + ":", trait.Name);                                                   
                    }
                }

                return TraitsWithParent;
            }
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
