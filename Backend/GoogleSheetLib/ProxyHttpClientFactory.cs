using Google.Apis.Http;
using System.Net.Http;
using SCReader = GoogleSheetLib.SpreadsheetConfigReader;

namespace GoogleSheetLib
{
    class ProxyHttpClientFactory : HttpClientFactory
    {
        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var webRequestHandler = new HttpClientHandler()
            {
                UseProxy = true,
                Proxy = Proxy.Get,
                UseCookies = false
            };
            return webRequestHandler;
        }
    }
}
