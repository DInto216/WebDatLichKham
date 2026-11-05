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

        /// <summary>
        /// Giao diện Trang chủ (Landing Page) dành cho Bệnh nhân.
        /// Sử dụng Async/Await để tránh nghẽn luồng (thread starvation) khi truy vấn thống kê từ Database.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Tối ưu hóa: Thay vì Count toàn bộ bảng, EF Core CountAsync() sẽ dịch thẳng ra câu lệnh "SELECT COUNT(*)" ở dưới DB.
            // Thêm số liệu ảo (offset) để giao diện trông thực tế hơn + số liệu thật từ DB
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

        /// <summary>
        /// Luồng: Đặt lịch khám bệnh. Trả về giao diện Wizard (Booking.cshtml)
        /// Form nhập liệu sẽ gọi API bằng JS thay vì PostBack truyền thống.
        /// </summary>
        public IActionResult Booking()
        {
            return View();
        }

        public IActionResult BookingSuccess()
        {
            return View();
        }

        /// <summary>
        /// Luồng: Tra cứu lịch hẹn của bệnh nhân
        /// </summary>
        public IActionResult Lookup()
        {
            return View();
        }

        /// <summary>
        /// Luồng: Xem kết quả khám và đơn thuốc điện tử
        /// </summary>
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

