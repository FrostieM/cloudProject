using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SCReader = GoogleSheetLib.SpreadsheetConfigReader;

namespace GoogleSheetLib
{
    static class ConnectionChecker
    {
        private static IPEndPoint fastestAddress = null;

        public static IPEndPoint FastestProxyAddress
        {
            get
            {
                if (fastestAddress == null)
                    fastestAddress = GetFastestProxyAddress();
                return fastestAddress;
            }
            set => fastestAddress = value;
        }

        public static void FindNewFastestProxyAddress()
        {
            FastestProxyAddress = GetFastestProxyAddress();
        }

        private static IPEndPoint GetFastestProxyAddress()
        {
            Console.WriteLine("Searching for the fastest proxy server...");
            var proxyAddresses = ProxyAddressesReader.Addresses.AsParallel().Select(GetProxyResponseTime)
                .Select(task => task.Result).Where(result => result != null);
            if (proxyAddresses.Count() > 0)
            {
                var fastestAddress = proxyAddresses.MinBy(adress => adress.Value.Value).Value.Key;
                Console.WriteLine($"Connecting to proxy server [{fastestAddress}]...");
                return Util.ToEndPoint(fastestAddress);
            }
            Console.WriteLine($"Searching failed...");
            return null;
        }

        public static async Task<bool> CheckProxy(IWebProxy proxy)
        {
            try
            {
                var handler = new HttpClientHandler { Proxy = proxy, UseProxy = true };
                using (var httpClient = new HttpClient(handler, true))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    var responce = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://httpbin.org/ip"));
                    if (responce.IsSuccessStatusCode)
                        return true;
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static async Task<bool> CheckInternet()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    var responce = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://httpbin.org/ip"));
                    if (responce.IsSuccessStatusCode)
                        return true;
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<KeyValuePair<string, long>?> GetProxyResponseTime(IPEndPoint endPoint)
        {
            try
            {
                var address = $"{endPoint.Address.ToString()}:{endPoint.Port.ToString()}";
                var uri = string.Format($"http://{address}");
                var proxy = new WebProxy(uri, true, null, null);
                var handler = new HttpClientHandler { Proxy = proxy, UseProxy = true };
                using (var httpClient = new HttpClient(handler, true))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(1);
                    var stopwatch = Stopwatch.StartNew();
                    var responce = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://httpbin.org/ip"));
                    if (responce.IsSuccessStatusCode)
                        return new KeyValuePair<string, long>(address, stopwatch.ElapsedMilliseconds);
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public static async Task<KeyValuePair<string, long>?> GetProxyResponseTime(string address)
        {
            return GetProxyResponseTime(Util.ToEndPoint(address)).Result;
        }

        public static bool HasInternet()
        {
            if (SCReader.UseProxy)
            {
                if (SCReader.UseCredentials)
                    return CheckProxy(new ProxyСustomizer(true).Proxy).Result;
                else
                {
                    if (FastestProxyAddress == null)
                        return false;
                    return CheckProxy(new ProxyСustomizer().Proxy).Result;
                }
            }
            else
                return CheckInternet().Result;
        }
    }
}
