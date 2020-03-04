using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Network
    {
        /// <summary>
        /// Cycle of processing incoming connections.
        /// </summary>
        /// <param name="port">Listenable port.</param>
        /// <param name="callback">Function for processing incoming connections.</param>
        public static void TcpListen(int port, Action<TcpClient> callback)
        {
            TcpListener listener=null;
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                while (true)
                {
                    try
                    {
                        var client = listener.AcceptTcpClient();
                        Task.Run(() => callback(client));
                    }
                    catch (Exception e){Console.WriteLine(e);}
                }
            }
            catch (Exception ex){Console.WriteLine(ex.Message);}
            finally{listener?.Stop();}
        }
        public static TcpClient GetConnectionClient(string ip, int port)
        {
            TcpClient client = null;
            do
            {
                try
                {
                    client = new TcpClient(ip, port);
                }
                catch (Exception)
                {
                    Thread.Sleep(5000);
                }
            } while (client == null || !client.Connected);
            return client;
        }
        public static void TcpSend(byte[] data, string ip, int port)
        {
            using (var client = GetConnectionClient(ip, port))
                using (var stream = client.GetStream())
                    stream.Write(data, 0, data.Length);
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
