using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Models;

namespace WebDatLichKham.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.Migrate();

            // Seed Admin Account
            if (!context.Accounts.Any(a => a.Role == "admin"))
            {
                _ = context.Accounts.Add(new Account
                {
                    Id = "acc_admin",
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "admin",
                    Name = "Quản trị viên",
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Seed sample Specialties
            if (!context.Specialties.Any())
            {
                context.Specialties.AddRange(
                    new Specialty { Id = "sp_01", Name = "Nội tổng quát", Icon = "bi-heart-pulse", Description = "Khám và điều trị các bệnh nội khoa", DoctorCount = 2 },
                    new Specialty { Id = "sp_02", Name = "Nhi khoa", Icon = "bi-brightness-high", Description = "Khám và điều trị bệnh cho trẻ em", DoctorCount = 1 },
                    new Specialty { Id = "sp_03", Name = "Tai Mũi Họng", Icon = "bi-ear", Description = "Khám các bệnh lý về Tai Mũi Họng", DoctorCount = 1 }
                );
            }
            _ = context.SaveChanges();

            // Seed sample Doctor
            if (!context.Doctors.Any())
            {
                var docId = "doc_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                _ = context.Doctors.Add(new Doctor
                {
                    Id = docId,
                    Name = "PGS.TS Nguyễn Văn An",
                    Degree = "PGS.TS",
                    SpecialtyId = "sp_01",
                    SpecialtyName = "Nội tổng quát",
                    Price = 300000,
                    Rating = 4.9,
                    Reviews = 120,
                    Bio = "Bác sĩ chuyên khoa Nội với hơn 15 năm kinh nghiệm làm việc tại các bệnh viện tuyến đầu.",
                    Avatar = "https://i.pravatar.cc/150?img=11",
                    ScheduleJson = "{}"
                });

                // Seed Doctor Account
                if (!context.Accounts.Any(a => a.Username == "doctor01"))
                {
                    _ = context.Accounts.Add(new Account
                    {
                        Id = "acc_" + docId,
                        Username = "doctor01",
                        Password = BCrypt.Net.BCrypt.HashPassword("doctor123"),
                        Role = "doctor",
                        Name = "PGS.TS Nguyễn Văn An",
                        DoctorId = docId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            _ = context.SaveChanges();
        }
    }
}
