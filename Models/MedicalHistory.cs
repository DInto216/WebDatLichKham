using System.ComponentModel.DataAnnotations;

namespace WebDatLichKham.Models
{
    public class MedicalHistory
    {
        [Key]
        public int MaLichSu { get; set; }

        public string SoDienThoai { get; set; } = string.Empty;

        public int MaBenhAn { get; set; }

        public string? BookingId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public string Symptoms { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;

        public string Prescription { get; set; } = string.Empty;

        public string VisitDate { get; set; } = string.Empty; // yyyy-MM-dd

        public string Notes { get; set; } = string.Empty;

        // Stores prescription items as JSON: e.g. [{"name": "Paracetamol", "qty": 20, "unit": "Viên", "dosage": "..."}]
        public string PrescriptionItemsJson { get; set; } = string.Empty;
    }
}
