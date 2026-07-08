using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebDatLichKham.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Accounts()
        {
            return View();
        }

        public IActionResult Appointments()
        {
            return View();
        }


        public IActionResult MedicalHistory()
        {
            return View();
        }

        public IActionResult Medicines()
        {
            return View();
        }

        public IActionResult Specialties()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Payments()
        {
            return View();
        }
    }
}
