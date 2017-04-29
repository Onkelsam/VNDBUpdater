namespace VNDBUpdater.Communication.VNDB.Entities
{
    public class Error
    {
        public double minwait { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string msg { get; set; }
        public double fullwait { get; set; }
    }
}
