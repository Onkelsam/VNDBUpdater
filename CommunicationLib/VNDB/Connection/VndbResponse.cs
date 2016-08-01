namespace CommunicationLib.VNDB.Connection
{
    /// <summary>
    /// VndbResponse contains the responseType and json-payload from VNDb.
    /// </summary>
    public class VndbResponse
    {
        private VndbResponseType _ResponseType;
        private string _Payload;

        /// <summary>
        /// Ok - No Errors.
        /// Error - Error.
        /// Unknown - Unknown Error.
        /// </summary>
        public VndbResponseType ResponseType
        {
            get { return _ResponseType; }
        }

        /// <summary>
        /// JsonPayload returned from VNDB.
        /// </summary>
        public string Payload
        {
            get { return _Payload; }
        }

        internal VndbResponse(VndbResponseType responseType, string jsonPayload)
        {
            _ResponseType = responseType;
            _Payload = jsonPayload;
        }
    }
}
