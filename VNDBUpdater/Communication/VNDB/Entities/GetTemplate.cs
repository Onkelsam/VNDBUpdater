using System.Collections.Generic;

namespace VNDBUpdater.Communication.VNDB.Entities
{
    public class GetTemplate<T>
    {
        public int num { get; set; }
        public bool more { get; set; }

        public List<T> items { get; set; }
    }
}
