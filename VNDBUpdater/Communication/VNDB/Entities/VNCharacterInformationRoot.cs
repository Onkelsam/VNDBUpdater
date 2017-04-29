using System.Collections.Generic;

namespace VNDBUpdater.Communication.VNDB.Entities
{
    public class VNCharacterInformationRoot
    {
        public List<VNCharacterInformation> items { get; set; }
        public bool more { get; set; }
        public int num { get; set; }
    }
}
