using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClassLib
{
    public enum AppStatus
    {
        Installed,
        Updated,
        Deleted
    }
    [Serializable]
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

        public override int GetHashCode() => $"{name} {version}".GetHashCode();
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var p = (App)obj;
            return name == p.name && version == p.version;
        }
    }
    [Serializable]
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
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public static AppsContainer FromBytes(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
                return (AppsContainer) formatter.Deserialize(stream);
        }

        public static AppsContainer FromStream(NetworkStream stream)
        {
            var data = new byte[1024];
            using (var ms = new MemoryStream())
            {
                int count;
                while ((count = stream.Read(data, 0, data.Length)) > 0) 
                    ms.Write(data, 0, count);
                return FromBytes(ms.ToArray());
            }
        }
    }
}