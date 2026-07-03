using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebDatLichKham.Models;

namespace WebDatLichKham.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Booking()
        {
            return View();
        }

        public IActionResult BookingSuccess()
        {
            return View();
        }

        public IActionResult Lookup()
        {
            return View();
        }

        public IActionResult ExamResult()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
