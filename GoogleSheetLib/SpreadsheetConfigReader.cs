using Newtonsoft.Json.Linq;
using System.IO;

namespace GoogleSheetLib
{
    static class SpreadsheetConfigReader
    {
        private static readonly JObject config = JObject.Parse(File.ReadAllText("configs/spreadsheet_config.json"));
               
        public static string ApiKey { get => config["ApiKey"].ToString(); }
        public static string ApplicationName { get => config["ApplicationName"].ToString(); }
        public static string ServiceAccount { get => config["ServiceAccount"].ToString(); }
        public static string SpreadsheetId { get => config["SpreadsheetId"].ToString(); }

        public static bool UseProxy { get => config["UseProxy"].ToObject<bool>(); }
        public static bool UseCredentials { get => config["UseCredentials"].ToObject<bool>(); }

        public static string ProxyPassword { get => config["ProxyPassword"].ToString(); }
        public static string ProxyUsername { get => config["ProxyUsername"].ToString(); }
        public static string ProxyAddress { get => config["ProxyAddress"].ToString(); }
        public static int ProxyPort { get => config["ProxyPort"].ToObject<int>(); }

    }
}
