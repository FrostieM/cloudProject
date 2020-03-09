using Google.Apis.Http;
using System.Net;
using System.Net.Http;

namespace GoogleSheetLib
{
    class ProxyHttpClientFactory : HttpClientFactory
    {
        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var proxy = new WebProxy(SpreadsheetConfigReader.Adress, true, null, null);
            var webRequestHandler = new HttpClientHandler()
            {
                UseProxy = true,
                Proxy = proxy,
                UseCookies = false
            };
            return webRequestHandler;
        }
    }
}
