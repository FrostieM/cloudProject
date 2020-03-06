using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using ClassLib;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client
{
    class Config
    {
        public string ServerIP;
        public int ServerPort;
        public string CacheFileName;
        public List<string> FilterNames;
    }
    static class Program
    {
        private static Config config;

        private static void Main()
        {
            LoadConfig();
            var apps = GetAppsList();
            var diff = GetDifference(apps);
            var data = new AppsContainer { apps = diff, hostname = Dns.GetHostName() }.GetBytes();
            Network.TcpSend(data, config.ServerIP, config.ServerPort);
            SaveToCache(apps);
        }
        private static void LoadConfig()
        {
            try
            {
                var json = JObject.Parse(File.ReadAllText("config.json"));
                config = new Config
                {
                    ServerIP = json["ip"].ToObject<string>(),
                    ServerPort = json["port"].ToObject<int>(),
                    CacheFileName = json["cacheFile"].ToString(),
                    FilterNames = json["filterNames"].ToObject<List<string>>()
                };
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Process.GetCurrentProcess().Kill();
            }
        }
        private static List<App> GetAppsList()
        {
            var softPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            var regArr = new List<RegistryKey>();
            regArr.Add(GetKeyFromHive(RegistryHive.LocalMachine,softPath));
            regArr.Add(GetKeyFromHive(RegistryHive.CurrentUser,softPath));
            foreach (var user in RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64).GetSubKeyNames())
                regArr.Add(GetKeyFromHive(RegistryHive.Users, $@"{user}\{softPath}"));
            var apps = new List<App>();
            foreach (var key in regArr) apps.AddRange(GetAppsFromBranch(key));
            apps = apps.Distinct().OrderBy(app=>app.name).ToList();
            return apps;
        }
        private static RegistryKey GetKeyFromHive(RegistryHive hive,string path) => 
            RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(path);
        private static List<App> GetAppsFromBranch(RegistryKey mainKey)
        {
            var apps = new List<App>();
            if (mainKey == null) return apps;
            foreach (var subKey1 in mainKey.GetSubKeyNames())
            {
                var subKey2 = mainKey.OpenSubKey(subKey1);
                var nameValue = subKey2?.GetValue("DisplayName");
                if (nameValue == null || config.FilterNames.Any(n => nameValue.ToString().StartsWith(n))) continue;
                var versionValue = subKey2.GetValue("DisplayVersion");
                var version = versionValue?.ToString() ?? string.Empty;
                apps.Add(new App(nameValue.ToString(), version));
            }
            return apps;
        }
        private static List<App> GetDifference(List<App> apps)
        {
            var result = new List<App>();
            if (!File.Exists(config.CacheFileName)) return apps;
            var cached = JsonConvert.DeserializeObject<List<App>>(File.ReadAllText(config.CacheFileName));
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
            var json = JsonConvert.SerializeObject(apps);
            File.Delete(config.CacheFileName);
            using (var writer = File.CreateText(config.CacheFileName))
                writer.Write(json);
        }
    }
}