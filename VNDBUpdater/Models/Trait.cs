using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Helper;
using VNUpdater.Data;

namespace VNDBUpdater.Models
{
    public class Trait
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Trait> ParentTraits { get; set; }

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
                        TraitList.Add(foundTrait);
                }
            }

            return TraitList;
        }
    }
}
