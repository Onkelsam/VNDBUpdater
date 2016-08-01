namespace CommunicationLib.VNDB
{
    /// <summary>
    /// This class contains the various options that can be used in a query.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// If VNDB responded with 'more' set to true.
        /// Increase this and query again so you will get the next 25 results.
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// Number of results per page.
        /// Max for normal queries is 25.
        /// Max for 'getwishlist' 'getvotelist' and 'getvnlist' is 100.
        /// </summary>
        public int results { get; set; }
        /// <summary>
        /// String used to sort the results.
        /// </summary>
        public string sort { get; set; }
        /// <summary>
        /// Reverses the sort method.
        /// </summary>
        public bool reverse { get; set; }

        /// <summary>
        /// Default constructor.
        /// Sets page to 1.
        /// Sets results to 25.
        /// Sets sort to 'id'.
        /// Sets reverse to false.
        /// </summary>
        public Options()
        {
            page = 1;
            results = 25;
            sort = "id";
            reverse = false;
        }
    }
}
