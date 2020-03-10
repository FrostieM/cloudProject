using Google.Apis.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Linq;
using MoreLinq;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GoogleSheetLib
{
    class ProxyHttpClientFactory : HttpClientFactory
    {
        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var proxy = new WebProxy(Proxy.Adress, true, null, null);
            var webRequestHandler = new HttpClientHandler()
            {
                UseProxy = true,
                Proxy = proxy,
                UseCookies = false
            };
            return webRequestHandler;
        }
    }

    static class Proxy
    {
        public static string Adress { get; } = SpreadsheetConfigReader.UseProxy ? GetFastest() : "";

        private static IEnumerable<string> ReadProxyAdresses()
        {
            var proxyAdresses = JObject.Parse(File.ReadAllText("configs/proxy_adresses.json"));
            return proxyAdresses["Adresses"].ToObject<string[]>();
        }

        private static async Task<KeyValuePair<string, long>?> CheckProxy(string address)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                var uri = string.Format($"http://{address}");
                var proxy = new WebProxy(uri, true, null, null);
                var handler = new HttpClientHandler { Proxy = proxy, UseProxy = true };

                using (var httpClient = new HttpClient(handler, true))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(1);
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

        private static string GetFastest()
        {
            var adresses = ReadProxyAdresses();
            Console.WriteLine("Searching for the fastest proxy server...");
            var proxyAdress = adresses.AsParallel().Select(CheckProxy)
                .Select(task => task.Result).Where(result => result != null)
                .MinBy(adress => adress.Value.Value).Value.Key;
            Console.WriteLine($"Connecting to proxy server [{proxyAdress}]...");
            return proxyAdress;
        }
    }
}
