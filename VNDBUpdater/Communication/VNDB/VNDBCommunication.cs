using CommunicationLib.VNDB;
using CommunicationLib.VNDB.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNDBUpdater.Models.Internal;

namespace VNDBUpdater.Communication.VNDB
{
    public static class VNDBCommunication
    {
        private static IVNDBCommunication Connection;
        private static VNDBCommunicationStatus _Status = VNDBCommunicationStatus.NotLoggedIn;

        private static string _ErrorMessage;
        private static int _ConnectionTries = 0;

        public static VNDBCommunicationStatus Status
        {
            get { return _Status; }
        }

        public static string StatusString
        {
            get
            {
                switch (_Status)
                {
                    case (VNDBCommunicationStatus.LoggedIn):
                        return Constants.LoggedIn + UserHelper.CurrentUser.Username + "'.";
                    case (VNDBCommunicationStatus.NotLoggedIn):
                        return Constants.NotLoggedIn;
                    case (VNDBCommunicationStatus.Throttled):
                        return Constants.LoggedIn + UserHelper.CurrentUser.Username + "'.Throttled by VNDB.";
                    default:
                        return ErrorMessage;
                }
            }
        }

        public static string ErrorMessage
        {
            get { return _ErrorMessage; }
        }

        public static void Connect()
        {
            if (_Status != VNDBCommunicationStatus.LoggedIn || !Connection.IsConnected)
            {                
                try
                {
                    Connection = new CommunicationLib.Communication().GetVNDBCommunication();
                    var response = Connection.Connect(UserHelper.CurrentUser.Username, Convert.ToBase64String(UserHelper.CurrentUser.EncryptedPassword));

                    if (response.ResponseType == VndbResponseType.Error)
                    {
                        ErrorResponse result = HandleError(response);

                        if (result == ErrorResponse.Throttled)
                        {
                            EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Connect), Constants.VNDBConnectionFailedThrottling);                            
                            Connect(); // If the connection was throttled try again (after waiting 'minwait').
                        }                                                    
                        else if (result == ErrorResponse.AuthenticationFailed)
                        {
                            EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Connect), Constants.VNDBConnectionFailedAuthentication);                           
                            _Status = VNDBCommunicationStatus.Error;
                        }            
                        else
                        {
                            EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Connect), Constants.VNDBConnectionFailedUnknownError);
                            _Status = VNDBCommunicationStatus.Error;
                        }                
                    }
                    else
                    {
                        EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Connect), Constants.ConnectionEstablished);
                        _Status = VNDBCommunicationStatus.LoggedIn;
                        _ConnectionTries = 0;
                    }
                        
                }
                catch (IOException ex)
                {
                    // IOExceptins occurs if VNDB closed the stream.
                    // Just try to connect again using recursion.
                    EventLogger.LogError(nameof(VNDBCommunication) + ":" + nameof(Connect), ex);                    
                    _Status = VNDBCommunicationStatus.NotLoggedIn;
                    _ConnectionTries++;

                    if (_ConnectionTries != Constants.MaxConnectionTries)
                    {
                        EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Connect), Constants.ConnectionErrorHandled + _ConnectionTries.ToString());                        
                        Connect();
                    }                        
                    else
                    {
                        _ConnectionTries = 0;
                        EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Connect), Constants.ConnectionErrorNotHandled + "(" + Constants.MaxConnectionTries + ")");                        
                        _Status = VNDBCommunicationStatus.Error;
                        _ErrorMessage = "Login failed. Error message: " + ex.Message + ex.GetType().ToString() + ex.GetBaseException().GetType().ToString();
                    }
                }
                catch (Exception ex)
                {
                    EventLogger.LogError(nameof(VNDBCommunication) + ":" + nameof(Connect), ex);
                    EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Connect), Constants.ConnectionErrorNotHandled);                    
                    _Status = VNDBCommunicationStatus.Error;
                    _ErrorMessage = "Login failed. Error message: " + ex.Message + ex.GetType().ToString() + ex.GetBaseException().GetType().ToString();
                }
            }
        }

        public static void Reconnect()
        {
            Disconnect();
            Connect();
        }

        public static List<VisualNovel> FetchVisualNovels(List<int> IDs)
        {
            CheckConnection();

            var visualNovels = new List<VisualNovel>();

            var idSplitter = new VNIDsSplitter(IDs.ToArray());

            idSplitter.Split();

            if (idSplitter.SplittingNeccessary)
            {
                for (int round = 0; round < idSplitter.NumberOfRequest; round++)
                    visualNovels.AddRange(AddToVisualNovelsList(idSplitter.IDs.Take(round * Constants.MaxVNsPerRequest, Constants.MaxVNsPerRequest).ToList()));

                if (idSplitter.Remainder > 0)
                    visualNovels.AddRange(AddToVisualNovelsList(idSplitter.IDs.Take(idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder).ToList()));
            }
            else
                visualNovels.AddRange(AddToVisualNovelsList(idSplitter.IDs.ToList()));

            return visualNovels;
        }

        public static VisualNovel FetchVisualNovel(int ID)
        {
            CheckConnection();

            var visualNovel = new VisualNovel();

            visualNovel.Basics = new BasicInformation(GetBasicInformation(ID).items[0]);

            foreach (var character in GetCharInfo(ID))
                visualNovel.Characters.Add(new CharacterInformation(character));

            return visualNovel;
        }

        private static List<VisualNovel> AddToVisualNovelsList(List<int> IDs)
        {
            var vns = new List<VisualNovel>();

            List<VNInformation> basicInformation = GetBasicInformation(IDs).items;
            List<VNCharacterInformation> charInformation = GetCharInfo(IDs);

            foreach (var id in IDs)
            {                
                VNInformation basic = basicInformation.Where(x => x.id == id).SingleOrDefault();
                List<VNCharacterInformation> chars = charInformation.FindAll(x => x.vns.Any(y => y.Any(u => u.ToString() == id.ToString()))).ToList();

                // Check if visual novel still exists.
                if (basic != null)
                {
                    var newVN = new VisualNovel();

                    newVN.Basics = new BasicInformation(basic);

                    foreach (var character in chars)
                        newVN.Characters.Add(new CharacterInformation(character));

                    vns.Add(newVN);
                }
            }

            return vns;
        }

        public static List<VN> GetVisualNovelListFromVNDB()
        {
            CheckConnection();

            return GetList<VN, VNListRoot>(Connection.QueryVNList);
        }

        public static List<Vote> GetVoteListFromVNDB()
        {
            CheckConnection();

            return GetList<Vote, VoteListRoot>(Connection.QueryVoteList);
        }

        public static void SetVNList(VisualNovel VN)
        {
            CheckConnection();

            SetList<SetJSONObjects.State>(VN, new SetJSONObjects.State { status = (int)VN.Category }, Connection.SetVNList);
        }

        public static void SetVoteList(VisualNovel VN)
        {
            CheckConnection();

            SetList<SetJSONObjects.Vote>(VN, new SetJSONObjects.Vote { vote = VN.Score }, Connection.SetVote);
        }

        public static void RemoveFromVNList(VisualNovel VN)
        {
            CheckConnection();

            RemoveFromList(VN, Connection.DeleteVNFromVNList);
        }

        public static void RemoveFromScoreList(VisualNovel VN)
        {
            CheckConnection();

            RemoveFromList(VN, Connection.DeleteVote);
        }

        private static VNInformationRoot GetBasicInformation(List<int> IDs)
        {
            VndbResponse result = Connection.QueryInformation(IDs.ToArray());

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (HandleError(result) == ErrorResponse.Throttled)
                    return GetBasicInformation(IDs);
                else
                    return null;
            }
            else
                return JsonConvert.DeserializeObject<VNInformationRoot>(result.Payload);
        }

        private static VNInformationRoot GetBasicInformation(int ID)
        {
            VndbResponse result = Connection.QueryInformation(ID);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (HandleError(result) == ErrorResponse.Throttled)
                    return GetBasicInformation(ID);
                else
                    return null;
            }
            else
                return JsonConvert.DeserializeObject<VNInformationRoot>(result.Payload);
        }

        private static List<VNCharacterInformation> GetCharInfo(int ID)
        {
            VndbResponse result = Connection.QueryCharacterInformation(ID);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (HandleError(result) == ErrorResponse.Throttled)
                    return GetCharInfo(ID);
                else
                    return new List<VNCharacterInformation>();
            }
            else
                return JsonConvert.DeserializeObject<VNCharacterInformationRoot>(result.Payload).items;
        }

        private static List<VNCharacterInformation> GetCharInfo(List<int> IDs)
        {
            var Chars = new List<VNCharacterInformation>();

            VNCharacterInformationRoot convertedResult;
            int page = 1;

            do
            {                
                convertedResult = JsonConvert.DeserializeObject<VNCharacterInformationRoot>(QueryChar(Connection.QueryCharacterInformation, page, IDs.ToArray()).Payload);

                page++;

                Chars.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return Chars;
        }

        private static VndbResponse QueryChar(Func<int[], int, VndbResponse> Query, int page, int[] IDs)
        {
            VndbResponse result = Query(IDs, page);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (HandleError(result) == ErrorResponse.Throttled)
                    return QueryChar(Query, page, IDs);
                else
                    return null;
            }
            else
                return result;
        }

        private static void SetList<T>(VisualNovel VN, T setObject, Func<int, T, VndbResponse> SetQuery)
        {
            VndbResponse result = SetQuery(VN.Basics.VNDBInformation.id, setObject);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (HandleError(result) == ErrorResponse.Throttled)
                    SetList<T>(VN, setObject, SetQuery);
            }
        }

        private static void RemoveFromList(VisualNovel VN, Func<int, VndbResponse> Query)
        {
            VndbResponse result = Query(VN.Basics.VNDBInformation.id);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (HandleError(result) == ErrorResponse.Throttled)
                    RemoveFromList(VN, Query);
            }
        }

        private static List<T> GetList<T, U>(Func<int, VndbResponse> Query) where U : GetTemplate<T>
        {
            var List = new List<T>();

            U convertedResult;
            int page = 1;

            do
            {
                convertedResult = JsonConvert.DeserializeObject<U>(QueryList(Query, page));
                page++;

                List.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return List;
        }

        private static string QueryList(Func<int, VndbResponse> Query, int page)
        {
            VndbResponse result = Query(page);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (HandleError(result) == ErrorResponse.Throttled)
                    return QueryList(Query, page);
                else
                    return string.Empty;
            }
            else
                return result.Payload;
        }

        private static ErrorResponse HandleError(VndbResponse Response)
        {
            var error = JsonConvert.DeserializeObject<Error>(Response.Payload);

            if (error.id == "throttled")
            {
                // In case of 'throttled' error wait 'minwait'.
                EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(HandleError), Constants.VNDBConnectionFailedThrottling + TimeSpan.FromSeconds(error.minwait).ToString());                
                Thread.Sleep(TimeSpan.FromSeconds(error.minwait));
                _Status = VNDBCommunicationStatus.Throttled;
                return ErrorResponse.Throttled;
            }
            else if (error.id == "auth")
            {
                EventLogger.LogInformation(nameof(HandleError) + ":" + nameof(HandleError), Constants.VNDBConnectionAuthenticationError);
                _Status = VNDBCommunicationStatus.Error;
                _ErrorMessage = "Wrong Username/Password";
                return ErrorResponse.AuthenticationFailed;
            }
            else
            {
                EventLogger.LogInformation(nameof(VNDBCommunication) + nameof(HandleError), Constants.VNDBConnectionUnknownErrorReceived);                
                _Status = VNDBCommunicationStatus.Error;
                _ErrorMessage = "Error while communicating with VNDB: Error ID: '" + error.id + "' Error message: '" + error.msg + "'";
                return ErrorResponse.Unknown;
            }
        }

        private static void CheckConnection()
        {
            if (_Status != VNDBCommunicationStatus.LoggedIn || !Connection.IsConnected)
                Connect();
        }

        public static void Disconnect()
        {
            if (_Status == VNDBCommunicationStatus.LoggedIn)
            {
                Connection.Disconnect();
                _Status = VNDBCommunicationStatus.NotLoggedIn;
                EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Disconnect), Constants.ConnectionAborted);                
            }
        }

        public static void Dispose()
        {
            if (_Status == VNDBCommunicationStatus.LoggedIn)
            {
                Disconnect();
                Connection.Dispose();
                EventLogger.LogInformation(nameof(VNDBCommunication) + ":" + nameof(Dispose), Constants.ObjectDisposed);                
            }
        }
    }
}
