using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Data;
using WebDatLichKham.Models;

namespace WebDatLichKham.Services
{
    public interface IMedicalHistoryService
    {
        Task<IEnumerable<object>> SearchAsync(string phone);
        Task<object> AddAsync(JsonElement rawData);
        Task<object> GetResultAsync(string bookingId);
        Task<object> GetByIdAsync(int id);
    }

    public class MedicalHistoryService : IMedicalHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public MedicalHistoryService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IEnumerable<object>> SearchAsync(string phone)
        {
            var query = _context.MedicalHistories.AsNoTracking();
            if (!string.IsNullOrEmpty(phone))
            {
                var cleanPhone = phone.Replace(" ", "");
                query = query.Where(h => h.SoDienThoai == cleanPhone);
            }
            var list = await query.ToListAsync();

            return list.OrderByDescending(h => h.VisitDate)
                .Select(h => MapMedicalHistory(h))
                .ToList();
        }

        /// <summary>
        /// Tạo hồ sơ Bệnh án mới và hoàn tất luồng Khám bệnh.
        /// Sử dụng EF Core Transaction (ACID) để đảm bảo: 
        /// 1. Dữ liệu Bệnh án được lưu vào bảng MedicalHistories.
        /// 2. Trạng thái của bảng Appointments được cập nhật sang "completed".
        /// => NẾU 1 TRONG 2 BƯỚC BỊ LỖI (Ví dụ mất kết nối mạng, lỗi Server), toàn bộ thay đổi sẽ bị Rollback để tránh việc Lịch hẹn đã hoàn thành nhưng chưa có Bệnh án (Inconsistency).
        /// </summary>
        public async Task<object> AddAsync(JsonElement rawData)
        {
            // Bắt đầu 1 Transaction Database cấp phát đồng bộ
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var h = new MedicalHistory();

                if (rawData.TryGetProperty("soDienThoai", out var phoneProp)) h.SoDienThoai = phoneProp.GetString() ?? "";
                if (rawData.TryGetProperty("maBenhAn", out var baProp) && int.TryParse(baProp.GetString(), out int maBenhAn)) h.MaBenhAn = maBenhAn;
                if (rawData.TryGetProperty("bookingId", out var bookProp)) h.BookingId = bookProp.GetString() ?? "";
                if (rawData.TryGetProperty("patientName", out var patProp)) h.PatientName = patProp.GetString() ?? "";
                if (rawData.TryGetProperty("doctorName", out var docProp)) h.DoctorName = docProp.GetString() ?? "";
                if (rawData.TryGetProperty("specialty", out var specProp)) h.Specialty = specProp.GetString() ?? "";
                if (rawData.TryGetProperty("symptoms", out var sympProp)) h.Symptoms = sympProp.GetString() ?? "";
                if (rawData.TryGetProperty("diagnosis", out var diagProp)) h.Diagnosis = diagProp.GetString() ?? "";
                if (rawData.TryGetProperty("prescription", out var presProp)) h.Prescription = presProp.GetString() ?? "";
                if (rawData.TryGetProperty("notes", out var notesProp)) h.Notes = notesProp.GetString() ?? "";
                if (rawData.TryGetProperty("prescriptionItems", out var itemsProp)) h.PrescriptionItemsJson = itemsProp.GetRawText();

                h.VisitDate = DateTime.Now.ToString("yyyy-MM-dd");

                if (h.MaBenhAn == 0)
                {
                    var lastRecord = await _context.MedicalHistories.OrderByDescending(x => x.MaBenhAn).FirstOrDefaultAsync();
                    h.MaBenhAn = lastRecord != null ? lastRecord.MaBenhAn + 1 : 501;
                }

                _ = _context.MedicalHistories.Add(h);

                if (!string.IsNullOrEmpty(h.BookingId))
                {
                    var appt = await _context.Appointments.FirstOrDefaultAsync(a => a.BookingId == h.BookingId);
                    if (appt != null)
                    {
                        appt.Status = "completed";

                        // Send exam result email if email is provided
                        if (!string.IsNullOrWhiteSpace(appt.Email))
                        {
                            try
                            {
                                await _emailService.SendExamResultAsync(
                                    toEmail: appt.Email,
                                    patientName: appt.PatientName,
                                    bookingId: appt.BookingId,
                                    doctorName: appt.DoctorName,
                                    diagnosis: h.Diagnosis,
                                    prescription: h.Prescription
                                );
                            }
                            catch (Exception ex)
                            {
                                // LỖI ẨN ĐÃ FIX: Không được để lỗi SMTP làm hỏng Database Transaction.
                                // Nếu gửi mail thất bại, ghi log và cho phép lưu Bệnh án bình thường.
                                Console.WriteLine($"Lỗi gửi email bệnh án (BookingId: {appt.BookingId}): {ex.Message}");
                            }
                        }
                    }
                }

                // Ghi nhận thay đổi vào CSDL
                _ = await _context.SaveChangesAsync();

                // Lưu lại và đóng gói Transaction thành công (Commit)
                await transaction.CommitAsync();

                return MapMedicalHistory(h);
            }
            catch (Exception)
            {
                // [QUAN TRỌNG] Hủy bỏ mọi thay đổi nếu có lỗi xảy ra ở bất cứ dòng lệnh nào bên trên
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<object> GetResultAsync(string bookingId)
        {
            var h = await _context.MedicalHistories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BookingId == bookingId);

            if (h == null) return null;
            return MapMedicalHistory(h);
        }

        public async Task<object> GetByIdAsync(int id)
        {
            var h = await _context.MedicalHistories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaLichSu == id);

            if (h == null) return null;
            return MapMedicalHistory(h);
        }

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
    }
}
