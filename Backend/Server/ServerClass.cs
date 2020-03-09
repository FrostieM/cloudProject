using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ClassLib;
using GoogleSheetLib;
using Newtonsoft.Json.Linq;

using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Net;

namespace Server
{
    class ServerClass
    {
        private static readonly object Locker = new object();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();


        private static JObject config;
             
        static void Main(string[] args)
        {
            
            config = JObject.Parse(File.ReadAllText("server_config.json"));
            var tray = config["tray"].ToObject<bool>();
            ShowWindow(GetConsoleWindow(), tray ? 0 : 1);
            if(tray) new NotifyIcon { Icon = new Icon("icon.ico"), Visible = true };
            var startDB = config["clearDB"].ToObject<bool>();
            if (startDB) dbWork.ClearDB();
            var port = config["port"].ToObject<int>();
            var callback = new Action<TcpClient>(client =>
            {
                var ip = ((IPEndPoint)client.Client.LocalEndPoint).Address;
                Console.WriteLine($"New connection {ip}");
                try
                {
                    NetworkStream stream = null;
                    try
                    {
                        stream = client.GetStream();
                        var container = AppsContainer.FromStream(stream);
                        dbWork.UpdateDB(container);
                        SheetsUpdater(dbWork.GetOneComputerData(container.hostname));
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                    finally { stream?.Close(); Console.WriteLine($"Stream {ip} closed!"); }
                }
                catch (Exception e) { Console.WriteLine(e); }
                finally { client.Close(); Console.WriteLine($"Connection {ip} closed!"); }
            });
            SheetsUpdater(dbWork.GetAllData());
            Console.WriteLine("Start listening...");
            Network.TcpListen(port, callback);
        }

        private static void SheetsUpdater(List<AppsContainer> container)
        {
            lock (Locker)
            {
                if (container.Count == 0) return;
                var accessProvider = new AccessProvider(AccessType.ServiceAccount);
                var data = new List<IList<object>>();
                foreach (var comp in container)
                {
                    data.Add(new List<object> {comp.hostname, comp.date.ToString()});
                    data.Add(new List<object>());
                    foreach (var app in comp.apps) data.Add(new List<object> {app.name, app.version});
                    accessProvider.WriteData(data, comp.hostname);
                }
            }
        }
    }
}
