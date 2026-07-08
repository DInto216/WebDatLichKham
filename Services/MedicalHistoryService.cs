using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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

        public async Task<object> AddAsync(JsonElement rawData)
        {
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
                if (rawData.TryGetProperty("prescriptionItems", out var itemsProp)) h.PrescriptionItemsJson = itemsProp.ToString();

                h.VisitDate = DateTime.Now.ToString("yyyy-MM-dd");

                if (h.MaBenhAn == 0)
                {
                    var lastRecord = await _context.MedicalHistories.OrderByDescending(x => x.MaBenhAn).FirstOrDefaultAsync();
                    h.MaBenhAn = lastRecord != null ? lastRecord.MaBenhAn + 1 : 501;
                }

                _context.MedicalHistories.Add(h);
                
                if (!string.IsNullOrEmpty(h.BookingId))
                {
                    var appt = await _context.Appointments.FirstOrDefaultAsync(a => a.BookingId == h.BookingId);
                    if (appt != null)
                    {
                        appt.Status = "completed";
                        
                        // Send exam result email if email is provided
                        if (!string.IsNullOrWhiteSpace(appt.Email))
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
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return MapMedicalHistory(h);
            }
            catch (Exception)
            {
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
