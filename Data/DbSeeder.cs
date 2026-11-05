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

