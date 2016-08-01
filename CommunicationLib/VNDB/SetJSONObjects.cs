namespace CommunicationLib.VNDB
{
    /// <summary>
    /// JsonObjects needed for 'SET'-Command.
    /// </summary>
    public class SetJSONObjects
    {
        /// <summary>
        /// Containts state of VN.
        /// </summary>
        public class State
        {
            /// <summary>
            /// State of VN.
            /// 0 = unknown.
            /// 1 = playing.
            /// 2 = finished.
            /// 3 = stalled.
            /// 4 = dropped.
            /// </summary>
            public int status { get; set; }
        }

        /// <summary>
        /// Containts vote of VN.
        /// </summary>
        public class Vote
        {
            /// <summary>
            /// Number between 10 and 100.
            /// </summary>
            public int vote { get; set; }
        }

        /// <summary>
        /// Containts priority of VN on wishlist.
        /// </summary>
        public class Priority
        {
            /// <summary>
            /// Priority of wish.
            /// 0 = high.
            /// 1 = medium.
            /// 2 = low.
            /// 3 = blacklist.
            /// </summary>
            public int priority { get; set; }
        }
    }
}
