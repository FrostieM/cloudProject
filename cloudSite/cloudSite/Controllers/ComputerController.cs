using Microsoft.AspNetCore.Mvc;

namespace cloudSite.Controllers
{
    public class ComputerController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}