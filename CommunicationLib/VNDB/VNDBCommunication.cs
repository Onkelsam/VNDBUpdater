using CommunicationLib.VNDB.Connection;
using Newtonsoft.Json;

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

        public VndbResponse Connect(string username, string password)
        {
            if (IsConnected)
                Disconnect();

            InputSanitization.CheckStringArgument(username);
            InputSanitization.CheckStringArgument(password);

            _VNDBOptions = new Options();

            Client = new VndbConnection();

            var response =  Client.Login(username, password);

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

        public VndbResponse Query(string command)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(command);

            return Client.Query(command + " " + JsonConvert.SerializeObject(_VNDBOptions));
        }

        public VndbResponse Set(string command)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(command);

            return Client.Query(command);
        }

        public VndbResponse SearchByTitle(string title, int page)
        {
            CheckConnection();

            InputSanitization.CheckStringArgument(title);

            SetOptions(page, 25, "id");

            return Query("get vn basic,details,stats,relations,tags,screens (search~" + JsonConvert.SerializeObject(title) + ")");
        }

        public VndbResponse QueryInformation(int[] IDs)
        {
            CheckConnection();

            InputSanitization.CheckIDArray(IDs);

            SetOptionsToDefault();

            return Query("get vn basic,details,stats,relations,tags,screens (id = [" + string.Join(",", IDs) + "] )");
        }

        public VndbResponse QueryCharacterInformation(int[] IDs, int page)
        {
            CheckConnection();

            InputSanitization.CheckIDArray(IDs);
            InputSanitization.CheckInt(page);

            SetOptions(page, 25, "id");

            return Query("get character basic,details,traits,vns (vn = [" + string.Join(",", IDs) + "] )");
        }

        public VndbResponse QueryInformation(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            SetOptionsToDefault();

            return Query("get vn basic,details,stats,relations,tags,screens (id = " + ID + ")");
        }

        public VndbResponse QueryCharacterInformation(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            SetOptionsToDefault();

            return Query("get character basic,details,traits,vns (vn = " + ID + ")");
        }

        public VndbResponse QueryVoteList(int page = 1)
        {
            CheckConnection();

            InputSanitization.CheckInt(page);

            SetOptions(page, 100, "vn");

            return Query("get votelist basic (uid = 0)");
        }

        public VndbResponse QueryVNList(int page = 1)
        {
            CheckConnection();

            InputSanitization.CheckInt(page);

            SetOptions(page, 100, "vn");

            return Query("get vnlist basic (uid = 0)");
        }

        public VndbResponse QueryWishList(int page = 1)
        {
            CheckConnection();

            InputSanitization.CheckInt(page);

            SetOptions(page, 100, "vn");

            return Query("get wishlist basic (uid = 0)");
        }

        public VndbResponse SetVote(int ID, SetJSONObjects.Vote vote)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            InputSanitization.CheckRange(vote.vote, 10, 100);

            return Set("set votelist " + ID + " " + JsonConvert.SerializeObject(vote));
        }

        public VndbResponse DeleteVote(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            return Set("set votelist " + ID);
        }

        public VndbResponse SetVNList(int ID, SetJSONObjects.State state)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            InputSanitization.CheckRange(state.status, 0, 4);

            return Set("set vnlist " + ID + " " + JsonConvert.SerializeObject(state));
        }

        public VndbResponse DeleteVNFromVNList(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            return Set("set vnlist " + ID);
        }

        public VndbResponse SetWishList(int ID, SetJSONObjects.Priority priority)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            InputSanitization.CheckRange(priority.priority, 0, 3);

            return Set("set wishlist " + ID + " " + JsonConvert.SerializeObject(priority));
        }

        public VndbResponse DeleteVNFromWishList(int ID)
        {
            CheckConnection();

            InputSanitization.CheckInt(ID);

            return Set("set wishlist " + ID);
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
