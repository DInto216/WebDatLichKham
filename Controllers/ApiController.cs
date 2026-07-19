using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebDatLichKham.Models;
using WebDatLichKham.Services;

namespace WebDatLichKham.Controllers
{
    /// <summary>
    /// Bộ điều khiển trung tâm (Centralized API Controller) xử lý dữ liệu cho SPA.
    /// [BẢO MẬT]: 100% các endpoint tương tác CSDL đều dùng EF Core Parameterized LINQ, không nối chuỗi (No SqlRaw).
    /// Việc này đảm bảo ngăn chặn triệt để tấn công SQL Injection từ mọi form nhập liệu (Patient Form, Admin Form).
    /// </summary>
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
        // SPECIALTIES (Chuyên khoa)
        // ============================================

        [HttpGet("specialties")]
        public async Task<IActionResult> GetSpecialties()
        {
            var specialties = await _specialtyService.GetAllAsync();
            return Ok(specialties);
        }

        /// <summary>
        /// [AUTHORIZATION - RBAC]: Chỉ role "admin" mới có quyền Thêm/Sửa/Xóa chuyên khoa.
        /// Chặn các truy cập từ user (bệnh nhân) hoặc Bác sĩ để bảo vệ tính nhất quán dữ liệu cốt lõi.
        /// </summary>
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
            try
            {
                var result = await _doctorService.AddAsync(doctor);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // [QA FIX]: Trả về JSON lỗi thân thiện thay vì văng trang lỗi 500 HTML
                return StatusCode(500, new { message = "Lỗi hệ thống khi thêm bác sĩ: " + ex.Message });
            }
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
        // APPOINTMENTS (Lịch hẹn khám)
        // ============================================

        /// <summary>
        /// [AUTHORIZATION - RBAC]: Cả "admin" và "doctor" đều xem được lịch hẹn.
        /// Tuy nhiên:
        /// - Admin xem được tất cả lịch của toàn bệnh viện.
        /// - Doctor chỉ xem được lịch hẹn của ĐÚNG MÌNH. Hệ thống chặn chéo ID (Tenant Isolation) ngay trong hàm.
        /// </summary>
        [Authorize(Roles = "admin,doctor")]
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments([FromQuery] string? doctorId, [FromQuery] string? date)
        {
            // Kiểm tra phân lập dữ liệu (Data Isolation / Multi-tenancy check)
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
            // [QA FIX]: Model Validation tự động bắt lỗi nếu thiếu trường Required (Tên, SĐT, Ngày, Giờ)
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại." });
            }

            try
            {
                var result = await _appointmentService.AddAsync(data);
                return Ok(result);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                // [QA FIX]: Bọc Try-Catch để tránh trả về HTML lỗi 500 làm sập giao diện React/Fetch API
                return StatusCode(500, new { message = "Lỗi hệ thống khi đặt lịch: " + ex.Message });
            }
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
        public async Task<IActionResult> LookupAppointments([FromQuery] string? phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return BadRequest(new { message = "Thiếu số điện thoại" });
            }
            var list = await _appointmentService.LookupAsync(phone);
            if (list == null || !list.Any()) return NotFound(new { message = "Không tìm thấy lịch hẹn nào với số điện thoại này." });

