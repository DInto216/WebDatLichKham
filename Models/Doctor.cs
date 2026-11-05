using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDatLichKham.Models
{
    public class Doctor
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        // Học hàm (Ví dụ: GS, PGS). Đảm bảo không bị null.
        public string AcademicTitle { get; set; } = string.Empty;

        // Học vị (Ví dụ: ThS, TS, BS, BSCKI, BSCKII).
        public string Degree { get; set; } = string.Empty;

        [Required]
        public string SpecialtyId { get; set; } = string.Empty;

        [ForeignKey("SpecialtyId")]
        public virtual Specialty? Specialty { get; set; }

        public string SpecialtyName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public double Rating { get; set; }

        public int Reviews { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Experience { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        // Stores schedule as JSON: e.g. {"monday": ["08:00", ...], "tuesday": [...]}
        public string ScheduleJson { get; set; } = string.Empty;
    }
}

