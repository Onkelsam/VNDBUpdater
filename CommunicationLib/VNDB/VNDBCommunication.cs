using CommunicationLib.VNDB.Connection;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CommunicationLib.VNDB
{
    internal class VNDBCommunication : IVNDBCommunication
    {
        private VndbConnection Client;
        private Options _VNDBOptions;

        private bool _IsConnected;

        public bool IsConnected
        {
            get { return Client == null ? false : Client.IsConnected; }
        }

        public Options VNDBOptions
        {
            get { return _VNDBOptions; }
            set { _VNDBOptions = value; }
        }

        public async Task<VndbResponse> Connect(string username, string password)
        {            
            InputSanitization.CheckStringArgument(username);
            InputSanitization.CheckStringArgument(password);

            if (IsConnected)
            {
                Disconnect();
            }

            _VNDBOptions = new Options();

            Client = new VndbConnection();

            await Client.Connect();

            var response = await Client.Login(username, password);

            if (response.ResponseType != VndbResponseType.Error)
                _IsConnected = true;
            else
                _IsConnected = false;            

            return response;
        }

        public void Disconnect()
        {
            if (_IsConnected)
            {
                Client.Disconnect();
                _IsConnected = false;
            }                
        }

        public async Task<VndbResponse> Query(string command)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(command);

            return await Client.Query(command + " " + JsonConvert.SerializeObject(_VNDBOptions));
        }

        public async Task<VndbResponse> Set(string command)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(command);

            return await Client.Query(command);
        }

        public async Task<VndbResponse> SearchByTitle(string title, int page)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(title);

            SetOptions(page, 25, "id");

            return await Query("get vn basic,details,stats,relations,tags,screens (search~" + JsonConvert.SerializeObject(title) + ")");
        }

        public async Task<VndbResponse> QueryInformation(int[] IDs)
        {
            CheckConnection();

            InputSanitization.CheckIDArray(IDs);

            SetOptionsToDefault();

            return await Query("get vn basic,details,stats,relations,tags,screens (id = [" + string.Join(",", IDs) + "] )");
        }

        public async Task<VndbResponse> QueryCharacterInformation(int[] IDs, int page)
        {
            CheckConnection();

            InputSanitization.CheckIDArray(IDs);
            InputSanitization.CheckInt(page);

            SetOptions(page, 25, "id");

            return await Query("get character basic,details,traits,vns (vn = [" + string.Join(",", IDs) + "] )");
        }

        public async Task<VndbResponse> QueryInformation(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            SetOptionsToDefault();

            return await Query("get vn basic,details,stats,relations,tags,screens (id = " + ID + ")");
        }

        public async Task<VndbResponse> QueryCharacterInformation(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            SetOptionsToDefault();

            return await Query("get character basic,details,traits,vns (vn = " + ID + ")");
        }

        public async Task<VndbResponse> QueryVoteList(int page = 1)
        {
            CheckConnection();

            InputSanitization.CheckInt(page);

            SetOptions(page, 100, "vn");

            return await Query("get votelist basic (uid = 0)");
        }

        public async Task<VndbResponse> QueryVNList(int page = 1)
        {
            CheckConnection();

            InputSanitization.CheckInt(page);

            SetOptions(page, 100, "vn");

            return await Query("get vnlist basic (uid = 0)");
        }

        public async Task<VndbResponse> QueryWishList(int page = 1)
        {
            CheckConnection();

            InputSanitization.CheckInt(page);

            SetOptions(page, 100, "vn");

            return await Query("get wishlist basic (uid = 0)");
        }

        public async Task<VndbResponse> SetVote(int ID, SetJSONObjects.Vote vote)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            InputSanitization.CheckRange(vote.vote, 10, 100);

            return await Set("set votelist " + ID + " " + JsonConvert.SerializeObject(vote));
        }

        public async Task<VndbResponse> DeleteVote(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            return await Set("set votelist " + ID);
        }

        public async Task<VndbResponse> SetVNList(int ID, SetJSONObjects.State state)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            InputSanitization.CheckRange(state.status, 0, 4);

            return await Set("set vnlist " + ID + " " + JsonConvert.SerializeObject(state));
        }

        public async Task<VndbResponse> DeleteVNFromVNList(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            return await Set("set vnlist " + ID);
        }

        public async Task<VndbResponse> SetWishList(int ID, SetJSONObjects.Priority priority)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            InputSanitization.CheckRange(priority.priority, 0, 3);

            return await Set("set wishlist " + ID + " " + JsonConvert.SerializeObject(priority));
        }

        public async Task<VndbResponse> DeleteVNFromWishList(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            return await Set("set wishlist " + ID);
        }

        private void CheckConnection()
        {
            if (!_IsConnected)
                throw new ConnectionException("No connection to Redis DB! Be sure you called the connect method!");
        }

        private void SetOptionsToDefault()
        {
            _VNDBOptions = new Options();
        }

        private void SetOptions(int page, int results, string sort)
        {
            _VNDBOptions.page = page;
            _VNDBOptions.results = results;
            _VNDBOptions.sort = sort;
        }

        public void Dispose()
        {
            if (Client != null)
            {
                Client.Dispose();
                _IsConnected = false;
            }
        }
    }
}
