using CommunicationLib.VNDB;
using CommunicationLib.VNDB.Connection;
using System;
using System.Threading.Tasks;
using VNDBUpdater.Data;

namespace VNDBUpdater.Communication.VNDB.Interfaces
{
    public interface IVNDB : IDisposable
    {
        void Connect();
        void Reconnect();

        bool LoggedIn { get; }
        bool Error { get; }
        bool Throttled { get; }
        string Status { get; }

        ErrorResponse HandleError(VndbResponse response);

        VndbResponse SetVNList(int ID, SetJSONObjects.State state);
        VndbResponse SetVote(int ID, SetJSONObjects.Vote vote);
        VndbResponse DeleteVote(int ID);
        VndbResponse DeleteVNFromVNList(int ID);
        VndbResponse QueryCharacterInformation(int[] IDs, int page);
        VndbResponse QueryCharacterInformation(int ID);
        VndbResponse QueryInformation(int ID);
        VndbResponse SearchByTitle(string title, int page);
        VndbResponse QueryInformation(int[] IDs);
        VndbResponse QueryVNList(int page = 1);
        VndbResponse QueryVoteList(int page = 1);
    }
}
