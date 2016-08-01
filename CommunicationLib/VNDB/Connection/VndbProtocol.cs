using System;
using System.Linq;
using System.Text;

namespace CommunicationLib.VNDB.Connection
{
    internal static class VndbProtocol
    {
        public const byte EndOfStreamByte = 0x04;
        
        private static Tuple<string, VndbResponseType>[] ResponseTypeMap = new Tuple<string, VndbResponseType>[]
        {
            new Tuple<string, VndbResponseType>("Ok", VndbResponseType.Ok),
            new Tuple<string, VndbResponseType>("Error", VndbResponseType.Error),
        };

        public static bool IsCompleteMessage(byte[] message, int bytesUsed)
        {
            if (bytesUsed == 0)
                throw new Exception("You have a bug, dummy. You should have at least one byte here.");

            if (message[bytesUsed - 1] == EndOfStreamByte)
                return true;
            else
                return false;
        }

        public static VndbResponse Parse(byte[] message, int bytesUsed)
        {
            if (!IsCompleteMessage(message, bytesUsed))
                throw new Exception("You have a bug, dummy.");

            string stringifiedResponse = Encoding.UTF8.GetString(message, 0, bytesUsed - 1);

            int firstSpace = stringifiedResponse.IndexOf(' ');

            if (firstSpace == bytesUsed - 1)
                throw new Exception("Protocol violation: last character in response is first space");
            else if (firstSpace == -1)
            {
                Tuple<string, VndbResponseType> responseTypeEntry = ResponseTypeMap.FirstOrDefault(
                    l => string.Compare(l.Item1, stringifiedResponse, StringComparison.OrdinalIgnoreCase) == 0);

                if (responseTypeEntry == null)
                    return new VndbResponse(VndbResponseType.Unknown, string.Empty);
                else
                    return new VndbResponse(responseTypeEntry.Item2, string.Empty);
            }
            else
            {
                string responseTypeString = stringifiedResponse.Substring(0, firstSpace);

                Tuple<string, VndbResponseType> responseTypeEntry = ResponseTypeMap.FirstOrDefault(
                    l => string.Compare(l.Item1, responseTypeString, StringComparison.OrdinalIgnoreCase) == 0);

                if (responseTypeEntry == null)
                    return new VndbResponse(VndbResponseType.Unknown, stringifiedResponse.Substring(firstSpace + 1));
                else
                    return new VndbResponse(responseTypeEntry.Item2, stringifiedResponse.Substring(firstSpace + 1));
            }
        }
    }
}
