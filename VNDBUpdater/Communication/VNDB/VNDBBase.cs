using CommunicationLib.VNDB;
using CommunicationLib.VNDB.Connection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Communication.VNDB.Interfaces;
using VNDBUpdater.Services.Logger;

namespace VNDBUpdater.Communication.VNDB
{
    public class VNDBBase : IVNDB
    {
        private IVNDBCommunication _Connection;
        private IUserRepository _UserRepository;
        private ILoggerService _Logger;

        private int _ConnectionTries = 0;
        private const int _MaxConnectionTries = 4;

        public VNDBBase(IUserRepository UserRepository, ILoggerService LoggerService)
        {
            _UserRepository = UserRepository;
            _Logger = LoggerService;

            Connect();
        }

        private string _Status;

        public string Status
        {
            get { return _Status; }
        }

        private bool _LoggedIn;

        public bool LoggedIn
        {
            get { return _LoggedIn; }
        }

        private bool _Error;

        public bool Error
        {
            get { return _Error; }
        }

        private bool _Throttled;

        public bool Throttled
        {
            get { return _Throttled; }
        }

        public void Connect()
        {
            if (!_LoggedIn || !_Connection.IsConnected)
            {
                try
                {
                    _Connection = new CommunicationLib.Communication().GetVNDBCommunication();
                    VndbResponse response = _Connection.Connect(_UserRepository.Get(0).Username, Convert.ToBase64String(_UserRepository.Get(0).EncryptedPassword));

                    if (response.ResponseType == VndbResponseType.Error)
                    {
                        ErrorResponse result = HandleError(response);

                        if (result == ErrorResponse.Throttled)
                        {
                            _Logger.Log("Connecting to VNDB failed because of throttling error. Trying reconnect.");
                            Connect();
                        }
                        else if (result == ErrorResponse.AuthenticationFailed)
                        {
                            _Logger.Log("Connecting to VNDB failed because of authentication error. Abort connection procedure.");
                            _Error = true;
                        }
                        else
                        {
                            _Logger.Log("Connecting to VNDB failed because of an unknown error. Abort connection procedure.");
                            _Error = true;
                        }
                    }
                    else
                    {
                        _Logger.Log("Connection to VNDB established successfully.");
                        _Error = false;
                        _LoggedIn = true;
                        _ConnectionTries = 0;
                    }

                }
                catch (IOException ex)
                {
                    // IOExceptins occurs if VNDB closed the stream.
                    // Just try to connect again using recursion.
                    _Logger.Log(ex);
                    _LoggedIn = false;
                    _ConnectionTries++;

                    if (_ConnectionTries != _MaxConnectionTries)
                    {
                        _Logger.Log("Error was handled. Trying reconnect. Current tries: " + _ConnectionTries.ToString());
                        Connect();
                    }
                    else
                    {
                        _Logger.Log("Error could not be handled. Maximal connection tries reacher.");
                        _ConnectionTries = 0;
                        _Error = true;
                        _Status = "Login failed. Error message: " + ex.Message + ex.GetType().ToString() + ex.GetBaseException().GetType().ToString();
                    }
                }
                catch (Exception ex)
                {
                    _Logger.Log(ex);
                    _Error = true;
                    _Status = "Login failed. Error message: " + ex.Message + ex.GetType().ToString() + ex.GetBaseException().GetType().ToString();
                }
            }
        }

        public void Reconnect()
        {
            Disconnect();
            Connect();
        }

        public void Disconnect()
        {
            if (_LoggedIn)
            {
                _Connection.Disconnect();
                _LoggedIn = false;
            }
        }

        public ErrorResponse HandleError(VndbResponse response)
        {
            var error = JsonConvert.DeserializeObject<Error>(response.Payload);

            if (error.id == "throttled")
            {
                // In case of 'throttled' error wait 'minwait'.
                _Logger.Log("Throttled by VNDB. Waiting: " + TimeSpan.FromSeconds(error.minwait).ToString());
                Thread.Sleep(TimeSpan.FromSeconds(error.minwait));
                _Throttled = true;
                return ErrorResponse.Throttled;
            }
            else if (error.id == "auth")
            {
                _Logger.Log("Authentication error by VNDB received.");
                _Error = true;
                _Status = "Wrong Username/Password";
                return ErrorResponse.AuthenticationFailed;
            }
            else
            {
                _Logger.Log("Unknown error by VNDB received.");
                _Error = true;
                _Status = "Error while communicating with VNDB: Error ID: '" + error.id + "' Error message: '" + error.msg + "'";
                return ErrorResponse.Unknown;
            }
        }

        public VndbResponse SetVNList(int ID, SetJSONObjects.State state)
        {
            return _Connection.SetVNList(ID, state);
        }

        public VndbResponse SetVote(int ID, SetJSONObjects.Vote vote)
        {
            return _Connection.SetVote(ID, vote);
        }

        public VndbResponse DeleteVote(int ID)
        {
            return _Connection.DeleteVote(ID);
        }

        public VndbResponse DeleteVNFromVNList(int ID)
        {
            return _Connection.DeleteVNFromVNList(ID);
        }

        public VndbResponse QueryCharacterInformation(int[] IDs, int page)
        {
            return _Connection.QueryCharacterInformation(IDs, page);
        }

        public VndbResponse QueryCharacterInformation(int ID)
        {
            return _Connection.QueryCharacterInformation(ID);
        }

        public VndbResponse QueryInformation(int ID)
        {
            return _Connection.QueryInformation(ID);
        }

        public VndbResponse SearchByTitle(string title, int page)
        {
            return _Connection.SearchByTitle(title, page);
        }

        public VndbResponse QueryInformation(int[] IDs)
        {
            return _Connection.QueryInformation(IDs);
        }

        public VndbResponse QueryVNList(int page = 1)
        {
            return _Connection.QueryVNList(page);
        }

        public VndbResponse QueryVoteList(int page = 1)
        {
            return _Connection.QueryVoteList(page);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_LoggedIn)
            {
                Disconnect();
                _Connection.Dispose();
            }
        }
    }
}
