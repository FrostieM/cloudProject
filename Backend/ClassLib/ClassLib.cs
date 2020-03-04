using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
            return Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(this));
        }

        public static AppsContainer FromStream(NetworkStream stream)
        {
            var data = new byte[1024];
            var builder = new StringBuilder();
            do
            {
                var bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (stream.DataAvailable);

            return JsonConvert.DeserializeObject<AppsContainer>(builder.ToString());
        }
    }
}