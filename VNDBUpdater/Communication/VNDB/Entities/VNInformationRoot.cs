using System.Collections.Generic;

namespace VNDBUpdater.Communication.VNDB.Entities
{
    public class VNInformationRoot
    {
        public List<VNInformation> items { get; set; }
        public int num { get; set; }
        public bool more { get; set; }
    }
}
