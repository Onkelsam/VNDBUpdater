using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib.VNDB.Connection
{
    internal class VndbConnection : IDisposable
    {
        private TcpClient TcpClient;
        private Stream Stream;

        private const string VNDBHost = "api.vndb.org";
        private const ushort VNDBPort = 19535;

        public bool IsConnected
        {
            get { return TcpClient == null ? false : TcpClient.Connected; }
        }

        public VndbConnection()
        {
        }

        public async Task Connect()
        {
            TcpClient = new TcpClient();

            await TcpClient.ConnectAsync(VNDBHost, VNDBPort);

            var sslStream = new SslStream(TcpClient.GetStream());

            await sslStream.AuthenticateAsClientAsync("api.vndb.org");

            Stream = sslStream;
        }

        public async Task<VndbResponse> Login(string username, string password)
        {
            var Login = new LoginData(username, password);

            return await IssueCommandReadResponse("login " + JsonConvert.SerializeObject(Login));
        }

        public async Task<VndbResponse> Query(string query)
        {
            return await IssueCommandReadResponse(query);
        }

        private async Task<VndbResponse> IssueCommandReadResponse(string command)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(command);

            var requestBuffer = new byte[encoded.Length + 1];

            Buffer.BlockCopy(encoded, 0, requestBuffer, 0, encoded.Length);
            requestBuffer[encoded.Length] = VndbProtocol.EndOfStreamByte;

            await Stream.WriteAsync(requestBuffer, 0, requestBuffer.Length);

            var responseBuffer = new byte[4096];
            int totalRead = 0;

            while (true)
            {
                int currentRead = await Stream.ReadAsync(responseBuffer, totalRead, responseBuffer.Length - totalRead);

                if (currentRead == 0)
                    throw new Exception("Connection closed while reading login response.");

                totalRead += currentRead;

                if (VndbProtocol.IsCompleteMessage(responseBuffer, totalRead))
                    break;

                if (totalRead == responseBuffer.Length)
                {
                    var biggerBadderBuffer = new byte[responseBuffer.Length * 2];
                    Buffer.BlockCopy(responseBuffer, 0, biggerBadderBuffer, 0, responseBuffer.Length);
                    responseBuffer = biggerBadderBuffer;
                }
            }

            return VndbProtocol.Parse(responseBuffer, totalRead);
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                TcpClient.Client.Disconnect(true);
                TcpClient.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Dispose)
        {
            if (Dispose)
            {
                Disconnect();
                TcpClient.Client.Dispose();
                Stream.Dispose();
            }
        }
    }
}
