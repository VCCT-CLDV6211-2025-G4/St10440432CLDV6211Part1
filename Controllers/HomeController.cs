//st10440432
//Matteo Nusca

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingSystemCLVD.Models;

namespace BookingSystemCLVD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Show homepage
        public IActionResult Index()
        {
            return View();
        }

        // Show privacy policy
        public IActionResult Privacy()
        {
            return View();
        }

        // Show error page if something goes wrong
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
