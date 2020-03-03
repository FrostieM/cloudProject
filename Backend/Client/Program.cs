using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using ClassLib;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client
{
    class Program
    {
        private static JObject _config;
        private static string _serverIp;
        private static int _serverPort;
        private static void Main(string[] args)
        {
            LoadConfig();
            var apps = GetAppsList();
            var diff = GetDifference(apps);
            var data = new AppsContainer { apps = diff, hostname = Dns.GetHostName() }.GetBytes();
            Network.TcpSend(data, _serverIp, _serverPort);
            SaveToCache(apps);
        }
        private static void LoadConfig()
        {
            try
            {
                _config = JObject.Parse(File.ReadAllText("config.json"));
                _serverIp = _config["server"]["ip"].ToObject<string>();
                _serverPort = _config["server"]["port"].ToObject<int>();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Process.GetCurrentProcess().Kill();
            }
        }
        static List<App> GetAppsList()
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
            var name = _config["cacheFile"].ToString();
            var result = new List<App>();
            if (!File.Exists(name)) return apps;
            var cached = JsonConvert.DeserializeObject<List<App>>(File.ReadAllText(name));
            foreach (var app in apps)
            {
                var cachedApp = cached.Find(a => a.name == app.name);
                if (cachedApp != null)
                {
                    cached.Remove(cachedApp);
                    if (app.version == cachedApp.version) continue;
                    app.status = AppStatus.Updated;
                }
                result.Add(app);
            }
            cached.ForEach(app => app.status = AppStatus.Deleted);
            result.AddRange(cached);
            return result;
        }
        private static void SaveToCache(List<App> apps)
        {
            var name = _config["cacheFile"].ToString();
            var json = JsonConvert.SerializeObject(apps);
            File.Delete(name);
            using (var writer = File.CreateText(name))
                writer.Write(json);
        }
    }
}