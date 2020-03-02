using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSheetAccessProviderLib
{
    public class ComputerInfo
    {
        public string Name { get; set; }
        public int AppsNum  => Apps.Count; 
        public DateTime Date { get; set; }
        public List<string> Apps { get; set; }
    }

    public class SpreadsheetReader
    {
        private readonly AccessProvider accessProvider = new AccessProvider("GoogleSheetAccessProvider", "1LrsHlVFmjkWU3vV6HOH1cj25jsxAUyUYYCZ-wNPoeD8");

        public ComputerInfo GetComputer(string computerName)
        {
            var sheetData = GetSheetData(computerName);
            var date = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(GetDate(sheetData)));
            var apps = GetApplications(sheetData).ToList();

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
                computersInfo.Add(comp);
            }
            return computersInfo;
        }

        private IEnumerable<IEnumerable<string>> GetSheetData(string sheetName)
        {
            return accessProvider.ReadEntries(sheetName);
        }

        private IEnumerable<string> GetApplications(IEnumerable<IEnumerable<string>> sheetData)
        {
            if (sheetData.ToList().Count == 0)
                return null;

            sheetData.Skip(2);
            var result = new List<string>();

            foreach (var row in sheetData)
            {                
                string appData = null;
                foreach (var cell in row)
                    appData += cell + " ";
                
                result.Add(appData);
            }

            return result;
        }
               
        private string GetDate(IEnumerable<IEnumerable<string>> sheetData)
        {
            if (sheetData.ToList().Count == 0)
                return null;
            
            return sheetData.First().Last();
        }   
    }
}
