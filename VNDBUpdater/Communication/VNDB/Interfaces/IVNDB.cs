using CommunicationLib.VNDB;
using CommunicationLib.VNDB.Connection;
using System;
using System.Threading.Tasks;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDB : IDisposable
    {
        Task Connect();
        Task Reconnect();

        bool LoggedIn { get; }
        bool Error { get; }
        bool Throttled { get; }
        string Status { get; }

        ErrorResponse HandleError(VndbResponse response);

        Task<VndbResponse> SetVNList(int ID, SetJSONObjects.State state);
        Task<VndbResponse> SetVote(int ID, SetJSONObjects.Vote vote);
        Task<VndbResponse> DeleteVote(int ID);
        Task<VndbResponse> DeleteVNFromVNList(int ID);
        Task<VndbResponse> QueryCharacterInformation(int[] IDs, int page);
        Task<VndbResponse> QueryCharacterInformation(int ID);
        Task<VndbResponse> QueryInformation(int ID);
        Task<VndbResponse> SearchByTitle(string title, int page);
        Task<VndbResponse> QueryInformation(int[] IDs);
        Task<VndbResponse> QueryVNList(int page = 1);
        Task<VndbResponse> QueryVoteList(int page = 1);
    }
}
