using System.Collections.Generic;

namespace VNDBUpdater.Communication.VNDB.Entities
{
    public class VNCharacterInformation
    {
        public string name { get; set; }
        public object description { get; set; }
        public List<object> birthday { get; set; }
        public string original { get; set; }
        public List<List<int>> traits { get; set; }
        public List<List<object>> vns { get; set; }
        public int id { get; set; }
        public string gender { get; set; }
        public string image { get; set; }
        public object aliases { get; set; }
        public object bloodt { get; set; }
    }
}
