using CommunicationLib.Redis;
using CommunicationLib.VNDB;

namespace CommunicationLib
{
    /// <summary>
    /// Handles creation of communication classes.
    /// </summary>
    public class Communication
    {
        /// <summary>
        /// Creates new object for Redis Communication.
        /// </summary>
        public IRedisCommunication GetRedisCommunication()
        {
            return new RedisCommunication();
        }

        /// <summary>
        /// Create new object for VNDB Communication.
        /// </summary>
        /// <returns></returns>
        public IVNDBCommunication GetVNDBCommunication()
        {
            return new VNDBCommunication();
        }
    }
}
