using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDatLichKham.Models
{
    public class PaymentSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MethodCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [MaxLength(100)]
        public string? Icon { get; set; }

        public string? AdditionalConfigJson { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
