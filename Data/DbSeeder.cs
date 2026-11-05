using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Models;

namespace WebDatLichKham.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DbSeeder] Migration warning skipped: " + ex.Message);
            }

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

            // Bổ sung đầy đủ Bác sĩ cho tất cả 10 Chuyên khoa
            var newDoctors = new List<Doctor>
            {
                new Doctor
                {
                    Id = "doc_07",
                    Name = "BS. Hoàng Minh Đức",
                    AcademicTitle = "ThS",
                    Degree = "BSCKII",
                    SpecialtyId = "sp_06",
                    SpecialtyName = "Mắt",
                    Price = 350000,
                    Rating = 4.9,
                    Reviews = 142,
                    Avatar = "https://images.unsplash.com/photo-1622253692010-333f2da6031d?w=150",
                    Bio = "Hơn 12 năm kinh nghiệm phẫu thuật khúc xạ và điều trị các bệnh lý về Mắt, Bệnh viện Mắt Trung ương.",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\",\"15:00\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_08",
                    Name = "BS. Nguyễn Phương Thảo",
                    AcademicTitle = "ThS",
                    Degree = "BS",
                    SpecialtyId = "sp_07",
                    SpecialtyName = "Răng Hàm Mặt",
                    Price = 300000,
                    Rating = 4.8,
                    Reviews = 189,
                    Avatar = "https://images.unsplash.com/photo-1594824813589-73e4a30a84d4?w=150",
                    Bio = "Chuyên gia nắn chỉnh răng thẩm mỹ, cấy ghép Implant và nha khoa phục hình cao cấp.",
                    ScheduleJson = "{\"monday\":[\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"tuesday\":[\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"wednesday\":[\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\"],\"thursday\":[\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"friday\":[\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\",\"15:00\"],\"saturday\":[\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_09",
                    Name = "BS. Vũ Tuấn Phong",
                    AcademicTitle = "PGS",
                    Degree = "TS",
                    SpecialtyId = "sp_08",
                    SpecialtyName = "Chấn thương chỉnh hình",
                    Price = 500000,
                    Rating = 4.9,
                    Reviews = 205,
                    Avatar = "https://images.unsplash.com/photo-1537368910025-700350fe46c7?w=150",
                    Bio = "Chuyên gia đầu ngành cơ xương khớp và phẫu thuật chỉnh hình chấn thương thể thao.",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"tuesday\":[\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"thursday\":[\"14:00\",\"14:30\",\"15:00\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"saturday\":[],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_10",
                    Name = "BS. Đỗ Quang Huy",
                    AcademicTitle = "TS",
                    Degree = "BS",
                    SpecialtyId = "sp_09",
                    SpecialtyName = "Thần kinh",
                    Price = 450000,
                    Rating = 4.8,
                    Reviews = 118,
                    Avatar = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=150",
                    Bio = "Nhiều năm tu nghiệp tại Pháp, chuyên sâu điều trị đau nửa đầu Migraine, rối loạn tiền đình và thần kinh cơ.",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\"],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_11",
                    Name = "BS. Bùi Khánh Linh",
                    AcademicTitle = "ThS",
                    Degree = "BSCKI",
                    SpecialtyId = "sp_01",
                    SpecialtyName = "Nội tổng quát",
                    Price = 350000,
                    Rating = 4.7,
                    Reviews = 95,
                    Avatar = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=150",
                    Bio = "Chuyên gia điều trị tiểu đường, tăng huyết áp và theo dõi sức khỏe nội khoa định kỳ.",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\"],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_12",
                    Name = "BS. Lê Thảo Trang",
                    AcademicTitle = "",
                    Degree = "BSCKII",
                    SpecialtyId = "sp_02",
                    SpecialtyName = "Nhi khoa",
                    Price = 380000,
                    Rating = 4.9,
                    Reviews = 168,
                    Avatar = "https://images.unsplash.com/photo-1594824813589-73e4a30a84d4?w=150",
                    Bio = "Bác sĩ chuyên khoa Nhi BV Nhi Đồng, tận tâm, thấu hiểu tâm lý trẻ nhỏ và tư vấn dinh dưỡng.",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"sunday\":[]}"
                }
            };

            foreach (var doc in newDoctors)
            {
                if (!context.Doctors.Any(d => d.Id == doc.Id))
                {
                    context.Doctors.Add(doc);
                }
            }
            // Cập nhật số lượng bác sĩ (DoctorCount) cho từng chuyên khoa
            foreach (var sp in context.Specialties.ToList())
            {
                sp.DoctorCount = context.Doctors.Count(d => d.SpecialtyId == sp.Id);
            }
            context.SaveChanges();

            // Đồng bộ: Tự động tạo Tài khoản (Account) cho tất cả Bác sĩ (Doctor) hiện có
            // nhằm đảm bảo số liệu hiển thị trên Admin khớp với số liệu thực tế ngoài trang chủ
            var allDoctors = context.Doctors.ToList();
            bool hasNewAccounts = false;
            foreach (var doc in allDoctors)
            {
                if (!context.Accounts.Any(a => a.DoctorId == doc.Id))
                {
                    // Lấy số từ doc_01, doc_02... để đặt username (ví dụ: doctor04)
                    string docNum = doc.Id.Replace("doc_", "");
                    if (docNum.Length != 2) docNum = new Random().Next(100, 999).ToString();
                    
                    context.Accounts.Add(new Account
                    {
                        Id = "acc_" + doc.Id,
                        Username = "doctor" + docNum,
                        Password = BCrypt.Net.BCrypt.HashPassword("doctor123"), // hash lại chuẩn theo AccountService
                        Role = "doctor",
                        Name = doc.Name,
                        DoctorId = doc.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                    hasNewAccounts = true;
                }
            }
            if (hasNewAccounts)
            {
                context.SaveChanges();
            }

            // Sinh dữ liệu mẫu (Sample Data Generator) từ 1/1/2025 đến nay
            if (context.Appointments.Count() < 50)
            {
                var random = new Random();
                var startDate = new DateTime(2025, 1, 1);
                var endDate = DateTime.UtcNow;
                var totalDays = (endDate - startDate).Days > 0 ? (endDate - startDate).Days : 1;

                var doctors = context.Doctors.ToList();
                if (doctors.Any())
                {
                    var sampleReasons = new[] { "Đau đầu, chóng mặt", "Khám tổng quát", "Đau bụng vùng thượng vị", "Ho, sốt kéo dài", "Nổi mẩn đỏ ngứa", "Khám định kỳ", "Đau nhức xương khớp", "Mất ngủ kéo dài", "Tim đập nhanh", "Viêm họng" };
                    var sampleNames = new[] { "Nguyễn Văn A", "Trần Thị B", "Lê Văn C", "Phạm Thị D", "Hoàng Văn E", "Vũ Thị F", "Đặng Văn G", "Bùi Thị H", "Đỗ Văn I", "Hồ Thị K", "Đinh Văn L", "Mai Thị M" };
                    var statuses = new[] { "pending", "approved", "completed", "cancelled" };

                    for (int i = 0; i < 500; i++)
                    {
                        var randomDate = startDate.AddDays(random.Next(totalDays));
                        var doc = doctors[random.Next(doctors.Count)];
                        var isPast = randomDate < endDate.AddDays(-1); 
                        var status = isPast ? (random.Next(10) < 8 ? "completed" : "cancelled") : statuses[random.Next(statuses.Length)];

                        var apt = new Appointment
                        {
                            Id = "apt_gen_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                            BookingId = "BK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                            PatientName = sampleNames[random.Next(sampleNames.Length)] + " " + random.Next(1, 100),
                            Phone = "09" + random.Next(10000000, 99999999).ToString(),
                            Email = $"patient{i}@email.com",
                            Gender = random.Next(2) == 0 ? "Nam" : "Nữ",
                            BirthYear = random.Next(1960, 2015),
                            Reason = sampleReasons[random.Next(sampleReasons.Length)],
                            DoctorId = doc.Id,
                            DoctorName = doc.Name,
                            Specialty = doc.SpecialtyName,
                            Date = randomDate.ToString("yyyy-MM-dd"),
                            Time = $"{random.Next(8, 17):D2}:{(random.Next(2) == 0 ? "00" : "30")}",
                            Status = status,
                            PaymentAmount = doc.Price,
                            PaymentMethod = random.Next(2) == 0 ? "cash" : (random.Next(2) == 0 ? "bank_transfer" : "momo"),
                            PaymentStatus = (status == "completed" || random.Next(10) < 3) ? "paid" : "unpaid",
                            CreatedAt = randomDate.AddDays(-random.Next(1, 10))
                        };
                        
                        if (apt.PaymentStatus == "paid") apt.PaidAt = randomDate;
                        context.Appointments.Add(apt);

                        if (status == "completed")
                        {
                            var history = new MedicalHistory
                            {
                                SoDienThoai = apt.Phone,
                                MaBenhAn = random.Next(1000, 9999),
                                BookingId = apt.BookingId,
                                PatientName = apt.PatientName,
                                DoctorName = apt.DoctorName,
                                Specialty = apt.Specialty,
                                Symptoms = apt.Reason,
                                Diagnosis = "Chẩn đoán mẫu: " + apt.Reason,
                                Prescription = "Thuốc mẫu theo chỉ định",
                                VisitDate = apt.Date,
                                Notes = "Khám và điều trị ngoại trú, tái khám nếu cần",
                                PrescriptionItemsJson = "[]"
                            };
                            context.MedicalHistories.Add(history);
                        }
                    }
                    context.SaveChanges();
                }
            }

            // Đồng bộ dữ liệu cũ: Tự động sửa lại những lịch khám (do seed đợt trước) bị thiếu PaymentAmount = 0
            var zeroAmountApts = context.Appointments.Where(a => a.PaymentAmount == 0).ToList();
            if (zeroAmountApts.Any())
            {
                var doctorsDict = context.Doctors.ToDictionary(d => d.Id, d => d.Price);
                foreach (var a in zeroAmountApts)
                {
                    if (doctorsDict.TryGetValue(a.DoctorId, out decimal price))
                    {
                        a.PaymentAmount = price;
                        a.PaymentMethod = "cash";
                        a.PaymentStatus = (a.Status == "completed") ? "paid" : "unpaid";
                        if (a.PaymentStatus == "paid" && a.PaidAt == null) a.PaidAt = a.CreatedAt.AddDays(1);
                    }
                }
                context.SaveChanges();
            }

            _ = context.SaveChanges();
        }
    }
}

