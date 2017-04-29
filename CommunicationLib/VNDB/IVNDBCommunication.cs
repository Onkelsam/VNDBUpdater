using CommunicationLib.VNDB.Connection;
using System;

namespace CommunicationLib.VNDB
{
    /// <summary>
    /// Interface for VNDB communication.
    /// </summary>
    public interface IVNDBCommunication : IDisposable
    {
        /// <summary>
        /// Shows connection status.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Set options used in 'Query' command.
        /// </summary>
        Options VNDBOptions { get; set; }

        /// <summary>
        /// Connect to VNDB API with given Username and Password (Password has to be encrypted).
        /// </summary>
        VndbResponse Connect(string username, string password);

        /// <summary>
        /// Disconnects from VNDB API
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Query VNDB API with given command.
        /// </summary>
        /// <returns>result as string</returns>
        VndbResponse Query(string command);

        /// <summary>
        /// Gets most of the information for given Visual Novel IDs.
        /// </summary>
        /// <param name="IDs">IDs as int. Max is 25.</param>
        /// <returns>result as string</returns>
        VndbResponse QueryInformation(int[] IDs);

        /// <summary>
        /// Gets most of the information for given Visual Novel ID.
        /// </summary>
        VndbResponse QueryInformation(int ID);

        /// <summary>
        /// Gets character information for given Visual Novels IDs.
        /// </summary>
        VndbResponse QueryCharacterInformation(int[] IDs, int page);

        /// <summary>
        /// Gets character information for given Visual Novel ID.
        /// </summary>
        VndbResponse QueryCharacterInformation(int ID);

        /// <summary>
        /// Gets vote list from VNDB for logged in user.
        /// </summary>
        VndbResponse QueryVoteList(int page = 1);

        /// <summary>
        /// Gets VN list from VNDB for logged in user.
        /// </summary>
        VndbResponse QueryVNList(int page = 1);

        /// <summary>
        /// Gets wish list from VNDB for logged in user.
        /// </summary>
        VndbResponse QueryWishList(int page = 1);

        /// <summary>
        /// Sets vote for given VN and logged in user.
        /// </summary>
        VndbResponse SetVote(int ID, SetJSONObjects.Vote vote);

        /// <summary>
        /// Deletes vote for given VN and logged in user.
        /// </summary>
        VndbResponse DeleteVote(int ID);

        /// <summary>
        /// Sets VN state for given VN and logged in user.
        /// </summary>
        VndbResponse SetVNList(int ID, SetJSONObjects.State state);

        /// <summary>
        /// Deletes VN for given VN and logged in user.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        VndbResponse DeleteVNFromVNList(int ID);

        /// <summary>
        /// Sets wishlist for given VN and logged in user.
        /// </summary>
        VndbResponse SetWishList(int ID, SetJSONObjects.Priority priority);

        /// <summary>
        /// Deletes wish for given VN and logged in user.
        /// </summary>
        VndbResponse DeleteVNFromWishList(int ID);

        VndbResponse SearchByTitle(string title, int page);
    }
}
