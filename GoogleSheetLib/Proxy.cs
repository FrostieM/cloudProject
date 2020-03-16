using MoreLinq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SCReader = GoogleSheetLib.SpreadsheetConfigReader;

namespace GoogleSheetLib
{
    static class Proxy
    {
        private static IWebProxy proxy = new ProxyСustomizer(SCReader.UseCredentials).WebProxy;
        public static IWebProxy Get
        {
            get
            {
                if (SCReader.UseCredentials)
                    return proxy;

                if(CheckConnection().Result)
                    return proxy;
                else
                {
                    proxy = new ProxyСustomizer().WebProxy;
                    return proxy;
                }
            }
            set
            {
                proxy = value;
            }
        }

        private static async Task<bool> CheckConnection()
        {
            try
            {
                var handler = new HttpClientHandler { Proxy = proxy, UseProxy = true };
                using (var httpClient = new HttpClient(handler, true))
                {
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
    }

    public class ProxyСustomizer
    {
        private IWebProxy proxy = null;

        public IPAddress Address { get => EndPoint.Address; }
        public int Port { get => EndPoint.Port; }
        public IPEndPoint EndPoint { get; private set; }
        public Uri Uri { get => new Uri($"http://{Address}:{Port.ToString()}"); }
        public IWebProxy WebProxy { get => proxy; }

        public ProxyСustomizer(bool useCredentials = false)
        {
            CredentialCache credentials = null;
            if (useCredentials)
            {
                EndPoint = ToEndPoint(SCReader.ProxyAddress, SCReader.ProxyPort);
                credentials = new CredentialCache();
                credentials.Add(Uri, "Basic",
                    new NetworkCredential(SCReader.ProxyUsername, SCReader.ProxyPassword));
            }
            else
                EndPoint = GetFastestAdress();

            proxy = new WebProxy(Uri, true, null, credentials);
        }

        private async Task<KeyValuePair<string, long>?> CheckProxy(string address)
        {
            try
            {
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

        private IEnumerable<string> ReadProxyAdresses()
        {
            var proxyAdresses = JObject.Parse(File.ReadAllText("configs/proxy_adresses.json"));
            return proxyAdresses["Adresses"].ToObject<string[]>();
        }

        private IPEndPoint GetFastestAdress()
        {
            var adresses = ReadProxyAdresses();
            Console.WriteLine("Searching for the fastest proxy server...");
            var proxyAdress = adresses.AsParallel().Select(CheckProxy)
                .Select(task => task.Result).Where(result => result != null)
                .MinBy(adress => adress.Value.Value).Value.Key;
            Console.WriteLine($"Connecting to proxy server [{proxyAdress}]...");
            return ToEndPoint(proxyAdress);
        }

        private IPEndPoint ToEndPoint(string address)
        {
            address = RemoveWhitespace(address);
            var ip = IPAddress.Parse(address.Substring(0, address.IndexOf(":")));
            var port = int.Parse(address.Substring(address.IndexOf(":") + 1));

            return new IPEndPoint(ip, port);
        }

        private IPEndPoint ToEndPoint(string ip, int port)
        {
            ip = RemoveWhitespace(ip);
            return new IPEndPoint(IPAddress.Parse(ip), port);
        }

        private string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
