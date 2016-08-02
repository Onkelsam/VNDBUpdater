using CommunicationLib.VNDB;
using CommunicationLib.VNDB.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models;
using VNUpdater.Helper;

namespace VNDBUpdater.Communication.VNDB
{
    public static class VNDBCommunication
    {
        private static IVNDBCommunication Connection;
        private static VNDBCommunicationStatus _Status = VNDBCommunicationStatus.NotLoggedIn;

        public const int MAXVNSPERREQUEST = 25;
        private static string _ErrorMessage;

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
                        return "Currently logged in as '" + RedisCommunication.GetUsername() + "'.";
                    case (VNDBCommunicationStatus.NotLoggedIn):
                        return "Currently not logged in. Please specify your Login-Credentials in the Options-Menu.";
                    case (VNDBCommunicationStatus.Throttled):
                        return "Currently logged in as '" + RedisCommunication.GetUsername() + "'. Throttled by VNDB. Please be patient.";
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
            if (_Status != VNDBCommunicationStatus.LoggedIn)
            {
                try
                {
                    Connection = new CommunicationLib.Communication().GetVNDBCommunication();
                    var response = Connection.Connect(RedisCommunication.GetUsername(), RedisCommunication.GetUserPassword());

                    if (response.ResponseType == VndbResponseType.Error)
                    {
                        if (HandleError(response) == ErrorResponse.Throttled)
                        {
                            Trace.TraceInformation("Connecting to VNDB failed because of throttling error. Trying reconnect.");
                            Connect(); // If the connection was throttled try again (after waiting 'minwait').
                        }                            
                        else
                        {
                            Trace.TraceInformation("Connecting to VNDB failed because of unknown error. Abort connection.");
                            _Status = VNDBCommunicationStatus.Error;
                        }                            
                    }
                    else
                    {
                        Trace.TraceInformation("Connection to VNDB established successfully.");
                        _Status = VNDBCommunicationStatus.LoggedIn;
                    }
                        
                }
                catch (IOException ex)
                {
                    // IOExceptins occurs if VNDB closed the stream.
                    // Just try to connect again using recursion.
                    Trace.TraceError("Caught exception while connecting to VNDB:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.GetType().Name + Environment.NewLine + ex.StackTrace);
                    Trace.TraceInformation("Error handled. Trying reconnect.");
                    _Status = VNDBCommunicationStatus.NotLoggedIn;
                    Connect();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Caught exception while connecting to VNDB:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.GetType().Name + Environment.NewLine + ex.StackTrace);
                    Trace.TraceInformation("Error could not be handled. Abort connection.");
                    _Status = VNDBCommunicationStatus.Error;
                    _ErrorMessage = "Login failed. Error message: " + ex.Message + ex.GetType().ToString() + ex.GetBaseException().GetType().ToString();
                }
            }
        }

        public static List<VisualNovel> FetchVisualNovels(List<int> IDs)
        {
            var visualNovels = new List<VisualNovel>();

            var idSplitter = new VNIDsSplitter(IDs.ToArray());

            idSplitter.Split();

            if (idSplitter.SplittingNeccessary)
            {
                for (int round = 0; round < idSplitter.NumberOfRequest; round++)
                    AddToVisualNovelsList(idSplitter.IDs.Take(round * MAXVNSPERREQUEST, MAXVNSPERREQUEST).ToList(), visualNovels);

                if (idSplitter.Remainder > 0)
                    AddToVisualNovelsList(idSplitter.IDs.Take(idSplitter.IDs.Length - idSplitter.Remainder, idSplitter.Remainder).ToList(), visualNovels);
            }
            else
                AddToVisualNovelsList(idSplitter.IDs.ToList(), visualNovels);

            return visualNovels.OrderBy(x => x.Basics.id).ToList();
        }

        public static VisualNovel FetchVisualNovel(int ID)
        {
            var visualNovel = new VisualNovel();

            visualNovel.Basics = GetBasicInformation(ID).items[0];
            visualNovel.Characters = GetCharInfo(ID);

            return visualNovel;
        }

