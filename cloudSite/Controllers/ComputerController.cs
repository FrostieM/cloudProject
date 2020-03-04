using System;
using System.Collections;
using System.Linq;
using cloudSite.ViewData;
using GoogleSheetAccessProviderLib;
using Microsoft.AspNetCore.Mvc;

namespace cloudSite.Controllers
{
    [Route("api/[controller]")]
    public class ComputerController : ControllerBase
    {

        private readonly SpreadsheetReader _reader = new SpreadsheetReader();
        private readonly int _pageSize = 10;
        
        [HttpGet]
        public IEnumerable GetComputers()
        {
            return _reader.GetComputers();
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
                    Apps = computerInfo.Apps.Skip(_pageSize * (page - 1)).Take(_pageSize).ToList()
                },
                Pagination = new PaginationViewData
                {
                    TotalItems = computerInfo.Apps.Count,
                    CurrentPage = page,
                    PageSize = _pageSize
                }
            };
        }
    }
}