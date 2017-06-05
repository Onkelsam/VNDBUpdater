using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Data;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.Traits
{
    public class TraitService : TagsAndTraitsBase, ITraitService
    {
        private List<TraitModel> _Traits;

        private const string _TraitsDumpFileName = "traits.json";
        private const string _TraitsZipFileName = "traits.json.gz";
        private const string _TraitsDownloadLink = "http://vndb.org/api/traits.json.gz";

        public TraitService()
        {
            _Traits = new List<TraitModel>();

            GetTraits();
        }

        public IList<TraitModel> Get()
        {
            return _Traits;
        }

        public async Task Refresh()
        {
            await base.RefreshAsync(_TraitsDownloadLink, _TraitsZipFileName);

            _Traits = new List<TraitModel>();

            GetTraits();
        }

        public bool Show(SpoilerSetting UserSetting, SpoilerLevel TSpoiler)
        {
            return (int)UserSetting >= (int)TSpoiler;
        }

        public TraitModel GetLastParentTrait(TraitModel trait)
        {
            if (trait.ParentTraits.Any())
            {
                return GetLastParentTrait(trait.ParentTraits.Last());
            }
            else
            {
                return trait;
            }
        }

        private void GetTraits()
        {
            var rawTraits = new List<TraitsLookUp>();

            if (File.Exists(_TraitsDumpFileName))
            {
                rawTraits = JsonConvert.DeserializeObject<List<TraitsLookUp>>(File.ReadAllText(_TraitsDumpFileName));

                foreach (var trait in rawTraits)
                {
                    _Traits.Add(new TraitModel(trait));
                }

                foreach (var trait in _Traits)
                {
                    foreach (var parent in rawTraits.Where(x => x.id == trait.ID).Select(x => x.parents).First().ToList())
                    {
                        trait.ParentTraits.AddRange(_Traits.Where(x => x.ID.ToString() == parent.ToString()));
                    }
                }
            }
        }
    }
}
