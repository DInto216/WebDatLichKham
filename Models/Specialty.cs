using System.ComponentModel.DataAnnotations;

namespace WebDatLichKham.Models
{
    public class Specialty
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int DoctorCount { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
