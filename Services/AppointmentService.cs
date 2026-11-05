using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync(string? doctorId = null, string? date = null);
        Task<Appointment> AddAsync(Appointment appointment);
        Task<Appointment> UpdateStatusAsync(string id, string status);
        Task<Appointment> UpdatePaymentAsync(string id, string paymentStatus);
        Task<IEnumerable<Appointment>> LookupAsync(string phone);
        Task<IEnumerable<object>> GetPaymentsAsync();
        Task<(bool Success, string Message, Appointment? Data)> CancelByPatientAsync(string bookingId, string phone, string? reason);
        Task<(bool Success, string Message, Appointment? Data)> RescheduleByPatientAsync(string bookingId, string phone, string newDate, string newTime);
        Task<object> GenerateAntiSpamCaptchaAsync();
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public AppointmentService(ApplicationDbContext context, IEmailService emailService, IMemoryCache cache)
        {
            _context = context;
            _emailService = emailService;
            _cache = cache;
        }

        // ============================================
        // ANTI-SPAM: Tạo câu hỏi xác minh Math Captcha
        // ============================================
        public Task<object> GenerateAntiSpamCaptchaAsync()
        {
            var rng = new Random();
            int a = rng.Next(1, 20);
            int b = rng.Next(1, 20);
            int answer = a + b;

            // Token ngẫu nhiên 1 lần dùng, hết hạn sau 10 phút
            var token = Guid.NewGuid().ToString("N");
            _cache.Set("captcha_" + token, answer.ToString(), TimeSpan.FromMinutes(10));

            return Task.FromResult<object>(new
            {
                token,
                question = $"{a} + {b} = ?"
            });
        }

        // ============================================
        // GET ALL APPOINTMENTS
        // ============================================
        public async Task<IEnumerable<Appointment>> GetAllAsync(string? doctorId = null, string? date = null)
        {
            // TỐI ƯU HÓA: Sử dụng AsNoTracking cho các truy vấn chỉ đọc để vô hiệu hóa Tracking Snapshot, tiết kiệm RAM.
            var query = _context.Appointments.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(doctorId))
            {
                query = query.Where(a => a.DoctorId == doctorId);
            }

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(a => a.Date == date);
            }

            return await query.OrderByDescending(a => a.Date).ThenByDescending(a => a.Time).ToListAsync();
        }

        // ============================================
        // ADD APPOINTMENT (có đầy đủ Anti-Spam)
        // ============================================
        public async Task<Appointment> AddAsync(Appointment data)
        {
            // === LỚP 1: HONEYPOT TRAP ===
            // Field ẩn trong form - bot thường tự điền, người thật để trống
            if (!string.IsNullOrWhiteSpace(data.BotTrap))
            {
                throw new ArgumentException("Yêu cầu đặt lịch bị từ chối do phát hiện hành vi tự động.");
            }

            // === LỚP 2: KIỂM TRA SỐ ĐIỆN THOẠI VIỆT NAM ===
            if (!string.IsNullOrEmpty(data.Phone))
            {
                data.Phone = data.Phone.Replace(" ", "").Replace("-", "");
            }
            if (string.IsNullOrEmpty(data.Phone) ||
                !System.Text.RegularExpressions.Regex.IsMatch(data.Phone, @"^(03|05|07|08|09)\d{8}$|^02\d{9}$"))
            {
                throw new ArgumentException("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam hợp lệ (10-11 chữ số, đầu số 02x, 03x, 05x, 07x, 08x, 09x).");
            }
            // Chặn số lặp bất thường (ví dụ: 0999999999)
            var digitRepeat = System.Text.RegularExpressions.Regex.IsMatch(data.Phone, @"(\d)\1{7,}");
            if (digitRepeat)
            {
                throw new ArgumentException("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại thực.");
            }

            // === LỚP 3: RATE LIMITING (Chống gửi liên tục) ===
            var rateLimitKey = "ratelimit_" + data.Phone;
            if (_cache.TryGetValue(rateLimitKey, out _))
            {
                throw new ArgumentException("Bạn vừa đặt lịch gần đây. Vui lòng đợi 30 giây trước khi thử lại.");
            }

            // === LỚP 4: GIỚI HẠN LỊCH HẸN ĐANG HOẠT ĐỘNG ===
            var activeCount = await _context.Appointments.AsNoTracking()
                .CountAsync(a => a.Phone == data.Phone && a.Status != "cancelled" && a.Status != "completed");
            if (activeCount >= 3)
            {
                throw new ArgumentException("Số điện thoại này đã có 3 lịch hẹn đang chờ xác nhận. Vui lòng hủy bớt lịch hẹn cũ trước khi đặt mới.");
            }

            // === LỚP 5: KIỂM TRA NGÀY HỢP LỆ ===
            if (DateTime.TryParse(data.Date, out var appointmentDate))
            {
                if (appointmentDate.Date < DateTime.Today)
                    throw new ArgumentException("Ngày khám không hợp lệ. Không thể đặt lịch trong quá khứ.");
                if (appointmentDate.Date > DateTime.Today.AddDays(60))
                    throw new ArgumentException("Ngày khám không hợp lệ. Chỉ được phép đặt lịch trong vòng 60 ngày tới.");
            }

            // === LỚP 6: CHỐNG TRÙNG LỊCH BÁC SĨ ===
            if (!string.IsNullOrEmpty(data.DoctorId))
            {
                bool slotTaken = await _context.Appointments.AnyAsync(a =>
                    a.DoctorId == data.DoctorId &&
                    a.Date == data.Date &&
                    a.Time == data.Time &&
                    a.Status != "cancelled");
                if (slotTaken)
                    throw new ArgumentException("Khung giờ này của bác sĩ đã có người đặt. Vui lòng chọn khung giờ khác.");
            }

            // === LỚP 7: XÁC MINH MATH CAPTCHA ===
            if (string.IsNullOrWhiteSpace(data.CaptchaToken) || string.IsNullOrWhiteSpace(data.CaptchaAnswer))
            {
                throw new ArgumentException("Vui lòng hoàn thành câu hỏi xác minh bảo mật trước khi đặt lịch.");
            }
            var cacheKey = "captcha_" + data.CaptchaToken;
            if (!_cache.TryGetValue(cacheKey, out string? correctAnswer) || correctAnswer != data.CaptchaAnswer.Trim())
            {
                throw new ArgumentException("Câu trả lời xác minh không đúng. Vui lòng thử lại.");
            }
            // Xóa token ngay sau khi dùng (one-time use)
            _cache.Remove(cacheKey);

            // === ĐẶT RATE LIMIT SAU KHI PASS HẾT KIỂM TRA ===
            _cache.Set(rateLimitKey, true, TimeSpan.FromSeconds(30));

            // ============================================
            // LƯU DỮ LIỆU
            // ============================================
            if (string.IsNullOrEmpty(data.BookingId))
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var result = new char[8];
                for (int i = 0; i < 8; i++)
                {
                    result[i] = chars[random.Next(chars.Length)];
                }
                data.BookingId = "BK-" + new string(result);
            }

            data.Id = "APT" + DateTime.Now.ToString("yyyyMMddHHmmss");
            data.Status = "pending";
            data.PaymentStatus = "unpaid";
            data.CreatedAt = DateTime.Now;

            // Optional: Doctor validation logic if provided
            if (!string.IsNullOrEmpty(data.DoctorId))
            {
                var doc = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == data.DoctorId);
                if (doc != null)
                {
                    data.DoctorName = doc.Name;
                    data.Specialty = doc.SpecialtyName;
                }
            }

            _ = _context.Appointments.Add(data);
            _ = await _context.SaveChangesAsync();

            // Send email notification if email is provided
            if (!string.IsNullOrWhiteSpace(data.Email))
            {
                try
                {
                    await _emailService.SendBookingConfirmationAsync(
                        toEmail: data.Email,
                        patientName: data.PatientName,
                        bookingId: data.BookingId,
                        date: data.Date,
                        time: data.Time,
                        doctorName: data.DoctorName,
                        specialty: data.Specialty
                    );
                }
                catch (Exception ex)
                {
                    // LỖI ẨN ĐÃ FIX: Bỏ qua lỗi gửi Email để không làm Crash quá trình Đặt lịch của bệnh nhân
                    Console.WriteLine($"Lỗi gửi email xác nhận đặt lịch (BookingId: {data.BookingId}): {ex.Message}");
                }
            }

            return data;
        }

        // ============================================
        // UPDATE STATUS
        // ============================================
        public async Task<Appointment> UpdateStatusAsync(string id, string status)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return null;

            appt.Status = status;
            _ = await _context.SaveChangesAsync();
            return appt;
        }

        // ============================================
        // UPDATE PAYMENT
        // ============================================
        public async Task<Appointment> UpdatePaymentAsync(string id, string paymentStatus)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return null;

            appt.PaymentStatus = paymentStatus;
            if (paymentStatus == "paid" && !appt.PaidAt.HasValue)
            {
                appt.PaidAt = DateTime.Now;
            }
            _ = await _context.SaveChangesAsync();
            return appt;
        }

        // ============================================
        // LOOKUP
        // ============================================
        public async Task<IEnumerable<Appointment>> LookupAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return new List<Appointment>();

            var cleanPhone = phone.Replace(" ", "");
            return await _context.Appointments.AsNoTracking()
                .Where(a => a.Phone == cleanPhone)
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.Time)
                .ToListAsync();
        }

        // ============================================
        // GET PAYMENTS
        // ============================================
        public async Task<IEnumerable<object>> GetPaymentsAsync()
        {
            var list = await _context.Appointments.AsNoTracking()
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.Time)
                .ToListAsync();

            return list.Select(a => new
            {
                id = a.Id,
                transactionId = a.Id,
                appointmentId = a.BookingId,
                patientName = a.PatientName,
                amount = a.PaymentAmount,
                paymentMethod = a.PaymentMethod,
                transactionDate = a.PaidAt?.ToString("yyyy-MM-ddTHH:mm:ss") ?? (a.Date + "T" + a.Time + ":00"),
                status = a.PaymentStatus,
                createdAt = a.CreatedAt
            });
        }

        // ============================================
        // CANCEL BY PATIENT
        // ============================================
        public async Task<(bool Success, string Message, Appointment? Data)> CancelByPatientAsync(string bookingId, string phone, string? reason)
        {
            if (string.IsNullOrWhiteSpace(bookingId) || string.IsNullOrWhiteSpace(phone))
            {
                return (false, "Vui lòng cung cấp mã đặt lịch và số điện thoại hợp lệ.", null);
            }

            var cleanPhone = phone.Trim().Replace(" ", "").Replace("-", "");
            var appt = await _context.Appointments.FirstOrDefaultAsync(a =>
                (a.BookingId == bookingId || a.Id == bookingId) && a.Phone == cleanPhone);

            if (appt == null)
            {
                return (false, "Không tìm thấy lịch hẹn phù hợp với mã đặt lịch và số điện thoại này.", null);
            }

            if (appt.Status == "completed")
            {
                return (false, "Không thể hủy lịch hẹn đã hoàn thành khám bệnh.", null);
            }

            if (appt.Status == "cancelled")
            {
                return (false, "Lịch hẹn này đã được hủy trước đó.", appt);
            }

            appt.Status = "cancelled";
            if (!string.IsNullOrWhiteSpace(reason))
            {
                appt.Reason = string.IsNullOrWhiteSpace(appt.Reason)
                    ? $"[Bệnh nhân hủy]: {reason}"
                    : $"{appt.Reason} | [Lý do hủy]: {reason}";
            }

            _ = await _context.SaveChangesAsync();
            return (true, "Hủy lịch hẹn thành công.", appt);
        }

        // ============================================
        // RESCHEDULE BY PATIENT
        // ============================================
        public async Task<(bool Success, string Message, Appointment? Data)> RescheduleByPatientAsync(string bookingId, string phone, string newDate, string newTime)
        {
            if (string.IsNullOrWhiteSpace(bookingId) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(newDate) || string.IsNullOrWhiteSpace(newTime))
            {
                return (false, "Vui lòng cung cấp đầy đủ thông tin mã lịch hẹn, số điện thoại, ngày khám và giờ khám mới.", null);
            }

            var cleanPhone = phone.Trim().Replace(" ", "").Replace("-", "");
            var appt = await _context.Appointments.FirstOrDefaultAsync(a =>
                (a.BookingId == bookingId || a.Id == bookingId) && a.Phone == cleanPhone);

            if (appt == null)
            {
                return (false, "Không tìm thấy lịch hẹn phù hợp với mã đặt lịch và số điện thoại này.", null);
            }

            if (appt.Status == "completed")
            {
                return (false, "Không thể đổi ngày giờ đối với lịch hẹn đã hoàn thành khám bệnh.", null);
            }

            if (appt.Status == "cancelled")
            {
                return (false, "Lịch hẹn này đã bị hủy. Vui lòng tạo lịch hẹn mới thay vì đổi lịch.", null);
            }

            if (DateTime.TryParse(newDate, out var targetDate))
            {
                if (targetDate.Date < DateTime.Today)
                {
                    return (false, "Ngày khám mới không được ở trong quá khứ.", null);
                }
            }

            // Kiểm tra trùng slot khám của Bác sĩ này
            bool isSlotTaken = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == appt.DoctorId &&
                a.Date == newDate &&
                a.Time == newTime &&
                a.Status != "cancelled" &&
                a.Id != appt.Id);

            if (isSlotTaken)
            {
                return (false, "Khung giờ này của bác sĩ đã có bệnh nhân khác đặt. Vui lòng chọn một khung giờ khác.", null);
            }

            appt.Date = newDate;
            appt.Time = newTime;
            appt.Status = "pending"; // Đặt lại trạng thái chờ phòng khám xác nhận khung giờ mới

            _ = await _context.SaveChangesAsync();
            return (true, "Đổi lịch khám thành công. Lịch hẹn của bạn đã được chuyển sang ngày giờ mới và đang chờ phòng khám duyệt.", appt);
        }
    }
}
