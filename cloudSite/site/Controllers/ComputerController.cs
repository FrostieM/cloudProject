using System.Collections;
using GoogleSheetAccessProviderLib;
using Microsoft.AspNetCore.Mvc;

namespace cloudSite.Controllers
{
    [Route("api/[controller]")]
    public class ComputerController : ControllerBase
    {

        private readonly SpreadsheetReader _reader = new SpreadsheetReader();
        
        [HttpGet]
        public IEnumerable GetComputers()
        {
            return _reader.GetComputers();
        }
        
        [Route("computerInfo")]
        [HttpGet]
        public ComputerInfo GetComputerInfo(string computerName)
        {
            return _reader.GetComputer(computerName);
        }
    }
}