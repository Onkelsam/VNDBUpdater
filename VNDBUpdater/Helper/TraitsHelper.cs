using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VNDBUpdater.Models;
using VNUpdater.Data;

namespace VNDBUpdater.Helper
{
    public class TraitsHelper
    {
        private static List<Trait> _LocalTraits;

        public static List<Trait> LocalTraits
        {
            private set { _LocalTraits = value; }
            get
            {
                if (_LocalTraits == null)
                {
                    var rawTraits = new List<TraitsLookUp>();

                    if (File.Exists(Constants.TraitsJsonFileName))
                        rawTraits = JsonConvert.DeserializeObject<List<TraitsLookUp>>(File.ReadAllText(Constants.TraitsJsonFileName));

                    _LocalTraits = new List<Trait>();

                    foreach (var trait in rawTraits)
                        _LocalTraits.Add(new Trait(trait));

                    foreach (var trait in _LocalTraits)
                        trait.SetParentTraits(rawTraits.Where(x => x.id == trait.ID).Select(x => x.parents).First().ToList());

                    return _LocalTraits;
                }

                return _LocalTraits;
            }
        }

        public static void ResetTraits()
        {
            _LocalTraits = null;
        }

        public class TraitsLookUp
        {
            public bool meta { get; set; }
            public int chars { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public List<object> parents { get; set; }
            public int id { get; set; }
            public List<object> aliases { get; set; }
        }
    }
}