            // Xắp xếp mới nhất lên đầu
            var sortedList = list.OrderByDescending(a => a.Date).ThenByDescending(a => a.Time).ToList();
            return Ok(sortedList);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _appointmentService.GetPaymentsAsync();
            return Ok(payments);
        }

        // ============================================
        // PAYMENT SETTINGS
        // ============================================
        [HttpGet("payments/settings")]
        public async Task<IActionResult> GetPaymentSettings()
        {
            var db = HttpContext.RequestServices.GetService(typeof(WebDatLichKham.Data.ApplicationDbContext)) as WebDatLichKham.Data.ApplicationDbContext;
            if (db == null) return StatusCode(500);
            var settings = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(db.PaymentSettings);
            return Ok(settings);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("payments/settings/{id}")]
        public async Task<IActionResult> UpdatePaymentSetting(int id, [FromBody] JsonElement data)
        {
            var db = HttpContext.RequestServices.GetService(typeof(WebDatLichKham.Data.ApplicationDbContext)) as WebDatLichKham.Data.ApplicationDbContext;
            if (db == null) return StatusCode(500);

            var setting = await db.PaymentSettings.FindAsync(id);
            if (setting == null) return NotFound();

            if (data.TryGetProperty("isActive", out JsonElement activeElement))
            {
                setting.IsActive = activeElement.GetBoolean();
            }
            if (data.TryGetProperty("additionalConfigJson", out JsonElement configElement))
            {
                setting.AdditionalConfigJson = configElement.ValueKind == JsonValueKind.Null ? null : configElement.GetString();
            }

            // [VALIDATION BẮT BUỘC]: Nếu kích hoạt (isActive = true) thì cấu hình không được phép trống (Trừ tiền mặt)
            if (setting.IsActive && setting.MethodCode != "cash")
            {
                if (string.IsNullOrWhiteSpace(setting.AdditionalConfigJson))
                {
                    return BadRequest(new { message = "Vui lòng nhập đầy đủ cấu hình trước khi kích hoạt phương thức thanh toán này!" });
                }

                try
                {
                    var configObj = System.Text.Json.JsonDocument.Parse(setting.AdditionalConfigJson).RootElement;

                    if (setting.MethodCode == "vnpay")
                    {
                        if (string.IsNullOrWhiteSpace(configObj.GetProperty("vnpTmnCode").GetString()) ||
                            string.IsNullOrWhiteSpace(configObj.GetProperty("vnpHashSecret").GetString()) ||
                            string.IsNullOrWhiteSpace(configObj.GetProperty("vnpUrl").GetString()))
                        {
                            return BadRequest(new { message = "Thiếu thông tin cấu hình VNPay (Mã Website, Khóa bí mật hoặc URL)." });
                        }
                    }
                    else if (setting.MethodCode == "momo")
                    {
                        if (string.IsNullOrWhiteSpace(configObj.GetProperty("partnerCode").GetString()) ||
                            string.IsNullOrWhiteSpace(configObj.GetProperty("accessKey").GetString()) ||
                            string.IsNullOrWhiteSpace(configObj.GetProperty("secretKey").GetString()))
                        {
                            return BadRequest(new { message = "Thiếu thông tin cấu hình MoMo (Partner Code, Access Key, Secret Key)." });
                        }
                    }
                    else if (setting.MethodCode == "bank_transfer")
                    {
                        if (string.IsNullOrWhiteSpace(configObj.GetProperty("bankName").GetString()) ||
                            string.IsNullOrWhiteSpace(configObj.GetProperty("accountNumber").GetString()) ||
                            string.IsNullOrWhiteSpace(configObj.GetProperty("accountName").GetString()))
                        {
                            return BadRequest(new { message = "Thiếu thông tin Ngân hàng chuyển khoản (Tên Ngân hàng, Số tài khoản, Chủ tài khoản)." });
                        }
                    }
                }
                catch (Exception)
                {
                    return BadRequest(new { message = "Định dạng cấu hình không hợp lệ." });
                }
            }

            setting.UpdatedAt = DateTime.UtcNow;

            _ = await db.SaveChangesAsync();
            return Ok(setting);
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
        // ============================================
        // UPLOAD LUỒNG TỐI ƯU (Tối ưu hóa File Storage)
        // ============================================

        /// <summary>
        /// Luồng Tối Ưu: Nhận file ảnh qua Form-Data, lưu trực tiếp vào ổ cứng Server (wwwroot/uploads/avatars)
        /// và chỉ trả về Đường dẫn tương đối (Relative Path) để lưu vào SQL Server.
        /// Chống phình to Database so với việc lưu chuỗi Base64.
        /// </summary>
        [Authorize(Roles = "admin,doctor")]
        [HttpPost("upload/avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] Microsoft.AspNetCore.Http.IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Vui lòng chọn file ảnh hợp lệ." });

                // Kiểm tra định dạng ảnh
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (Array.IndexOf(allowedExtensions, ext) < 0)
                    return BadRequest(new { message = "Chỉ hỗ trợ định dạng JPG, JPEG, PNG, GIF." });

                // Tạo thư mục nếu chưa có
                var uploadsFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!System.IO.Directory.Exists(uploadsFolder))
                    _ = System.IO.Directory.CreateDirectory(uploadsFolder);

                // Lưu file với tên ngẫu nhiên
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về đường dẫn tương đối (sẽ được lưu vào cột Avatar trong bảng Accounts)
                var relativePath = $"/uploads/avatars/{uniqueFileName}";
                return Ok(new { url = relativePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải ảnh: " + ex.Message });
            }
        }
    }
}
