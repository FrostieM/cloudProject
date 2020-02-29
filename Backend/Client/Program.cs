using System.Collections.Generic;
using System.IO;
using System.Net;
using ClassLib;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client
{
    class Program
    {
        private static JObject config;
        private static void Main(string[] args)
        {
            Program.config = JObject.Parse(File.ReadAllText("config.json"));
            var apps = GetAppList();
            var diff = GetDifference(apps);
            SendToServer(diff);
            SaveToCache(apps);
        }
        static List<App> GetAppList()
        {
            var apps = new List<App>();
            var SoftwareKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            var rk = RegistryKey
                .OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .OpenSubKey(SoftwareKey);
            foreach (var key in rk.GetSubKeyNames())
            {
                var subKey = rk.OpenSubKey(key);
                var name = subKey?.GetValue("DisplayName");
                if (name == null) continue;
                var version = subKey.GetValue("DisplayVersion")?.ToString() ?? string.Empty;
                apps.Add(new App(name.ToString(), version));
            }
            return apps;
        }
        private static List<App> GetDifference(List<App> apps)
        {
            var name = config["cacheFile"].ToString();
            var result = new List<App>();
            if (!File.Exists(name)) return apps;
            var cached = JsonConvert.DeserializeObject<List<App>>(File.ReadAllText(name));
            foreach (var app in apps)
            {
                var cachedApp = cached.Find(a => a.name == app.name);
                if (cachedApp == null) continue;
                if (app.version != cachedApp.version)
                {
                    app.status = AppStatus.Updated;
                    result.Add(app);
                }
                cached.Remove(cachedApp);
            }
            cached.ForEach(app => app.status = AppStatus.Deleted);
            result.AddRange(cached);
            return result;
        }
        private static void SaveToCache(List<App> apps)
        {
            var name = config["cacheFile"].ToString();
            var json = JsonConvert.SerializeObject(apps);
            File.Delete(name);
            using (var writer = File.CreateText(name))
                writer.Write(json);
        }
        private static void SendToServer(List<App> apps)
        {
            var ip = config["server"]["ip"].ToObject<string>();
            var port = config["server"]["port"].ToObject<int>();
            using (var client = Network.GetConnectionClient(ip, port))
            using (var stream = client.GetStream())
            {
                var container = new AppsContainer { apps = apps, hostname = Dns.GetHostName() };
                var data = container.GetBytes();
                stream.Write(data,0,data.Length);
            }
        }
    }
}