using System;
using System.ComponentModel.DataAnnotations;

namespace WebDatLichKham.Models
{
    public class Account
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // admin or doctor

        public string? DoctorId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
