using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cloudSite.ViewData;
using GoogleSheetLib;
using Microsoft.AspNetCore.Mvc;

namespace cloudSite.Controllers
{
    [Route("api/[controller]")]
    public class ComputerController : ControllerBase
    {
        
        private static readonly SpreadsheetReader Reader = new SpreadsheetReader();
        private static readonly ComputerAccess Computer = new ComputerAccess();
        
        private int _maxPageSize = 10;
        

        class ComputerAccess
        {
            private List<ComputerInfo> Computers { get; set; } = new List<ComputerInfo>();
            private DateTime? Date { get; set; }
            
            public IEnumerable<ComputerInfo> GetComputers(SpreadsheetReader reader)
            {
                var now = DateTime.Now;
                if (Date != null && Date.Value.AddMinutes(2) >= now) return Computers;
                
                Computers.Clear();
                Date = now;
                Computers = reader.GetComputers().ToList();

                return Computers;
            }
        }
        
        [HttpGet]
        public IEnumerable GetComputers()
        {
            return Computer.GetComputers(Reader);
        }
        
        [HttpGet, Route("getCompsByProgram")]
        public IEnumerable GetComputersByProgram(string programName)
        {
            if (programName == null) return Computer.GetComputers(Reader);
            
            return Computer.GetComputers(Reader)
                .Where(comp 
                    => comp.Apps.Any(app => app.Name.ToLower().Contains(programName.ToLower())));
        }
        
        [HttpGet, Route("computerInfo")]
        public ComputerViewData GetComputerInfo(string computerName, int page = 1)
        {
            var computerInfo = Computer.GetComputers(Reader)
                .Single(comp => comp.Name == computerName);
            
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