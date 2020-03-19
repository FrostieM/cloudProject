using System.Collections;
using System.Linq;
using cloudSite.ViewData;
using GoogleSheetLib;
using Microsoft.AspNetCore.Mvc;

namespace cloudSite.Controllers
{
    [Route("api/[controller]")]
    public class ComputerController : ControllerBase
    {

        private readonly SpreadsheetReader _reader = new SpreadsheetReader();
        private int _maxPageSize = 10;

        [HttpGet]
        public IEnumerable GetComputers()
        {
            return _reader.GetComputers();
        }
        
        [Route("getCompsByProgram")]
        [HttpGet]
        public IEnumerable GetComputersByProgram(string programName)
        {
            return _reader.GetComputers()
                .Where(comp 
                    => comp.Apps.Any(app => app.Name.ToLower().Contains(programName.ToLower())));
        }

        [Route("computerInfo")]
        [HttpGet]
        public ComputerViewData GetComputerInfo(string computerName, int page = 1)
        {
            var computerInfo = _reader.GetComputer(computerName);
            return new ComputerViewData
            {
                ComputerInfo = new ComputerInfo
                {
                    Date = computerInfo.Date,
                    Name = computerInfo.Name,
                    Apps = computerInfo.Apps.Skip(_maxPageSize * (page - 1)).Take(_maxPageSize)
                },
                Pagination = new PaginationViewData
                {
                    CurrentPage = page,
                    PageSize = _maxPageSize,
                    TotalItems = computerInfo.Apps.Count()
                }
            };
        }
    }
}