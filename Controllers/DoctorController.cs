using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebDatLichKham.Controllers
{
    [Authorize(Roles = "doctor")]
    public class DoctorController : Controller
    {
        public IActionResult Examinations()
        {
            return View();
        }

        public IActionResult Schedule()
        {
            return View();
        }

        public IActionResult MedicalHistory()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult CreateRecord()
        {
            return View();
        }
    }
}

