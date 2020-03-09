using Newtonsoft.Json.Linq;
using System.IO;

namespace GoogleSheetLib
{
    static class SpreadsheetConfigReader
    {
        private static JObject config = JObject.Parse(File.ReadAllText("spreadsheet_config.json"));

        public static string Adress { get => string.Format($"http://{ProxyIp}:{ProxyPort}"); }
        public static string ApiKey { get => config["ApiKey"].ToString(); }
        public static string ApplicationName { get => config["ApplicationName"].ToString(); }
        public static string ProxyIp { get => config["ProxyIp"].ToString(); }
        public static string ProxyPort { get => config["ProxyPort"].ToString(); }
        public static string ServiceAccount { get => config["ServiceAccount"].ToString(); }
        public static string SpreadsheetId { get => config["SpreadsheetId"].ToString(); }
        public static bool UseProxy { get => config["UseProxy"].ToObject<bool>(); }
    }
}
