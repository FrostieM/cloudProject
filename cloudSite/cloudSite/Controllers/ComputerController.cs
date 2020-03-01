using System;
using System.Collections;
using cloudSite.ViewData;
using Microsoft.AspNetCore.Mvc;

namespace cloudSite.Controllers
{
    [Route("api/[controller]")]
    public class ComputerController : ControllerBase
    {
    
        [HttpGet]
        public IEnumerable GetComputers()
        {
            return new[]
            {
                new ComputerViewData{ Name = "123.214.152.215", Programs = 5},
                new ComputerViewData{ Name = "325.23.235.234", Programs = 2},
                new ComputerViewData{ Name = "235.235.234.532", Programs = 3},
                new ComputerViewData{ Name = "234.425.65.434", Programs = 10},
                new ComputerViewData{ Name = "132.12.422.24", Programs = 12},
                new ComputerViewData{ Name = "143.53.24.42", Programs = 42},
            };
        }
        
        [Route("computerInfo")]
        [HttpGet]
        public IEnumerable GetComputerInfo(string computerName)
        {
            Console.WriteLine(computerName);
            return new[] { "cs go", "dota", "fortnite", "dauntless", "vs code", "firefox" };
        }
    }
}