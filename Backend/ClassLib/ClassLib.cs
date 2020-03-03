using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ClassLib
{
    public enum AppStatus
    {
        Installed,
        Updated,
        Deleted
    }

    public class App
    {
        public AppStatus status;
        public string name;
        public string version;

        public App(string name, string version)
        {
            this.name = name;
            this.version = version;
        }

        public override string ToString() => $"{name} {version}";
    }

    public class AppsContainer
    {
        public string hostname;
        public long date;
        public List<App> apps;

        public AppsContainer()
        {
            date = DateTimeOffset.Now.ToUnixTimeSeconds();
            apps = new List<App>();
        }

        public byte[] GetBytes()
        {
            var jsoned = JsonConvert.SerializeObject(this);
            return Encoding.Unicode.GetBytes(jsoned);
        }
    }
}