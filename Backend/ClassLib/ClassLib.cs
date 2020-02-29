using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public override string ToString()
        {
            return $"{name} {version}";
        }
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

        public static AppsContainer LoadFromBytes(byte[] data)
        {
            var message = Encoding.Unicode.GetString(data);
            return JsonConvert.DeserializeObject<AppsContainer>(message);
        }

        public byte[] GetBytes()
        {
            var jsoned = JsonConvert.SerializeObject(this);
            return Encoding.Unicode.GetBytes(jsoned);
        }
    }
}