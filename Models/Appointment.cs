using System;
using System.ComponentModel.DataAnnotations;

namespace WebDatLichKham.Models
{
    public class Appointment
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string BookingId { get; set; } = string.Empty;

        [Required]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public int BirthYear { get; set; }

        public string Reason { get; set; } = string.Empty;

        [Required]
        public string DoctorId { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        [Required]
        public string Date { get; set; } = string.Empty; // yyyy-MM-dd

        [Required]
        public string Time { get; set; } = string.Empty; // HH:mm

        public string Status { get; set; } = "pending"; // pending, approved, cancelled, completed

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ---- Payment Fields ----
        /// <summary>cash | bank_transfer | momo | zalopay</summary>
        public string PaymentMethod { get; set; } = "cash";

        /// <summary>unpaid | pending_bank | paid | refunded</summary>
        public string PaymentStatus { get; set; } = "unpaid";

        public decimal PaymentAmount { get; set; } = 0;

        public DateTime? PaidAt { get; set; }
    }
}
