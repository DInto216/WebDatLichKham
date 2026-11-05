using Microsoft.AspNetCore.Mvc;

namespace WebDatLichKham.Controllers
{
    public class AccountController : Controller
    {
        private readonly WebDatLichKham.Data.ApplicationDbContext _context;

        public AccountController(WebDatLichKham.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            ViewBag.DoctorCount = _context.Doctors.Count();
            ViewBag.AppointmentCount = _context.Appointments.Count();
            ViewBag.SpecialtyCount = _context.Specialties.Count();
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}

