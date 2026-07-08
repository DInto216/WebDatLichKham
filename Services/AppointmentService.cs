using Microsoft.EntityFrameworkCore;
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
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AppointmentService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

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

        public async Task<Appointment> AddAsync(Appointment data)
        {
            if (!string.IsNullOrEmpty(data.Phone))
            {
                data.Phone = data.Phone.Replace(" ", "").Replace("-", "");
            }
            if (string.IsNullOrEmpty(data.Phone) || !System.Text.RegularExpressions.Regex.IsMatch(data.Phone, @"^\d{10,11}$"))
            {
                throw new ArgumentException("Số điện thoại không hợp lệ. Số điện thoại phải gồm 10-11 chữ số.");
            }

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

        public async Task<Appointment> UpdateStatusAsync(string id, string status)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return null;

            appt.Status = status;
            _ = await _context.SaveChangesAsync();
            return appt;
        }

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

        public async Task<IEnumerable<object>> GetPaymentsAsync()
        {
            var list = await _context.Appointments.AsNoTracking()
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.Time)
                .ToListAsync();

            return list.Select(a => new
            {
                id = a.Id,
                transactionId = a.Id, // Use appointment ID as transaction ID
                appointmentId = a.BookingId,
                patientName = a.PatientName,
                amount = a.PaymentAmount,
                paymentMethod = a.PaymentMethod,
                transactionDate = a.PaidAt?.ToString("yyyy-MM-ddTHH:mm:ss") ?? (a.Date + "T" + a.Time + ":00"),
                status = a.PaymentStatus, // frontend expects 'status' to be the payment status
                createdAt = a.CreatedAt
            });
        }
    }
}