        private static void AddToVisualNovelsList(List<int> IDs, List<VisualNovel> visualNovels)
        {
            var basicInformation = GetBasicInformation(IDs).items;
            var charInformation = GetCharInfo(IDs);

            foreach (var id in IDs)
            {                
                var basic = basicInformation.Where(x => x.id == id).FirstOrDefault();
                var chars = charInformation.FindAll(x => x.vns.Any(y => y.Any(u => u.ToString() == id.ToString()))).ToList();

                // Check if visual novel still exists.
                if (basic != null)
                {
                    var newVN = new VisualNovel()
                    {
                        Basics = basic,
                        Characters = chars,
                    };

                    visualNovels.Add(newVN);
                }
            }
        }

        public static List<VN> GetVisualNovelListFromVNDB()
        {
            return GetList<VN, VNListRoot>(Connection.QueryVNList);
        }

        public static List<Wish> GetWishListFromVNDB()
        {
            return GetList<Wish, WishListRoot>(Connection.QueryWishList);
        }

        public static List<Vote> GetVoteListFromVNDB()
        {
            return GetList<Vote, VoteListRoot>(Connection.QueryVoteList);
        }

        public static void SetVNList(VisualNovel VN)
        {
            SetList<SetJSONObjects.State>(VN, new SetJSONObjects.State { status = (int)VN.Category }, Connection.SetVNList);
        }

        public static void SetWishList(VisualNovel VN)
        {
            SetList<SetJSONObjects.Priority>(VN, new SetJSONObjects.Priority { priority = 1 }, Connection.SetWishList);
        }

        public static void SetVoteList(VisualNovel VN)
        {
            SetList<SetJSONObjects.Vote>(VN, new SetJSONObjects.Vote { vote = VN.Score }, Connection.SetVote);
        }

        public static void RemoveFromVNList(VisualNovel VN)
        {
            RemoveFromList(VN, Connection.DeleteVNFromVNList);
        }

        public static void RemoveFromWishList(VisualNovel VN)
        {
            RemoveFromList(VN, Connection.DeleteVNFromWishList);
        }

        public static void RemoveFromScoreList(VisualNovel VN)
        {
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
                    return new VNInformationRoot();
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
                    return new VNInformationRoot();
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
                VndbResponse result = Connection.QueryCharacterInformation(IDs.ToArray(), page);

                if (result.ResponseType == VndbResponseType.Error)
                {
                    if (HandleError(result) == ErrorResponse.Throttled)
                        result = Connection.QueryCharacterInformation(IDs.ToArray(), page);
                    else
                        return new List<VNCharacterInformation>();
                }

                page++;

                convertedResult = JsonConvert.DeserializeObject<VNCharacterInformationRoot>(result.Payload);

                Chars.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return Chars;
        }

        private static void SetList<T>(VisualNovel VN, T setObject, Func<int, T, VndbResponse> SetQuery)
        {
            VndbResponse result = SetQuery(VN.Basics.id, setObject);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (HandleError(result) == ErrorResponse.Throttled)
                    SetList<T>(VN, setObject, SetQuery);
            }
        }

        private static void RemoveFromList(VisualNovel VN, Func<int, VndbResponse> Query)
        {
            VndbResponse result = Query(VN.Basics.id);

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
                Trace.TraceInformation("Throttled error by VNDB detected. Waiting " + TimeSpan.FromSeconds(error.minwait).ToString() + " before proceeding with Queries.");
                Thread.Sleep(TimeSpan.FromSeconds(error.minwait));
                _Status = VNDBCommunicationStatus.Throttled;
                return ErrorResponse.Throttled;
            }
            else
            {
                Trace.TraceError("Unknown error by VNDB caught. Aborting connection.");
                _Status = VNDBCommunicationStatus.Error;
                _ErrorMessage = "Error while communicating with VNDB: Error ID: '" + error.id + "' Error message: '" + error.msg + "'";
                return ErrorResponse.Unknown;
            }
        }

        public static void Disconnect()
        {
            if (_Status == VNDBCommunicationStatus.LoggedIn)
            {
                Connection.Disconnect();
                _Status = VNDBCommunicationStatus.NotLoggedIn;
                Trace.TraceInformation("VNDB unlogged successfully.");
            }
        }

        public static void Dispose()
        {
            if (_Status == VNDBCommunicationStatus.LoggedIn)
            {
                Connection.Disconnect();
                Connection.Dispose();
                _Status = VNDBCommunicationStatus.NotLoggedIn;
                Trace.TraceInformation("VNDB connection disposed successfully.");
            }
        }
    }
}
