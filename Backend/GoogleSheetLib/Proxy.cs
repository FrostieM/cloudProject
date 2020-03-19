using System;
using System.Net;
using SCReader = GoogleSheetLib.SpreadsheetConfigReader;

namespace GoogleSheetLib
{
    static class Proxy
    {
        private static readonly object locker = new object();
        private static IWebProxy proxy = null;
        public static IWebProxy Get
        {

            get { lock (locker) { return GetProxy(); } }
            set { lock (locker) { proxy = value; } }
        }

        private static IWebProxy GetProxy()
        {
            if (proxy == null)
                proxy = new ProxyСustomizer(SCReader.UseCredentials).Proxy;

            if (SCReader.UseCredentials)
                return proxy;

            if (ConnectionChecker.CheckProxy(proxy).Result)
                return proxy;
            else
            {
                Console.WriteLine("Proxy server connection lost!");
                ConnectionChecker.FindNewFastestProxyAddress();
                proxy = new ProxyСustomizer().Proxy;
                return proxy;
            }
        }               
    }

    public class ProxyСustomizer
    {
        public IPAddress Address { get => EndPoint.Address; }
        public int Port { get => EndPoint.Port; }
        public IPEndPoint EndPoint { get; private set; }
        public Uri Uri { get => new Uri($"http://{Address}:{Port.ToString()}"); }
        public IWebProxy Proxy { get; private set; }

        public ProxyСustomizer(bool useCredentials = false)
        {
            CredentialCache credentials = null;
            if (useCredentials)
            {
                EndPoint = Util.ToEndPoint(SCReader.ProxyAddress, SCReader.ProxyPort);
                credentials = new CredentialCache();
                credentials.Add(Uri, "Basic",
                    new NetworkCredential(SCReader.ProxyUsername, SCReader.ProxyPassword));
            }
            else
            {
                EndPoint = ConnectionChecker.FastestProxyAddress;
                if (EndPoint == null)
                    throw new Exception("Нет доступных прокси. Увы =(");
            }
            Proxy = new WebProxy(Uri, true, null, credentials);
        }                
    }
}
