using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleSheetLib
{
    public class Application
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public override string ToString()
        {
            var version = string.IsNullOrWhiteSpace(Version) ? "Unknown" : Version;
            return string.Format($"{Name} version: {version}");
        }
    }

    public class ComputerInfo
    {
        public string Name { get; set; }
        public int AppsNum { get => Apps == null ? 0 : Apps.Count(); }
        public DateTime? Date { get; set; }
        public IEnumerable<Application> Apps { get; set; }
    }

    public class SpreadsheetReader
    {
        
        private readonly AccessProvider accessProvider = new AccessProvider(AccessType.ServiceAccount);

        public ComputerInfo GetComputer(string computerName)
        {
            var sheetData = GetSheetData(computerName);
            var date = ToDateTime(GetDate(sheetData));
            var apps = GetApplications(sheetData);

            var comp = new ComputerInfo
            {
                Name = computerName,
                Date = date,
                Apps = apps
            };
            return comp;
        }

        public IEnumerable<ComputerInfo> GetComputers()
        {
            var compNames = accessProvider.GetSheetNames();

            var computersInfo = new List<ComputerInfo>();
            
            foreach (var name in compNames)
            {
                var comp = GetComputer(name);
                if (comp.Date != null)
                {
                    computersInfo.Add(comp);
                }
            }
            
            return computersInfo;
        }

        private IEnumerable<IEnumerable<string>> GetSheetData(string sheetName)
        {
            return accessProvider.ReadEntries(sheetName);
        }

        private IEnumerable<Application> GetApplications(IEnumerable<IEnumerable<string>> sheetData)
        {
            if (sheetData == null)
                return null;

            sheetData = sheetData.Skip(2);
            var applications = new List<Application>();

            foreach (var row in sheetData)
            {
                var app = new Application
                {
                    Name = row.ElementAtOrDefault(0),
                    Version = row.ElementAtOrDefault(1)
                };

                if(!string.IsNullOrWhiteSpace(app.Name))
                    applications.Add(app);
            }
            return applications;
        }
               
        private string GetDate(IEnumerable<IEnumerable<string>> sheetData)
        {
            return sheetData?.FirstOrDefault()?.LastOrDefault();
        }

        private DateTime? ToDateTime(string strSeconds)
        {
            if (string.IsNullOrWhiteSpace(strSeconds))
                return null;

            double seconds;
            if (double.TryParse(strSeconds, out seconds))
                return new DateTime(1970, 1, 1).AddSeconds(seconds).ToLocalTime();
            return null;
        }
    }
}
