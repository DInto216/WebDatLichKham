using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================
        // SPECIALTIES
        // ============================================
        [HttpGet("specialties")]
        public async Task<IActionResult> GetSpecialties()
        {
            var specialties = await _context.Specialties.ToListAsync();
            return Ok(specialties);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("specialties")]
        public async Task<IActionResult> AddSpecialty([FromBody] Specialty specialty)
        {
            specialty.Id = "sp_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            specialty.DoctorCount = 0;
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();
            return Ok(specialty);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("specialties/{id}")]
        public async Task<IActionResult> UpdateSpecialty(string id, [FromBody] Specialty data)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null) return NotFound(new { message = "Không tìm thấy chuyên khoa" });

            specialty.Name = data.Name;
            specialty.Icon = data.Icon;
            specialty.Description = data.Description;

            await _context.SaveChangesAsync();
            return Ok(specialty);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("specialties/{id}")]
        public async Task<IActionResult> DeleteSpecialty(string id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null) return NotFound(new { message = "Không tìm thấy chuyên khoa" });

            var hasDoctors = await _context.Doctors.AnyAsync(d => d.SpecialtyId == id);
            if (hasDoctors)
            {
                return BadRequest(new { message = "Không thể xóa chuyên khoa này vì đang có bác sĩ hoạt động." });
            }

            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        // ============================================
        // DOCTORS
        // ============================================
        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors([FromQuery] string? specialtyId, [FromQuery] string? search)
        {
            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrEmpty(specialtyId))
            {
                query = query.Where(d => d.SpecialtyId == specialtyId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var q = search.ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(q) || d.SpecialtyName.ToLower().Contains(q));
            }

            var doctors = await query.ToListAsync();

            // Deserialize schedule JSON to object so JS receives object, not double-encoded string
            var result = doctors.Select(d => new
            {
                d.Id,
                d.Name,
                d.Degree,
                d.SpecialtyId,
                specialty = d.SpecialtyName,
                d.Price,
                d.Rating,
                d.Reviews,
                d.Avatar,
                d.Bio,
                schedule = string.IsNullOrEmpty(d.ScheduleJson) ? new Dictionary<string, List<string>>() : JsonSerializer.Deserialize<Dictionary<string, List<string>>>(d.ScheduleJson)
            });

            return Ok(result);
        }

        [HttpGet("doctors/{id}")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var d = await _context.Doctors.FindAsync(id);
            if (d == null) return NotFound();

            var result = new
            {
                d.Id,
                d.Name,
                d.Degree,
                d.SpecialtyId,
                specialty = d.SpecialtyName,
                d.Price,
                d.Rating,
                d.Reviews,
                d.Avatar,
                d.Bio,
                schedule = string.IsNullOrEmpty(d.ScheduleJson) ? new Dictionary<string, List<string>>() : JsonSerializer.Deserialize<Dictionary<string, List<string>>>(d.ScheduleJson)
            };

            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("doctors")]
        public async Task<IActionResult> AddDoctor([FromBody] JsonElement rawData)
        {
            var doctor = new Doctor
            {
                Id = "doc_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                Rating = 5.0,
                Reviews = 0,
                Avatar = "",
                ScheduleJson = "{\"monday\":[],\"tuesday\":[],\"wednesday\":[],\"thursday\":[],\"friday\":[],\"saturday\":[],\"sunday\":[]}"
            };

            if (rawData.TryGetProperty("name", out var nameProp)) doctor.Name = nameProp.GetString() ?? "";
            if (rawData.TryGetProperty("degree", out var degProp)) doctor.Degree = degProp.GetString() ?? "";
            if (rawData.TryGetProperty("specialtyId", out var spIdProp)) doctor.SpecialtyId = spIdProp.GetString() ?? "";
            if (rawData.TryGetProperty("specialty", out var spNameProp)) doctor.SpecialtyName = spNameProp.GetString() ?? "";
            if (rawData.TryGetProperty("price", out var priceProp)) doctor.Price = priceProp.GetDecimal();
            if (rawData.TryGetProperty("bio", out var bioProp)) doctor.Bio = bioProp.GetString() ?? "";
            if (rawData.TryGetProperty("schedule", out var schedProp)) doctor.ScheduleJson = schedProp.ToString();

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            // Update DoctorCount on Specialty
            var specialty = await _context.Specialties.FindAsync(doctor.SpecialtyId);
            if (specialty != null)
            {
                specialty.DoctorCount = await _context.Doctors.CountAsync(d => d.SpecialtyId == doctor.SpecialtyId);
                await _context.SaveChangesAsync();
            }

            return Ok(doctor);
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpPut("doctors/{id}")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] JsonElement rawData)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound(new { message = "Không tìm thấy bác sĩ" });

            var oldSpecialtyId = doctor.SpecialtyId;

            if (rawData.TryGetProperty("name", out var nameProp)) doctor.Name = nameProp.GetString() ?? "";
            if (rawData.TryGetProperty("degree", out var degProp)) doctor.Degree = degProp.GetString() ?? "";
            if (rawData.TryGetProperty("specialtyId", out var spIdProp)) doctor.SpecialtyId = spIdProp.GetString() ?? "";
            if (rawData.TryGetProperty("specialty", out var spNameProp)) doctor.SpecialtyName = spNameProp.GetString() ?? "";
            if (rawData.TryGetProperty("price", out var priceProp)) doctor.Price = priceProp.GetDecimal();
            if (rawData.TryGetProperty("bio", out var bioProp)) doctor.Bio = bioProp.GetString() ?? "";
            if (rawData.TryGetProperty("schedule", out var schedProp)) doctor.ScheduleJson = schedProp.ToString();
            if (rawData.TryGetProperty("rating", out var ratingProp)) doctor.Rating = ratingProp.GetDouble();
            if (rawData.TryGetProperty("reviews", out var revProp)) doctor.Reviews = revProp.GetInt32();
            if (rawData.TryGetProperty("avatar", out var avatarProp)) doctor.Avatar = avatarProp.GetString() ?? "";

            await _context.SaveChangesAsync();

            // Update DoctorCount for old and new specialties if changed
            if (oldSpecialtyId != doctor.SpecialtyId)
            {
                if (!string.IsNullOrEmpty(oldSpecialtyId))
                {
                    var oldSpec = await _context.Specialties.FindAsync(oldSpecialtyId);
                    if (oldSpec != null)
                    {
                        oldSpec.DoctorCount = await _context.Doctors.CountAsync(d => d.SpecialtyId == oldSpecialtyId);
                    }
                }
                if (!string.IsNullOrEmpty(doctor.SpecialtyId))
                {
                    var newSpec = await _context.Specialties.FindAsync(doctor.SpecialtyId);
                    if (newSpec != null)
                    {
                        newSpec.DoctorCount = await _context.Doctors.CountAsync(d => d.SpecialtyId == doctor.SpecialtyId);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return Ok(doctor);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("doctors/{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound(new { message = "Không tìm thấy bác sĩ" });

            var specialtyId = doctor.SpecialtyId;

            // Delete associated login account
            var associatedAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.DoctorId == id);
            if (associatedAccount != null)
            {
                _context.Accounts.Remove(associatedAccount);
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            // Fix #4: Cập nhật DoctorCount khi xóa bác sĩ
            if (!string.IsNullOrEmpty(specialtyId))
            {
                var specialty = await _context.Specialties.FindAsync(specialtyId);
                if (specialty != null)
                {
                    specialty.DoctorCount = await _context.Doctors.CountAsync(d => d.SpecialtyId == specialtyId);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(true);
        }

        [HttpGet("doctors/{id}/slots")]
        public async Task<IActionResult> GetAvailableSlots(string id, [FromQuery] string date)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return Ok(new List<object>());

            var dayOfWeek = DateTime.Parse(date).DayOfWeek.ToString().ToLower();
            
            Dictionary<string, List<string>> scheduleMap = new();
            if (!string.IsNullOrEmpty(doctor.ScheduleJson))
            {
                scheduleMap = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(doctor.ScheduleJson) ?? new();
            }

            var allSlots = scheduleMap.ContainsKey(dayOfWeek) ? scheduleMap[dayOfWeek] : new List<string>();

            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == id && a.Date == date && a.Status != "cancelled")
                .Select(a => a.Time)
                .ToListAsync();

            var result = allSlots.Select(slot => new
            {
                time = slot,
                available = !bookedSlots.Contains(slot)
            });

            return Ok(result);
        }

        // ============================================
        // APPOINTMENTS
        // ============================================
        [Authorize(Roles = "admin,doctor")]
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments([FromQuery] string? status, [FromQuery] string? doctorId, [FromQuery] string? date)
        {
            var query = _context.Appointments.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }
            if (!string.IsNullOrEmpty(doctorId))
            {
                query = query.Where(a => a.DoctorId == doctorId);
            }
            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(a => a.Date == date);
            }

            var list = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
            return Ok(list);
        }

        [HttpPost("appointments")]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appt)
        {
            if (appt == null)
            {
                return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ." });
            }

            // 1. Basic validation
            if (string.IsNullOrEmpty(appt.PatientName) || string.IsNullOrEmpty(appt.Phone) ||
                string.IsNullOrEmpty(appt.DoctorId) || string.IsNullOrEmpty(appt.Date) || 
                string.IsNullOrEmpty(appt.Time) || string.IsNullOrEmpty(appt.Reason))
            {
                return BadRequest(new { message = "Vui lòng nhập đầy đủ các thông tin bắt buộc." });
            }

            // 2. Clean phone number
            appt.Phone = appt.Phone.Replace(" ", "");

            // 3. Verify Doctor exists
            var doctor = await _context.Doctors.FindAsync(appt.DoctorId);
            if (doctor == null)
            {
                return BadRequest(new { message = "Bác sĩ không tồn tại hoặc đã ngừng công tác." });
            }
            appt.DoctorName = doctor.Name;
            appt.Specialty = doctor.SpecialtyName;

            // 4. Validate Date (not in the past)
            if (!DateTime.TryParse(appt.Date, out var parsedDate))
            {
                return BadRequest(new { message = "Ngày khám không đúng định dạng." });
            }
            if (parsedDate.Date < DateTime.Today)
            {
                return BadRequest(new { message = "Không thể đặt lịch khám cho các ngày trong quá khứ." });
            }

            // 5. Validate Slot exists in Doctor's Schedule
            var dayOfWeek = parsedDate.DayOfWeek.ToString().ToLower();
            Dictionary<string, List<string>> scheduleMap = new();
            if (!string.IsNullOrEmpty(doctor.ScheduleJson))
            {
                try
                {
                    scheduleMap = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(doctor.ScheduleJson) ?? new();
                }
                catch {}
            }
            var allSlots = scheduleMap.ContainsKey(dayOfWeek) ? scheduleMap[dayOfWeek] : new List<string>();
            if (!allSlots.Contains(appt.Time))
            {
                return BadRequest(new { message = "Khung giờ chọn không nằm trong lịch làm việc của bác sĩ." });
            }

            // 6. Double-booking check
            var slotTaken = await _context.Appointments.AnyAsync(a => 
                a.DoctorId == appt.DoctorId && 
                a.Date == appt.Date && 
                a.Time == appt.Time && 
                a.Status != "cancelled");

            if (slotTaken)
            {
                return BadRequest(new { message = "Khung giờ này đã được đặt trước bởi bệnh nhân khác. Vui lòng chọn khung giờ khác." });
            }

            // 7. Initialize other fields
            appt.Id = "appt_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            appt.BookingId = GenerateBookingId();
            appt.Status = "pending";
            appt.CreatedAt = DateTime.Now;

            // 8. Payment fields
            if (string.IsNullOrEmpty(appt.PaymentMethod)) appt.PaymentMethod = "cash";
            appt.PaymentAmount = doctor.Price;
            // If paying at counter, stays unpaid; bank/momo/zalopay → pending_bank
            appt.PaymentStatus = appt.PaymentMethod == "cash" ? "unpaid" : "pending_bank";
            appt.PaidAt = null;

            _context.Appointments.Add(appt);
            await _context.SaveChangesAsync();
            return Ok(appt);
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpPut("appointments/{id}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(string id, [FromBody] JsonElement rawData)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return NotFound(new { message = "Không tìm thấy lịch hẹn" });

            if (rawData.TryGetProperty("status", out var statusProp))
            {
                appt.Status = statusProp.GetString() ?? appt.Status;
            }

            await _context.SaveChangesAsync();
            return Ok(appt);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("appointments/{id}/payment")]
        public async Task<IActionResult> UpdatePaymentStatus(string id, [FromBody] JsonElement rawData)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return NotFound(new { message = "Không tìm thấy lịch hẹn" });

            if (rawData.TryGetProperty("paymentStatus", out var psProp))
            {
                appt.PaymentStatus = psProp.GetString() ?? appt.PaymentStatus;
            }
            if (rawData.TryGetProperty("paymentMethod", out var pmProp))
            {
                appt.PaymentMethod = pmProp.GetString() ?? appt.PaymentMethod;
            }

            // Record payment timestamp when marking as paid
            if (appt.PaymentStatus == "paid" && appt.PaidAt == null)
            {
                appt.PaidAt = DateTime.Now;
            }
            else if (appt.PaymentStatus != "paid")
            {
                appt.PaidAt = null;
            }

            await _context.SaveChangesAsync();
            return Ok(new {
                appt.Id,
                appt.BookingId,
                appt.PaymentStatus,
                appt.PaymentMethod,
                appt.PaymentAmount,
                appt.PaidAt
            });
        }

        [HttpGet("appointments/lookup")]
        public async Task<IActionResult> LookupAppointment([FromQuery] string phone, [FromQuery] string bookingId)
        {
            var found = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Phone == phone && a.BookingId == bookingId);

            if (found == null) return Ok(null);

            // Return masked info
            var result = new
            {
                found.Id,
                found.BookingId,
                patientName = MaskName(found.PatientName),
                phone = MaskPhone(found.Phone),
                email = MaskEmail(found.Email),
                found.Gender,
                found.BirthYear,
                found.Reason,
                found.DoctorId,
                found.DoctorName,
                found.Specialty,
                found.Date,
                found.Time,
                found.Status,
                found.CreatedAt,
                found.PaymentMethod,
                found.PaymentStatus,
                found.PaymentAmount,
                found.PaidAt
            };

            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments([FromQuery] string? paymentStatus, [FromQuery] string? date)
        {
            var query = _context.Appointments.AsQueryable();

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(a => a.PaymentStatus == paymentStatus);
            if (!string.IsNullOrEmpty(date))
                query = query.Where(a => a.Date == date);

            var list = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();

            var result = list.Select(a => new
            {
                a.Id,
                a.BookingId,
                a.PatientName,
                a.Phone,
                a.DoctorName,
                a.Specialty,
                a.Date,
                a.Time,
                a.Status,
                a.PaymentMethod,
                a.PaymentStatus,
                a.PaymentAmount,
                a.PaidAt,
                a.CreatedAt
            });

            return Ok(result);
        }

        // ============================================
        // ACCOUNTS
        // ============================================
        [Authorize(Roles = "admin")]
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            var result = accounts.Select(a => new
            {
                a.Id,
                a.Username,
                password = "••••••••",
                a.Role,
                a.DoctorId,
                a.Name,
                a.CreatedAt
            });
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("accounts")]
        public async Task<IActionResult> CreateAccount([FromBody] Account acc)
        {
            var exists = await _context.Accounts.AnyAsync(a => a.Username == acc.Username);
            if (exists) return BadRequest(new { message = "Username đã tồn tại" });

            acc.Id = "acc_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            acc.CreatedAt = DateTime.Now;
            acc.Password = BCrypt.Net.BCrypt.HashPassword(acc.Password);
            _context.Accounts.Add(acc);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                acc.Id,
                acc.Username,
                password = "••••••••",
                acc.Role,
                acc.DoctorId,
                acc.Name,
                acc.CreatedAt
            });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("accounts/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var acc = await _context.Accounts.FindAsync(id);
            if (acc == null) return NotFound(new { message = "Không tìm thấy tài khoản" });

            _context.Accounts.Remove(acc);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        [Authorize]
        [HttpPut("accounts/{id}/profile")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] JsonElement data)
        {
            var acc = await _context.Accounts.FindAsync(id);
            if (acc == null) return NotFound(new { message = "Không tìm thấy tài khoản" });

            if (data.TryGetProperty("oldPassword", out var oldPwdProp) && data.TryGetProperty("newPassword", out var newPwdProp))
            {
                bool oldMatch = false;
                if (acc.Password.StartsWith("$2a$") || acc.Password.StartsWith("$2b$") || acc.Password.StartsWith("$2y$")) {
                    oldMatch = BCrypt.Net.BCrypt.Verify(oldPwdProp.GetString(), acc.Password);
                } else {
                    oldMatch = (acc.Password == oldPwdProp.GetString());
                }

                if (!oldMatch)
                {
                    return BadRequest(new { message = "Mật khẩu cũ không chính xác" });
                }
                acc.Password = BCrypt.Net.BCrypt.HashPassword(newPwdProp.GetString() ?? "");
            }

            if (data.TryGetProperty("name", out var nameProp))
            {
                acc.Name = nameProp.GetString() ?? acc.Name;
            }

            if (data.TryGetProperty("avatar", out var avatarProp))
            {
                acc.Avatar = avatarProp.GetString() ?? acc.Avatar;
            }

            await _context.SaveChangesAsync();

            // If account is associated with a doctor, update doctor details too
            if (acc.Role == "doctor" && !string.IsNullOrEmpty(acc.DoctorId))
            {
                var doc = await _context.Doctors.FindAsync(acc.DoctorId);
                if (doc != null)
                {
                    if (data.TryGetProperty("name", out var dNameProp)) doc.Name = dNameProp.GetString() ?? doc.Name;
                    if (data.TryGetProperty("degree", out var dDegProp)) doc.Degree = dDegProp.GetString() ?? doc.Degree;
                    if (data.TryGetProperty("biography", out var dBioProp)) doc.Bio = dBioProp.GetString() ?? doc.Bio;
                    if (data.TryGetProperty("avatar", out var dAvatarProp)) doc.Avatar = dAvatarProp.GetString() ?? doc.Avatar;
                    await _context.SaveChangesAsync();
                }
            }

            // Fix #3: Trả DTO thay vì entity (không lộ password hash)
            return Ok(new { acc.Id, acc.Username, acc.Role, acc.Name, acc.DoctorId, avatar = acc.Avatar ?? "" });
        }

        // ============================================
        // AUTH
        // ============================================
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] JsonElement creds)
        {
            string username = creds.GetProperty("username").GetString() ?? "";
            string password = creds.GetProperty("password").GetString() ?? "";

            // Handle plain text migration seamlessly
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
            if (account == null) return BadRequest(new { message = "Sai tên đăng nhập hoặc mật khẩu" });

            bool passwordMatches = false;
            if (account.Password.StartsWith("$2a$") || account.Password.StartsWith("$2b$") || account.Password.StartsWith("$2y$")) 
            {
                passwordMatches = BCrypt.Net.BCrypt.Verify(password, account.Password);
            } 
            else 
            {
                if (account.Password == password) 
                {
                    passwordMatches = true;
                    account.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    await _context.SaveChangesAsync();
                }
            }
            if (!passwordMatches) return BadRequest(new { message = "Sai tên đăng nhập hoặc mật khẩu" });

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

            var session = new
            {
                id = account.Id,
                username = account.Username,
                role = account.Role,
                name = account.Name,
                doctorId = account.DoctorId,
                avatar = account.Avatar ?? ""
            };

            return Ok(session);
        }

        [HttpPost("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        // ============================================
        // MEDICINES
        // ============================================
        [HttpGet("medicines")]
        public async Task<IActionResult> GetMedicines()
        {
            var meds = await _context.Medicines.ToListAsync();
            return Ok(meds);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("medicines")]
        public async Task<IActionResult> AddMedicine([FromBody] Medicine med)
        {
            med.Id = "med_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            _context.Medicines.Add(med);
            await _context.SaveChangesAsync();
            return Ok(med);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("medicines/{id}")]
        public async Task<IActionResult> UpdateMedicine(string id, [FromBody] Medicine data)
        {
            var med = await _context.Medicines.FindAsync(id);
            if (med == null) return NotFound(new { message = "Không tìm thấy thuốc" });

            med.Name = data.Name;
            med.Ingredients = data.Ingredients;
            med.Unit = data.Unit;
            med.ExpiryDate = data.ExpiryDate;

            await _context.SaveChangesAsync();
            return Ok(med);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("medicines/{id}")]
        public async Task<IActionResult> DeleteMedicine(string id)
        {
            var med = await _context.Medicines.FindAsync(id);
            if (med == null) return NotFound(new { message = "Không tìm thấy thuốc" });

            _context.Medicines.Remove(med);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        // ============================================
        // MEDICAL HISTORY
        // ============================================
        [Authorize(Roles = "admin,doctor")]
        [HttpGet("medical-history")]
        public async Task<IActionResult> SearchMedicalHistory([FromQuery] string? phone)
        {
            if (string.IsNullOrEmpty(phone)) return Ok(new List<object>());
            var cleanPhone = phone.Replace(" ", "");
            var list = await _context.MedicalHistories
                .Where(h => h.SoDienThoai == cleanPhone)
                .ToListAsync();

            var result = list.OrderByDescending(h => h.VisitDate)
                .Select(h => MapMedicalHistory(h))
                .ToList();

            return Ok(result);
        }

        [Authorize(Roles = "doctor")]
        [HttpPost("medical-history")]
        public async Task<IActionResult> AddMedicalHistory([FromBody] JsonElement rawData)
        {
            // 1. Verify doctor identity from claims
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;
            if (string.IsNullOrEmpty(doctorIdClaim))
            {
                return Forbid();
            }

            string? bookingId = null;
            if (rawData.TryGetProperty("bookingId", out var bIdProp))
            {
                bookingId = bIdProp.GetString();
            }

            if (!string.IsNullOrEmpty(bookingId))
            {
                var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.BookingId == bookingId);
                if (appointment == null)
                {
                    return BadRequest(new { message = "Không tìm thấy lịch hẹn tương ứng." });
                }
                if (appointment.DoctorId != doctorIdClaim)
                {
                    return BadRequest(new { message = "Bạn không được phân công khám cho lịch hẹn này." });
                }
                if (appointment.Status != "approved")
                {
                    return BadRequest(new { message = "Lịch hẹn chưa được duyệt hoặc đã hoàn thành." });
                }
            }

            // Fix #1: Bỏ Random ID, để SQL Server auto-increment (IDENTITY)
            var h = new MedicalHistory
            {
                VisitDate = DateTime.Now.ToString("yyyy-MM-dd")
            };

            // Tự động tạo MaBenhAn dựa trên max hiện tại
            var maxMaBenhAn = await _context.MedicalHistories.MaxAsync(m => (int?)m.MaBenhAn) ?? 0;
            h.MaBenhAn = maxMaBenhAn + 1;

            if (rawData.TryGetProperty("soDienThoai", out var phoneProp)) h.SoDienThoai = phoneProp.GetString() ?? "";
            h.BookingId = bookingId;
            if (rawData.TryGetProperty("patientName", out var patNameProp)) h.PatientName = patNameProp.GetString() ?? "";
            if (rawData.TryGetProperty("doctorName", out var docNameProp)) h.DoctorName = docNameProp.GetString() ?? "";
            if (rawData.TryGetProperty("specialty", out var specProp)) h.Specialty = specProp.GetString() ?? "";
            if (rawData.TryGetProperty("symptoms", out var sympProp)) h.Symptoms = sympProp.GetString() ?? "";
            if (rawData.TryGetProperty("diagnosis", out var diagProp)) h.Diagnosis = diagProp.GetString() ?? "";
            if (rawData.TryGetProperty("prescription", out var presProp)) h.Prescription = presProp.GetString() ?? "";
            if (rawData.TryGetProperty("notes", out var notesProp)) h.Notes = notesProp.GetString() ?? "";
            if (rawData.TryGetProperty("prescriptionItems", out var itemsProp)) h.PrescriptionItemsJson = itemsProp.ToString();

            _context.MedicalHistories.Add(h);
            await _context.SaveChangesAsync();

            return Ok(MapMedicalHistory(h));
        }

        [HttpGet("medical-history/result")]
        public async Task<IActionResult> GetExamResult([FromQuery] string bookingId)
        {
            // Fix #6: Tìm chính xác theo bookingId, không fallback sai
            var record = await _context.MedicalHistories.FirstOrDefaultAsync(h => h.BookingId == bookingId);
            if (record != null)
            {
                return Ok(MapMedicalHistory(record));
            }

            return Ok(null);
        }

        // ============================================
        // DASHBOARD STATS
        // ============================================
        [Authorize(Roles = "admin")]
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            // Fix #2 & #7: Tính toán dữ liệu thật từ DB bằng aggregate query
            var today = DateTime.Now.ToString("yyyy-MM-dd");

            var totalAppointments = await _context.Appointments.CountAsync();
            var todayAppointments = await _context.Appointments.CountAsync(a => a.Date == today);
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalSpecialties = await _context.Specialties.CountAsync();

            var pending = await _context.Appointments.CountAsync(a => a.Status == "pending");
            var approved = await _context.Appointments.CountAsync(a => a.Status == "approved");
            var completed = await _context.Appointments.CountAsync(a => a.Status == "completed");
            var cancelled = await _context.Appointments.CountAsync(a => a.Status == "cancelled");

            // Thống kê theo tháng (6 tháng gần nhất) - dữ liệu thật
            var monthlyData = new List<object>();
            for (int i = 5; i >= 0; i--)
            {
                var d = DateTime.Now.AddMonths(-i);
                var monthStart = d.ToString("yyyy-MM");
                var label = d.ToString("MM/yyyy");

                var monthAppts = await _context.Appointments
                    .CountAsync(a => a.Date != null && a.Date.StartsWith(monthStart));
                var monthCompleted = await _context.Appointments
                    .CountAsync(a => a.Date != null && a.Date.StartsWith(monthStart) && a.Status == "completed");

                // Tính doanh thu dựa trên số lượt khám hoàn thành * giá trung bình
                var avgPrice = await _context.Doctors.AverageAsync(d2 => (decimal?)d2.Price) ?? 350000;
                var monthRevenue = (long)(monthCompleted * avgPrice);

                monthlyData.Add(new { label, appointments = monthAppts, revenue = monthRevenue });
            }

            // Thống kê theo chuyên khoa - dữ liệu thật
            var specialties = await _context.Specialties.Take(6).ToListAsync();
            var specialtyStats = new List<object>();
            foreach (var s in specialties)
            {
                var count = await _context.Appointments.CountAsync(a => a.Specialty == s.Name);
                specialtyStats.Add(new { name = s.Name, count });
            }

            // Doanh thu tổng = số lượt completed * giá khám trung bình
            var globalAvgPrice = await _context.Doctors.AverageAsync(d2 => (decimal?)d2.Price) ?? 350000;

            var stats = new
            {
                totalPatients = totalAppointments,
                todayAppointments,
                totalDoctors,
                totalSpecialties,
                pending,
                approved,
                completed,
                cancelled,
                monthlyData,
                specialtyStats,
                revenue = (long)(completed * globalAvgPrice)
            };

            return Ok(stats);
        }

        // ============================================
        // HELPERS
        // ============================================

        // Fix #8: Helper method cho MedicalHistory mapping (không còn lặp code)
        private object MapMedicalHistory(MedicalHistory h)
        {
            return new
            {
                h.MaLichSu,
                h.SoDienThoai,
                h.MaBenhAn,
                h.BookingId,
                h.PatientName,
                h.DoctorName,
                h.Specialty,
                h.Symptoms,
                h.Diagnosis,
                prescription = h.Prescription,
                visitDate = h.VisitDate,
                h.Notes,
                prescriptionItems = string.IsNullOrEmpty(h.PrescriptionItemsJson)
                    ? new List<object>()
                    : JsonSerializer.Deserialize<List<object>>(h.PrescriptionItemsJson)
            };
        }

        private string GenerateBookingId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = "BK-";
            for (int i = 0; i < 8; i++)
            {
                result += chars[random.Next(chars.Length)];
            }
            return result;
        }

        private string MaskName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            var parts = name.Trim().Split(' ');
            if (parts.Length <= 1) return name[0] + "***";
            return parts[0] + " " + string.Join(" ", parts.Skip(1).Take(parts.Length - 2).Select(_ => "*")) + (parts.Length > 2 ? " " : "") + parts.Last()[0] + "***";
        }

        private string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 6) return phone;
            return phone.Substring(0, 3) + "****" + phone.Substring(phone.Length - 2);
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return "";
            int atIndex = email.IndexOf('@');
            if (atIndex <= 0) return "****" + email;
            if (atIndex > 2)
            {
                return email.Substring(0, 2) + "****" + email.Substring(atIndex);
            }
            return email.Substring(0, 1) + "****" + email.Substring(atIndex);
        }
    }
}
