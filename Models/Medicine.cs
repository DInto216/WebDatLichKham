using System.ComponentModel.DataAnnotations;

namespace WebDatLichKham.Models
{
    public class Medicine
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Ingredients { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string ExpiryDate { get; set; } = string.Empty;
    }
}
