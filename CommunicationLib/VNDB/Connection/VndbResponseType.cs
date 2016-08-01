namespace CommunicationLib.VNDB.Connection
{
    /// <summary>
    /// Response enum from VNDB.
    /// </summary>
    public enum VndbResponseType
    {
        /// <summary>
        /// Response is valid.
        /// </summary>
        Ok,
        /// <summary>
        /// Error returned.
        /// </summary>
        Error,
        /// <summary>
        /// Unknown - Unknown state.
        /// </summary>
        Unknown,
    }
}
