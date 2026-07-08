using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalDoctors = await _context.Doctors.CountAsync();
            ViewBag.TotalPatients = await _context.Appointments.CountAsync();
            ViewBag.TotalSpecialties = await _context.Specialties.CountAsync();
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewBag.TotalDoctors = await _context.Doctors.CountAsync();
            ViewBag.TotalPatients = await _context.Appointments.CountAsync();
            ViewBag.TotalSpecialties = await _context.Specialties.CountAsync();
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

        public async Task<IActionResult> DoctorDetail(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            var doc = await _context.Doctors.FindAsync(id);
            if (doc == null) return RedirectToAction("Index");
            return View(doc);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
