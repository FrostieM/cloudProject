using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using ClassLib;
using GoogleSheetAccessProviderLib;
using Newtonsoft.Json;
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
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();


        private static JObject config;
             
        static void Main(string[] args)
        {
            
            ServerClass.config = JObject.Parse(File.ReadAllText("config.json"));
            var tray = config["tray"].ToObject<bool>();
            if (tray == true)
            {
                ShowWindow(GetConsoleWindow(), 0);
                var icon = new NotifyIcon();
                icon.Icon = new Icon("icon.ico");
                icon.Visible = true;
            }
            else
            {
                ShowWindow(GetConsoleWindow(), 1); 
            }
            var startDB = config["clearDB"].ToObject<bool>();
            if (startDB == true)
            {
                dbWork.ClearDB();
            }
            var port = config["port"].ToObject<int>();

            var callback = new Action<TcpClient>(client =>
             {
                 Console.WriteLine("New connection "+ ((IPEndPoint)client.Client.LocalEndPoint).Address.ToString());
                 NetworkStream stream = null;
                 try
                 {
                     stream = client.GetStream();
                     var data = new byte[1024];
                     var builder = new StringBuilder();
                     do
                     {
                         var bytes = stream.Read(data, 0, data.Length);
                         builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                     } while (stream.DataAvailable);

                     var container = JsonConvert.DeserializeObject<AppsContainer>(builder.ToString());
                     dbWork.UpdateDB(container);

                     SheetsUpdater(dbWork.GetAllData());
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine(ex.Message);
                 }
                 finally
                 {
                     Console.WriteLine("Connection " + ((IPEndPoint)client.Client.LocalEndPoint).Address.ToString()+" closed");
                     stream?.Close();
                     client?.Close();

                 }
             });

            SheetsUpdater(dbWork.GetAllData());
            Network.TcpListen(port, callback);
            
        }
       public static void SheetsUpdater(List<AppsContainer> container)
        {
            if (container.Count == 0)
                return;

            AccessProvider accessProvider = new AccessProvider("GoogleSheetAccessProvider", "1LrsHlVFmjkWU3vV6HOH1cj25jsxAUyUYYCZ-wNPoeD8");

            List<IList<object>> data = new List<IList<object>>();
            foreach (var comp in container)
            {
                data.Add(new List<object> { comp.hostname, comp.date.ToString() });
                data.Add(new List<object>());
                foreach (var app in comp.apps)
                {
                    data.Add(new List<object> { app.name, app.version });
                }
                accessProvider.WriteData(data, comp.hostname);
            }
        }
    }
}
