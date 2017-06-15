using CommunicationLib.VNDB;
using CommunicationLib.VNDB.Connection;
using System;
using System.Threading.Tasks;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDB : IDisposable
    {
        Task ConnectAsync();
        Task ReconnectAsync();

        bool LoggedIn { get; }
        bool Error { get; }
        bool Throttled { get; }
        string Status { get; }

        ErrorResponse HandleError(VndbResponse response);

        Task<VndbResponse> SetVNListAsync(int ID, SetJSONObjects.State state);
        Task<VndbResponse> SetVoteAsync(int ID, SetJSONObjects.Vote vote);
        Task<VndbResponse> DeleteVoteAsync(int ID);
        Task<VndbResponse> DeleteVNFromVNListAsync(int ID);
        Task<VndbResponse> QueryCharacterInformationAsync(int[] IDs, int page);
        Task<VndbResponse> QueryCharacterInformationAsync(int ID);
        Task<VndbResponse> QueryInformationAsync(int ID);
        Task<VndbResponse> SearchByTitleAsync(string title, int page);
        Task<VndbResponse> QueryInformationAsync(int[] IDs);
        Task<VndbResponse> QueryVNListAsync(int page = 1);
        Task<VndbResponse> QueryVoteListAsync(int page = 1);
    }
}
