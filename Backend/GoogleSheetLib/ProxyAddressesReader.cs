using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace GoogleSheetLib
{
    static class ProxyAddressesReader
    {
        private static readonly JObject addresses = JObject.Parse(File.ReadAllText("configs/proxy_adresses.json"));

        public static IEnumerable<IPEndPoint> Addresses { get => addresses["Addresses"].ToObject<string[]>().Select(address => Util.ToEndPoint(address)); }
 
    }
}
