using System.ComponentModel.DataAnnotations;

namespace WebDatLichKham.Models
{
    public class DoctorReview
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public string DoctorId { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;

        public int Rating { get; set; } = 5;

        public string Date { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;
    }
}

