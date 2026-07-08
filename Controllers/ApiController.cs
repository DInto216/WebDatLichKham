using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebDatLichKham.Models;
using WebDatLichKham.Services;

namespace WebDatLichKham.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly IMedicineService _medicineService;
        private readonly IDashboardService _dashboardService;
        private readonly IAccountService _accountService;
        private readonly IMedicalHistoryService _medicalHistoryService;
        private readonly IDoctorReviewService _doctorReviewService;

        public ApiController(
            ISpecialtyService specialtyService,
            IDoctorService doctorService,
            IAppointmentService appointmentService,
            IMedicineService medicineService,
            IDashboardService dashboardService,
            IAccountService accountService,
            IMedicalHistoryService medicalHistoryService,
            IDoctorReviewService doctorReviewService)
        {
            _specialtyService = specialtyService;
            _doctorService = doctorService;
            _appointmentService = appointmentService;
            _medicineService = medicineService;
            _dashboardService = dashboardService;
            _accountService = accountService;
            _medicalHistoryService = medicalHistoryService;
            _doctorReviewService = doctorReviewService;
        }

        // ============================================
        // SPECIALTIES
        // ============================================
        [HttpGet("specialties")]
        public async Task<IActionResult> GetSpecialties()
        {
            var specialties = await _specialtyService.GetAllAsync();
            return Ok(specialties);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("specialties")]
        public async Task<IActionResult> AddSpecialty([FromBody] Specialty specialty)
        {
            var result = await _specialtyService.AddAsync(specialty);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("specialties/{id}")]
        public async Task<IActionResult> UpdateSpecialty(string id, [FromBody] Specialty data)
        {
            var result = await _specialtyService.UpdateAsync(id, data);
            if (result == null) return NotFound(new { message = "Không tìm thấy chuyên khoa" });
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("specialties/{id}")]
        public async Task<IActionResult> DeleteSpecialty(string id)
        {
            var result = await _specialtyService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy chuyên khoa" });
            return Ok(true);
        }

        // ============================================
        // DOCTORS
        // ============================================
        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("doctors/{id}")]
        public async Task<IActionResult> GetDoctor(string id)
        {
            var doc = await _doctorService.GetByIdAsync(id);
            if (doc == null) return NotFound(new { message = "Không tìm thấy bác sĩ" });
            return Ok(doc);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("doctors")]
        public async Task<IActionResult> AddDoctor([FromBody] Doctor doctor)
        {
            var result = await _doctorService.AddAsync(doctor);
            return Ok(result);
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpPut("doctors/{id}")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] JsonElement data)
        {
            var result = await _doctorService.UpdateAsync(id, data);
            if (result == null) return NotFound(new { message = "Không tìm thấy bác sĩ" });
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("doctors/{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var result = await _doctorService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy bác sĩ" });
            return Ok(true);
        }

        [HttpGet("doctors/{id}/slots")]
        public async Task<IActionResult> GetDoctorSlots(string id, [FromQuery] string date)
        {
            var slots = await _doctorService.GetDoctorSlotsAsync(id, date);
            return Ok(slots);
        }

        // ============================================
        // APPOINTMENTS
        // ============================================
        [Authorize(Roles = "admin,doctor")]
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments([FromQuery] string? doctorId, [FromQuery] string? date)
        {
            if (User.IsInRole("doctor") && !User.IsInRole("admin"))
            {
                var claimDoctorId = User.FindFirst("DoctorId")?.Value;
                if (string.IsNullOrEmpty(claimDoctorId))
                {
                    return Forbid();
                }
                doctorId = claimDoctorId;
            }
            var appts = await _appointmentService.GetAllAsync(doctorId, date);
            return Ok(appts);
        }

        [HttpPost("appointments")]
        public async Task<IActionResult> AddAppointment([FromBody] Appointment data)
        {
            var result = await _appointmentService.AddAsync(data);
            return Ok(result);
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpPut("appointments/{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] JsonElement data)
        {
            string status = data.GetProperty("status").GetString() ?? "pending";
            var result = await _appointmentService.UpdateStatusAsync(id, status);
            if (result == null) return NotFound(new { message = "Không tìm thấy lịch hẹn" });
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("appointments/{id}/payment")]
        public async Task<IActionResult> UpdatePayment(string id, [FromBody] JsonElement data)
        {
            string status = data.GetProperty("status").GetString() ?? "unpaid";
            var result = await _appointmentService.UpdatePaymentAsync(id, status);
            if (result == null) return NotFound(new { message = "Không tìm thấy lịch hẹn" });
            return Ok(result);
        }

        [HttpGet("appointments/lookup")]
        public async Task<IActionResult> LookupAppointments([FromQuery] string? phone, [FromQuery] string? bookingId)
        {
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(bookingId))
            {
                return BadRequest(new { message = "Thiếu số điện thoại hoặc mã đặt lịch" });
            }
            var list = await _appointmentService.LookupAsync(phone);
            var appt = list.FirstOrDefault(a => a.BookingId.Equals(bookingId.Trim(), StringComparison.OrdinalIgnoreCase));
            if (appt == null) return NotFound(new { message = "Không tìm thấy lịch hẹn" });
            return Ok(appt);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _appointmentService.GetPaymentsAsync();
            return Ok(payments);
        }

        // ============================================
        // ACCOUNTS
        // ============================================
        [Authorize(Roles = "admin")]
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("accounts")]
        public async Task<IActionResult> AddAccount([FromBody] Account acc)
        {
            var result = await _accountService.AddAsync(acc);
            return Ok(new { result.Id, result.Username, result.Role, result.Name, result.DoctorId, avatar = result.Avatar ?? "" });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("accounts/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var result = await _accountService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy tài khoản" });
            return Ok(true);
        }

        [Authorize]
        [HttpPut("accounts/{id}/profile")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] JsonElement data)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!User.IsInRole("admin") && id != currentUserId)
            {
                return Forbid();
            }

            try
            {
                var result = await _accountService.UpdateProfileAsync(id, data);
                if (result == null) return NotFound(new { message = "Không tìm thấy tài khoản" });
                return Ok(new { result.Id, result.Username, result.Role, result.Name, result.DoctorId, avatar = result.Avatar ?? "" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================
        // AUTH
        // ============================================
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] JsonElement creds)
        {
            string username = creds.GetProperty("username").GetString() ?? "";
            string password = creds.GetProperty("password").GetString() ?? "";

            var account = await _accountService.AuthenticateAsync(username, password);
            if (account == null) return BadRequest(new { message = "Sai tên đăng nhập hoặc mật khẩu" });

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, account.Id),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, account.Username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, account.Role),
                new System.Security.Claims.Claim("FullName", account.Name ?? ""),
            };
            if (!string.IsNullOrEmpty(account.DoctorId))
            {
                claims.Add(new System.Security.Claims.Claim("DoctorId", account.DoctorId));
            }

            var identity = new System.Security.Claims.ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new
            {
                id = account.Id,
                username = account.Username,
                role = account.Role,
                name = account.Name,
                doctorId = account.DoctorId,
                avatar = account.Avatar ?? ""
            });
        }

        [HttpPost("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true });
        }

        [HttpGet("auth/me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                string avatarStr = "";
                if (!string.IsNullOrEmpty(id))
                {
                    var acc = await _accountService.GetAccountByIdAsync(id);
                    if (acc != null)
                    {
                        avatarStr = acc.Avatar ?? "";
                    }
                }

                return Ok(new
                {
                    id = id,
                    username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value,
                    role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
                    name = User.FindFirst("FullName")?.Value,
                    doctorId = User.FindFirst("DoctorId")?.Value,
                    avatar = avatarStr
                });
            }
            return Unauthorized();
        }

        // ============================================
        // MEDICINES
        // ============================================
        [HttpGet("medicines")]
        public async Task<IActionResult> GetMedicines()
        {
            var meds = await _medicineService.GetAllAsync();
            return Ok(meds);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("medicines")]
        public async Task<IActionResult> AddMedicine([FromBody] Medicine med)
        {
            var result = await _medicineService.AddAsync(med);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("medicines/{id}")]
        public async Task<IActionResult> UpdateMedicine(string id, [FromBody] Medicine data)
        {
            var result = await _medicineService.UpdateAsync(id, data);
            if (result == null) return NotFound(new { message = "Không tìm thấy thuốc" });
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("medicines/{id}")]
        public async Task<IActionResult> DeleteMedicine(string id)
        {
            var result = await _medicineService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy thuốc" });
            return Ok(true);
        }

        // ============================================
        // MEDICAL HISTORY
        // ============================================
        [Authorize(Roles = "admin,doctor")]
        [HttpGet("medical-history")]
        public async Task<IActionResult> SearchMedicalHistory([FromQuery] string? phone)
        {
            var result = await _medicalHistoryService.SearchAsync(phone);
            return Ok(result);
        }

        [Authorize(Roles = "doctor")]
        [HttpPost("medical-history")]
        public async Task<IActionResult> AddMedicalHistory([FromBody] JsonElement rawData)
        {
            var result = await _medicalHistoryService.AddAsync(rawData);
            return Ok(result);
        }

        [HttpGet("medical-history/result")]
        public async Task<IActionResult> GetMedicalHistoryResult([FromQuery] string? bookingId, [FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var result = await _medicalHistoryService.GetByIdAsync(id.Value);
                if (result == null) return NotFound(new { message = "Không tìm thấy hồ sơ bệnh án" });
                return Ok(result);
            }
            else if (!string.IsNullOrEmpty(bookingId))
            {
                var result = await _medicalHistoryService.GetResultAsync(bookingId);
                if (result == null) return NotFound(new { message = "Không tìm thấy hồ sơ bệnh án" });
                return Ok(result);
            }
            return BadRequest(new { message = "Thiếu thông tin tra cứu" });
        }

        // ============================================
        // DASHBOARD
        // ============================================
        [Authorize(Roles = "admin,doctor")]
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _dashboardService.GetStatsAsync();
            return Ok(stats);
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpGet("dashboard/revenue")]
        public async Task<IActionResult> GetRevenueStats([FromQuery] int year)
        {
            var stats = await _dashboardService.GetRevenueStatsAsync(year);
            return Ok(stats);
        }

        // ============================================
        // DOCTOR REVIEWS
        // ============================================
        [HttpGet("doctor-reviews")]
        public async Task<IActionResult> GetDoctorReviews()
        {
            var reviews = await _doctorReviewService.GetAllAsync();
            return Ok(reviews);
        }

        [HttpPost("doctor-reviews")]
        public async Task<IActionResult> AddDoctorReview([FromBody] DoctorReview review)
        {
            var result = await _doctorReviewService.AddAsync(review);
            return Ok(result);
        }
    }
}
