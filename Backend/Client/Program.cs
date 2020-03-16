using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;
using ClassLib;
using Microsoft.Win32;

namespace Client
{
    public class Config
    {
        public string ServerIP;
        public int ServerPort;
        public string CacheFileName;
        public List<string> FilterNames;
    }

    static class Program
    {
        private static Config _config;

        private static void Main()
        {
            LoadConfig();
            var apps = GetAppsList();
            var diff = GetDifference(apps);
            var container = new AppsContainer {apps = diff, hostname = Dns.GetHostName()};
            var data = container.GetBytes();
            Network.TcpSend(data, _config.ServerIP, _config.ServerPort);
            SaveToCache(container);
        }

        private static void LoadConfig()
        {
            try
            {
                var formatter = new XmlSerializer(typeof(Config));
                using (var fs = new FileStream("client_config.xml", FileMode.Open))
                    _config = (Config) formatter.Deserialize(fs);
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
            regArr.Add(GetKeyFromHive(RegistryHive.LocalMachine, softPath));
            regArr.Add(GetKeyFromHive(RegistryHive.CurrentUser, softPath));
            foreach (var user in RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64).GetSubKeyNames())
                regArr.Add(GetKeyFromHive(RegistryHive.Users, $@"{user}\{softPath}"));
            var apps = new List<App>();
            foreach (var key in regArr) apps.AddRange(GetAppsFromBranch(key));
            apps = apps.Distinct().OrderBy(app => app.name).ToList();
            return apps;
        }

        private static RegistryKey GetKeyFromHive(RegistryHive hive, string path) =>
            RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(path);

        private static List<App> GetAppsFromBranch(RegistryKey mainKey)
        {
            var apps = new List<App>();
            if (mainKey == null) return apps;
            foreach (var subKey1 in mainKey.GetSubKeyNames())
            {
                var subKey2 = mainKey.OpenSubKey(subKey1);
                var nameValue = subKey2?.GetValue("DisplayName");
                if (nameValue == null || _config.FilterNames.Any(n => nameValue.ToString().StartsWith(n))) continue;
                var versionValue = subKey2.GetValue("DisplayVersion");
                var version = versionValue?.ToString() ?? string.Empty;
                apps.Add(new App(nameValue.ToString(), version));
            }
            return apps;
        }

        private static List<App> GetDifference(List<App> apps)
        {
            var result = new List<App>();
            if (!File.Exists(_config.CacheFileName)) return apps;
            var cachedApps = AppsContainer.FromBytes(File.ReadAllBytes(_config.CacheFileName)).apps;
            foreach (var app in apps)
            {
                var cachedApp = cachedApps.Find(a => a.name == app.name);
                if (cachedApp != null)
                {
                    cachedApps.Remove(cachedApp);
                    if (app.version == cachedApp.version) continue;
                    app.status = AppStatus.Updated;
                }
                result.Add(app);
            }

            cachedApps.ForEach(app => app.status = AppStatus.Deleted);
            result.AddRange(cachedApps);
            return result;
        }

        private static void SaveToCache(AppsContainer container)
        {
            var formatter = new BinaryFormatter();
            using (var fs = new FileStream(_config.CacheFileName, FileMode.OpenOrCreate))
                formatter.Serialize(fs, container);
        }
    }
}