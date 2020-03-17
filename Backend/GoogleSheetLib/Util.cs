using System.Net;

namespace GoogleSheetLib
{
    static class Util
    {
        public static IPEndPoint ToEndPoint(string address)
        {
            var ip = IPAddress.Parse(address.Substring(0, address.IndexOf(":")));
            var port = int.Parse(address.Substring(address.IndexOf(":") + 1));

            return new IPEndPoint(ip, port);
        }

        public static IPEndPoint ToEndPoint(string ip, int port)
        {
            return new IPEndPoint(IPAddress.Parse(ip), port);
        }
    }
}
