using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;

namespace VNDBUpdater.Models
{
    public class Trait
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<Trait> ParentTraits { get; private set; }
        public SpoilerLevel Spoiler { get; private set; }

        public Trait()
        {
            ParentTraits = new List<Trait>();
        }

        public Trait(TraitsHelper.TraitsLookUp RawData)
            : this()
        {
            ID = RawData.id;
            Name = RawData.name;
            Description = RawData.description;
        }

        public Trait LastParentTrait(Trait trait)
        {
            if (trait.ParentTraits.Any())
                return LastParentTrait(trait.ParentTraits.Last());
            else
                return trait;
        }

        public bool ShowTrait()
        {
            return (int)UserHelper.CurrentUser.Settings.SpoilerSetting >= (int)Spoiler;
        }

        public void SetParentTraits(List<object> Parents)
        {
            foreach (var parent in Parents)
                ParentTraits.AddRange(TraitsHelper.LocalTraits.Where(x => x.ID.ToString() == parent.ToString()));
        }

        public static void RefreshTraits()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(Constants.TraitsDownloadLink, Constants.TraitsZipFileName);
                FileHelper.Decompress(new FileInfo(Constants.TraitsZipFileName));
            }

            FileHelper.DeleteFile(Constants.TraitsZipFileName);
            TraitsHelper.ResetTraits();
        }

        public static List<Trait> FindMatchingTraitsForCharacter(VNCharacterInformation character)
        {
            var TraitList = new List<Trait>();

            if (character.traits != null)
            {
                foreach (var trait in character.traits)
                {
                    Trait foundTrait = TraitsHelper.LocalTraits.FirstOrDefault(x => x.ID == trait[0]);

                    if (foundTrait != null)
                    {
                        foundTrait.Spoiler = ExtensionMethods.ParseEnum<SpoilerLevel>(trait[1].ToString());

                        var newTrait = new Trait()
                        {
                            Description = foundTrait.Description,
                            ID = foundTrait.ID,
                            Name = foundTrait.Name,
                            ParentTraits = foundTrait.ParentTraits,
                            Spoiler = foundTrait.Spoiler
                        };

                        TraitList.Add(newTrait);
                    }
                }                
            }

            return TraitList;
        }
    }
}
