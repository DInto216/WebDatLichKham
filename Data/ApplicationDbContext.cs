using Microsoft.EntityFrameworkCore;
using WebDatLichKham.Models;

namespace WebDatLichKham.Data
{
    /// <summary>
    /// [CLEAN CODE] TẦNG DATA ACCESS LAYER: Kế thừa DbContext của Entity Framework Core.
    /// File này cấu hình toàn bộ mapping (Object-Relational Mapping - ORM) giữa ứng dụng C# và cơ sở dữ liệu SQL Server.
    /// Việc gom nhóm DbSet tập trung tại đây giúp đảm bảo Data Synchronization và tính toàn vẹn (Integrity) của luồng dữ liệu.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // KHAI BÁO CÁC BẢNG TRONG CƠ SỞ DỮ LIỆU (DbSet)
        // Tất cả Entity đều được khai báo đầy đủ, đảm bảo đồng bộ chặt chẽ với bản EF Migrations
        // ==========================================
        public DbSet<Specialty> Specialties { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Medicine> Medicines { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<MedicalHistory> MedicalHistories { get; set; } = null!;
        public DbSet<DoctorReview> DoctorReviews { get; set; } = null!;
        public DbSet<PaymentSetting> PaymentSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // [DATA SEEDING]: Cấu hình dữ liệu mẫu chuẩn để tạo sẵn ngay khi chạy bản Migration đầu tiên.
            // Điều này giúp dự án Production luôn có sẵn cấu hình gốc (vd: Tài khoản Admin mặc định) mà không cần insert tay.

            // Seed Specialties
            _ = modelBuilder.Entity<Specialty>().HasData(
                new Specialty { Id = "sp_01", Name = "Nội tổng quát", Icon = "🫀", Description = "Khám và điều trị các bệnh nội khoa", DoctorCount = 8 },
                new Specialty { Id = "sp_02", Name = "Nhi khoa", Icon = "👶", Description = "Chăm sóc sức khỏe trẻ em", DoctorCount = 5 },
                new Specialty { Id = "sp_03", Name = "Tim mạch", Icon = "❤️", Description = "Chẩn đoán và điều trị bệnh tim", DoctorCount = 4 },
                new Specialty { Id = "sp_04", Name = "Da liễu", Icon = "🧴", Description = "Điều trị các bệnh về da", DoctorCount = 3 },
                new Specialty { Id = "sp_05", Name = "Tai Mũi Họng", Icon = "👂", Description = "Khám và điều trị TMH", DoctorCount = 4 },
                new Specialty { Id = "sp_06", Name = "Mắt", Icon = "👁️", Description = "Khám và điều trị bệnh về mắt", DoctorCount = 3 },
                new Specialty { Id = "sp_07", Name = "Răng Hàm Mặt", Icon = "🦷", Description = "Chăm sóc sức khỏe răng miệng", DoctorCount = 6 },
                new Specialty { Id = "sp_08", Name = "Chấn thương chỉnh hình", Icon = "🦴", Description = "Điều trị xương khớp", DoctorCount = 3 },
                new Specialty { Id = "sp_09", Name = "Thần kinh", Icon = "🧠", Description = "Chẩn đoán bệnh thần kinh", DoctorCount = 2 },
                new Specialty { Id = "sp_10", Name = "Sản phụ khoa", Icon = "🤰", Description = "Chăm sóc sức khỏe phụ nữ", DoctorCount = 5 }
            );

            // Seed Doctors
            _ = modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = "doc_01",
                    Name = "BS. Nguyễn Văn An",
                    AcademicTitle = "PGS",
                    Degree = "TS",
                    SpecialtyId = "sp_01",
                    SpecialtyName = "Nội tổng quát",
                    Price = 350000,
                    Rating = 4.8,
                    Reviews = 124,
                    Avatar = "",
                    Bio = "Hơn 15 năm kinh nghiệm trong lĩnh vực Nội khoa tổng quát",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\",\"16:00\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_02",
                    Name = "BS. Trần Thị Bích",
                    AcademicTitle = "",
                    Degree = "TS",
                    SpecialtyId = "sp_02",
                    SpecialtyName = "Nhi khoa",
                    Price = 400000,
                    Rating = 4.9,
                    Reviews = 210,
                    Avatar = "",
                    Bio = "20 năm kinh nghiệm chuyên khoa Nhi, từng tu nghiệp tại Nhật Bản",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"10:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\",\"15:00\"],\"wednesday\":[],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_03",
                    Name = "BS. Lê Minh Tuấn",
                    AcademicTitle = "",
                    Degree = "ThS",
                    SpecialtyId = "sp_03",
                    SpecialtyName = "Tim mạch",
                    Price = 500000,
                    Rating = 4.7,
                    Reviews = 98,
                    Avatar = "",
                    Bio = "Chuyên gia tim mạch can thiệp, tốt nghiệp ĐH Y Hà Nội",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"thursday\":[],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"saturday\":[],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_04",
                    Name = "BS. Phạm Hồng Nhung",
                    AcademicTitle = "",
                    Degree = "CKI",
                    SpecialtyId = "sp_04",
                    SpecialtyName = "Da liễu",
                    Price = 300000,
                    Rating = 4.6,
                    Reviews = 156,
                    Avatar = "",
                    Bio = "Chuyên gia Da liễu thẩm mỹ, nhiều năm kinh nghiệm",
                    ScheduleJson = "{\"monday\":[\"09:00\",\"09:30\",\"10:00\",\"10:30\",\"14:00\",\"14:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"wednesday\":[\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"friday\":[],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_05",
                    Name = "BS. Võ Thanh Hùng",
                    AcademicTitle = "PGS",
                    Degree = "TS",
                    SpecialtyId = "sp_05",
                    SpecialtyName = "Tai Mũi Họng",
                    Price = 350000,
                    Rating = 4.8,
                    Reviews = 89,
                    Avatar = "",
                    Bio = "Phó Giáo sư chuyên ngành Tai Mũi Họng",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"tuesday\":[\"14:00\",\"14:30\",\"15:00\",\"15:30\",\"16:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"saturday\":[],\"sunday\":[]}"
                },
                new Doctor
                {
                    Id = "doc_06",
                    Name = "BS. Đặng Thị Mai",
                    AcademicTitle = "",
                    Degree = "TS",
                    SpecialtyId = "sp_10",
                    SpecialtyName = "Sản phụ khoa",
                    Price = 450000,
                    Rating = 4.9,
                    Reviews = 175,
                    Avatar = "",
                    Bio = "Tiến sĩ chuyên ngành Sản phụ khoa, BV Từ Dũ",
                    ScheduleJson = "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\"],\"friday\":[\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\"],\"sunday\":[]}"
                },
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
            );

            // Seed Accounts
            _ = modelBuilder.Entity<Account>().HasData(
                new Account { Id = "acc_admin", Username = "admin", Password = "admin123", Role = "admin", Name = "Quản trị viên", CreatedAt = new DateTime(2025, 1, 1) },
                new Account { Id = "acc_doc01", Username = "doctor01", Password = "doctor123", Role = "doctor", DoctorId = "doc_01", Name = "BS. Nguyễn Văn An", CreatedAt = new DateTime(2025, 1, 15) },
                new Account { Id = "acc_doc02", Username = "doctor02", Password = "doctor123", Role = "doctor", DoctorId = "doc_02", Name = "BS. Trần Thị Bích", CreatedAt = new DateTime(2025, 2, 1) },
                new Account { Id = "acc_doc03", Username = "doctor03", Password = "doctor123", Role = "doctor", DoctorId = "doc_03", Name = "BS. Lê Minh Tuấn", CreatedAt = new DateTime(2025, 2, 10) },
                new Account { Id = "acc_doc04", Username = "doctor04", Password = "doctor123", Role = "doctor", DoctorId = "doc_04", Name = "BS. Phạm Hồng Nhung", CreatedAt = new DateTime(2025, 2, 15) },
                new Account { Id = "acc_doc05", Username = "doctor05", Password = "doctor123", Role = "doctor", DoctorId = "doc_05", Name = "BS. Võ Thanh Hùng", CreatedAt = new DateTime(2025, 2, 20) },
                new Account { Id = "acc_doc06", Username = "doctor06", Password = "doctor123", Role = "doctor", DoctorId = "doc_06", Name = "BS. Đặng Thị Mai", CreatedAt = new DateTime(2025, 3, 1) },
                new Account { Id = "acc_doc07", Username = "doctor07", Password = "doctor123", Role = "doctor", DoctorId = "doc_07", Name = "BS. Hoàng Minh Đức", CreatedAt = new DateTime(2025, 3, 5) },
                new Account { Id = "acc_doc08", Username = "doctor08", Password = "doctor123", Role = "doctor", DoctorId = "doc_08", Name = "BS. Nguyễn Phương Thảo", CreatedAt = new DateTime(2025, 3, 10) },
                new Account { Id = "acc_doc09", Username = "doctor09", Password = "doctor123", Role = "doctor", DoctorId = "doc_09", Name = "BS. Vũ Tuấn Phong", CreatedAt = new DateTime(2025, 3, 15) },
                new Account { Id = "acc_doc10", Username = "doctor10", Password = "doctor123", Role = "doctor", DoctorId = "doc_10", Name = "BS. Đỗ Quang Huy", CreatedAt = new DateTime(2025, 3, 20) },
                new Account { Id = "acc_doc11", Username = "doctor11", Password = "doctor123", Role = "doctor", DoctorId = "doc_11", Name = "BS. Bùi Khánh Linh", CreatedAt = new DateTime(2025, 3, 25) },
                new Account { Id = "acc_doc12", Username = "doctor12", Password = "doctor123", Role = "doctor", DoctorId = "doc_12", Name = "BS. Lê Thảo Trang", CreatedAt = new DateTime(2025, 3, 28) }
            );

            // Seed Medicines
            _ = modelBuilder.Entity<Medicine>().HasData(
                new Medicine { Id = "med_01", Name = "Paracetamol 500mg", Ingredients = "Paracetamol", Unit = "Viên", ExpiryDate = "2027-12-31" },
                new Medicine { Id = "med_02", Name = "Amoxicillin 250mg", Ingredients = "Amoxicillin", Unit = "Viên", ExpiryDate = "2026-05-15" },
                new Medicine { Id = "med_03", Name = "Ibuprofen 400mg", Ingredients = "Ibuprofen", Unit = "Viên", ExpiryDate = "2028-01-20" },
                new Medicine { Id = "med_04", Name = "Vitamin C 1000mg", Ingredients = "Vitamin C", Unit = "Viên sủi", ExpiryDate = "2025-10-10" },
                new Medicine { Id = "med_05", Name = "Thuốc ho Bảo Thanh", Ingredients = "Thảo dược", Unit = "Chai", ExpiryDate = "2026-02-28" }
            );

            // Seed Appointments
            _ = modelBuilder.Entity<Appointment>().HasData(
                new Appointment
                {
                    Id = "appt_01",
                    BookingId = "BK-A1B2C3D4",
                    PatientName = "Nguyễn Thị Hoa",
                    Phone = "0912345678",
                    Email = "hoa@email.com",
                    Gender = "Nữ",
                    BirthYear = 1990,
                    Reason = "Đau đầu, chóng mặt kéo dài",
                    DoctorId = "doc_01",
                    DoctorName = "BS. Nguyễn Văn An",
                    Specialty = "Nội tổng quát",
                    Date = "2026-06-26",
                    Time = "08:00",
                    Status = "pending",
                    CreatedAt = new DateTime(2026, 6, 25, 10, 0, 0)
                },
                new Appointment
                {
                    Id = "appt_02",
                    BookingId = "BK-E5F6G7H8",
                    PatientName = "Trần Văn Minh",
                    Phone = "0987654321",
                    Email = "minh@email.com",
                    Gender = "Nam",
                    BirthYear = 1985,
                    Reason = "Khám sức khỏe tổng quát",
                    DoctorId = "doc_01",
                    DoctorName = "BS. Nguyễn Văn An",
                    Specialty = "Nội tổng quát",
                    Date = "2026-06-26",
                    Time = "09:00",
                    Status = "approved",
                    CreatedAt = new DateTime(2026, 6, 24, 14, 30, 0)
                },
                new Appointment
                {
                    Id = "appt_03",
                    BookingId = "BK-I9J0K1L2",
                    PatientName = "Lê Thị Lan",
                    Phone = "0909123456",
                    Email = "lan@email.com",
                    Gender = "Nữ",
                    BirthYear = 1995,
                    Reason = "Bé sốt cao, ho nhiều",
                    DoctorId = "doc_02",
                    DoctorName = "BS. Trần Thị Bích",
                    Specialty = "Nhi khoa",
                    Date = "2026-06-26",
                    Time = "08:30",
                    Status = "approved",
                    CreatedAt = new DateTime(2026, 6, 24, 9, 0, 0)
                },
                new Appointment
                {
                    Id = "appt_04",
                    BookingId = "BK-M3N4O5P6",
                    PatientName = "Phạm Quốc Bảo",
                    Phone = "0978111222",
                    Email = "bao@email.com",
                    Gender = "Nam",
                    BirthYear = 1978,
                    Reason = "Đau ngực, khó thở",
                    DoctorId = "doc_03",
                    DoctorName = "BS. Lê Minh Tuấn",
                    Specialty = "Tim mạch",
                    Date = "2026-06-25",
                    Time = "14:00",
                    Status = "completed",
                    CreatedAt = new DateTime(2026, 6, 23, 16, 0, 0)
                },
                new Appointment
                {
                    Id = "appt_05",
                    BookingId = "BK-Q7R8S9T0",
                    PatientName = "Hoàng Thị Yến",
                    Phone = "0961222333",
                    Email = "yen@email.com",
                    Gender = "Nữ",
                    BirthYear = 2000,
                    Reason = "Nổi mẩn đỏ, ngứa da",
                    DoctorId = "doc_04",
                    DoctorName = "BS. Phạm Hồng Nhung",
                    Specialty = "Da liễu",
                    Date = "2026-06-27",
                    Time = "09:00",
                    Status = "pending",
                    CreatedAt = new DateTime(2026, 6, 25, 11, 0, 0)
                },
                new Appointment
                {
                    Id = "appt_06",
                    BookingId = "BK-U1V2W3X4",
                    PatientName = "Đỗ Văn Tùng",
                    Phone = "0945333444",
                    Email = "tung@email.com",
                    Gender = "Nam",
                    BirthYear = 1970,
                    Reason = "Viêm họng mãn tính",
                    DoctorId = "doc_05",
                    DoctorName = "BS. Võ Thanh Hùng",
                    Specialty = "Tai Mũi Họng",
                    Date = "2026-06-25",
                    Time = "08:30",
                    Status = "cancelled",
                    CreatedAt = new DateTime(2026, 6, 22, 8, 0, 0)
                }
            );

            // Seed Medical History
            _ = modelBuilder.Entity<MedicalHistory>().HasData(
                new MedicalHistory
                {
                    MaLichSu = 901,
                    SoDienThoai = "0912345678",
                    MaBenhAn = 501,
                    BookingId = "BK-A1B2C3D4",
                    PatientName = "Nguyễn Thị Hoa",
                    DoctorName = "BS. Nguyễn Văn An",
                    Specialty = "Nội tổng quát",
                    Symptoms = "Đau đầu dữ dội vùng trán và thái dương, buồn nôn, nhạy cảm ánh sáng",
                    Diagnosis = "Đau nửa đầu migraine",
                    Prescription = "Paracetamol 500mg x2, Ibuprofen 400mg x1",
                    VisitDate = "2026-03-10",
                    Notes = "Tái khám sau 2 tuần. Tránh căng thẳng, ngủ đủ giấc.",
                    PrescriptionItemsJson = "[{\"name\":\"Paracetamol 500mg\",\"qty\":20,\"unit\":\"Viên\",\"dosage\":\"Ngày 2 lần, mỗi lần 1 viên, sau ăn\"},{\"name\":\"Ibuprofen 400mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 lần khi đau, sau ăn no\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 902,
                    SoDienThoai = "0912345678",
                    MaBenhAn = 502,
                    BookingId = null,
                    PatientName = "Nguyễn Thị Hoa",
                    DoctorName = "BS. Nguyễn Văn An",
                    Specialty = "Nội tổng quát",
                    Symptoms = "Tái khám, đau đầu giảm",
                    Diagnosis = "Tái khám - Đau nửa đầu",
                    Prescription = "Paracetamol 500mg x1",
                    VisitDate = "2026-03-24",
                    Notes = "Tình trạng cải thiện tốt",
                    PrescriptionItemsJson = "[{\"name\":\"Paracetamol 500mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Khi đau, tối đa 2 viên/ngày\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 903,
                    SoDienThoai = "0912345678",
                    MaBenhAn = 503,
                    BookingId = null,
                    PatientName = "Nguyễn Thị Hoa",
                    DoctorName = "BS. Phạm Hồng Nhung",
                    Specialty = "Da liễu",
                    Symptoms = "Da tay bị đỏ, ngứa, bị nổi mẩn",
                    Diagnosis = "Viêm da tiếp xúc",
                    Prescription = "Kem bôi Hydrocortisone 1%",
                    VisitDate = "2026-05-15",
                    Notes = "Tránh tiếp xúc hóa chất",
                    PrescriptionItemsJson = "[{\"name\":\"Kem bôi Hydrocortisone 1%\",\"qty\":1,\"unit\":\"Tuýp\",\"dosage\":\"Bôi vùng tổn thương 2 lần/ngày\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 904,
                    SoDienThoai = "0987654321",
                    MaBenhAn = 504,
                    BookingId = null,
                    PatientName = "Trần Văn Minh",
                    DoctorName = "BS. Nguyễn Văn An",
                    Specialty = "Nội tổng quát",
                    Symptoms = "Đau bụng vùng thượng vị, ợ chua",
                    Diagnosis = "Viêm dạ dày",
                    Prescription = "Omeprazole 20mg x2, Sucralfate x3",
                    VisitDate = "2026-01-20",
                    Notes = "Kiêng đồ cay nóng, rượu bia",
                    PrescriptionItemsJson = "[{\"name\":\"Omeprazole 20mg\",\"qty\":14,\"unit\":\"Viên\",\"dosage\":\"Ngày 2 lần trước ăn 30 phút\"},{\"name\":\"Sucralfate 1g\",\"qty\":21,\"unit\":\"Gói\",\"dosage\":\"Ngày 3 lần trước ăn\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 905,
                    SoDienThoai = "0987654321",
                    MaBenhAn = 505,
                    BookingId = null,
                    PatientName = "Trần Văn Minh",
                    DoctorName = "BS. Lê Minh Tuấn",
                    Specialty = "Tim mạch",
                    Symptoms = "Chóng mặt, đau đầu khi thay đổi tư thế",
                    Diagnosis = "Tăng huyết áp độ 1",
                    Prescription = "Amlodipine 5mg x1",
                    VisitDate = "2026-04-05",
                    Notes = "Theo dõi huyết áp hàng ngày",
                    PrescriptionItemsJson = "[{\"name\":\"Amlodipine 5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng sau ăn\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 906,
                    SoDienThoai = "0909123456",
                    MaBenhAn = 506,
                    BookingId = null,
                    PatientName = "Lê Thị Lan",
                    DoctorName = "BS. Trần Thị Bích",
                    Specialty = "Nhi khoa",
                    Symptoms = "Bé sốt cao, ho nhiều, khó thở",
                    Diagnosis = "Viêm phế quản cấp (bé)",
                    Prescription = "Amoxicillin 250mg x3, Thuốc ho Bảo Thanh",
                    VisitDate = "2026-02-14",
                    Notes = "Uống nhiều nước, nghỉ ngơi",
                    PrescriptionItemsJson = "[{\"name\":\"Amoxicillin 250mg\",\"qty\":21,\"unit\":\"Viên\",\"dosage\":\"Ngày 3 lần, mỗi lần 1 viên\"},{\"name\":\"Thuốc ho Bảo Thanh\",\"qty\":1,\"unit\":\"Chai\",\"dosage\":\"Ngày 3 lần, 10ml mỗi lần\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 907,
                    SoDienThoai = "0978111222",
                    MaBenhAn = 507,
                    BookingId = "BK-M3N4O5P6",
                    PatientName = "Phạm Quốc Bảo",
                    DoctorName = "BS. Lê Minh Tuấn",
                    Specialty = "Tim mạch",
                    Symptoms = "Đau ngực, khó thở khi gắng sức, hồi hộp",
                    Diagnosis = "Rối loạn nhịp tim",
                    Prescription = "Bisoprolol 2.5mg x1, Aspirin 81mg x1",
                    VisitDate = "2026-05-20",
                    Notes = "Hạn chế vận động mạnh. Tái khám sau 3 tuần.",
                    PrescriptionItemsJson = "[{\"name\":\"Bisoprolol 2.5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng\"},{\"name\":\"Aspirin 81mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sau ăn trưa\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 908,
                    SoDienThoai = "0978111222",
                    MaBenhAn = 508,
                    BookingId = null,
                    PatientName = "Phạm Quốc Bảo",
                    DoctorName = "BS. Lê Minh Tuấn",
                    Specialty = "Tim mạch",
                    Symptoms = "Tái khám, còn hồi hộp nhẹ",
                    Diagnosis = "Tái khám - Rối loạn nhịp tim",
                    Prescription = "Bisoprolol 2.5mg x1",
                    VisitDate = "2026-06-10",
                    Notes = "Nhịp tim ổn định, tiếp tục thuốc",
                    PrescriptionItemsJson = "[{\"name\":\"Bisoprolol 2.5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 909,
                    SoDienThoai = "0961222333",
                    MaBenhAn = 509,
                    BookingId = null,
                    PatientName = "Hoàng Thị Yến",
                    DoctorName = "BS. Phạm Hồng Nhung",
                    Specialty = "Da liễu",
                    Symptoms = "Nổi mẩn đỏ, ngứa khắp người",
                    Diagnosis = "Mề đay dị ứng",
                    Prescription = "Loratadine 10mg x1, Kem dưỡng ẩm",
                    VisitDate = "2026-04-18",
                    Notes = "Tránh hải sản, phấn hoa",
                    PrescriptionItemsJson = "[{\"name\":\"Loratadine 10mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, tối\"},{\"name\":\"Kem dưỡng ẩm Cetaphil\",\"qty\":1,\"unit\":\"Tuýp\",\"dosage\":\"Bôi 2 lần/ngày sau tắm\"}]"
                },
                new MedicalHistory
                {
                    MaLichSu = 910,
                    SoDienThoai = "0945333444",
                    MaBenhAn = 510,
                    BookingId = null,
                    PatientName = "Đỗ Văn Tùng",
                    DoctorName = "BS. Võ Thanh Hùng",
                    Specialty = "Tai Mũi Họng",
                    Symptoms = "Nghẹt mũi, đau đầu vùng trán, chảy dịch mũi",
                    Diagnosis = "Viêm xoang mãn tính",
                    Prescription = "Amoxicillin 250mg x3, Xịt mũi Nasonex",
                    VisitDate = "2026-02-28",
                    Notes = "Rửa mũi bằng nước muối sinh lý hàng ngày",
                    PrescriptionItemsJson = "[{\"name\":\"Amoxicillin 250mg\",\"qty\":21,\"unit\":\"Viên\",\"dosage\":\"Ngày 3 lần, mỗi lần 1 viên\"},{\"name\":\"Xịt mũi Nasonex\",\"qty\":1,\"unit\":\"Chai\",\"dosage\":\"Xịt 2 nhát mỗi bên mũi, ngày 1 lần\"}]"
                }
            );

            // Seed Doctor Reviews
            _ = modelBuilder.Entity<DoctorReview>().HasData(
                new DoctorReview { Id = "rev_01", DoctorId = "doc_01", DoctorName = "BS. Nguyễn Văn An", PatientName = "Nguyễn Tấn Đ.", Rating = 5, Date = "2026-06-15", Comment = "Bác sĩ An tư vấn rất nhiệt tình, chu đáo. Quy trình khám diễn ra nhanh chóng, tôi rất hài lòng.", Specialty = "Nội tổng quát" },
                new DoctorReview { Id = "rev_02", DoctorId = "doc_02", DoctorName = "BS. Trần Thị Bích", PatientName = "Lê Minh H.", Rating = 5, Date = "2026-06-20", Comment = "Phòng khám sạch sẽ, bác sĩ Bích giải thích kỹ càng về bệnh án và thuốc. Tuyệt vời!", Specialty = "Nhi khoa" },
                new DoctorReview { Id = "rev_03", DoctorId = "doc_03", DoctorName = "BS. Lê Minh Tuấn", PatientName = "Trần Quyết T.", Rating = 4, Date = "2026-06-28", Comment = "Chuyên môn cao, dịch vụ tốt. Tuy nhiên lúc chờ lấy thuốc hơi lâu một chút.", Specialty = "Tim mạch" },
                new DoctorReview { Id = "rev_04", DoctorId = "doc_04", DoctorName = "BS. Phạm Hồng Nhung", PatientName = "Phạm Thị M.", Rating = 5, Date = "2026-07-01", Comment = "Bác sĩ Nhung rất nhẹ nhàng, mát tay. Các nốt mẩn đỏ của tôi đã hết hẳn sau 1 tuần dùng thuốc.", Specialty = "Da liễu" },
                new DoctorReview { Id = "rev_05", DoctorId = "doc_05", DoctorName = "BS. Võ Thanh Hùng", PatientName = "Vũ Đức K.", Rating = 5, Date = "2026-07-02", Comment = "Rất chuyên nghiệp, bác sĩ Hùng thao tác cực kỳ cẩn thận.", Specialty = "Tai Mũi Họng" },
                new DoctorReview { Id = "rev_06", DoctorId = "doc_01", DoctorName = "BS. Nguyễn Văn An", PatientName = "Đinh Tuấn N.", Rating = 5, Date = "2026-07-03", Comment = "Hệ thống đặt lịch rất tiện, không phải chờ đợi. Bác sĩ siêu có tâm.", Specialty = "Nội tổng quát" },
                new DoctorReview { Id = "rev_07", DoctorId = "doc_07", DoctorName = "BS. Hoàng Minh Đức", PatientName = "Hoàng Long V.", Rating = 5, Date = "2026-07-04", Comment = "Bác sĩ Đức khám mắt rất kỹ, máy móc hiện đại và tư vấn phẫu thuật khúc xạ tận tâm.", Specialty = "Mắt" },
                new DoctorReview { Id = "rev_08", DoctorId = "doc_08", DoctorName = "BS. Nguyễn Phương Thảo", PatientName = "Ngô Mai P.", Rating = 5, Date = "2026-07-05", Comment = "Bác sĩ Thảo làm răng rất nhẹ nhàng, không đau một chút nào. 10 điểm chất lượng!", Specialty = "Răng Hàm Mặt" },
                new DoctorReview { Id = "rev_09", DoctorId = "doc_09", DoctorName = "BS. Vũ Tuấn Phong", PatientName = "Trịnh Quốc K.", Rating = 5, Date = "2026-07-06", Comment = "Chấn thương dây chằng của tôi hồi phục rất nhanh nhờ phác đồ của PGS Phong.", Specialty = "Chấn thương chỉnh hình" },
                new DoctorReview { Id = "rev_10", DoctorId = "doc_10", DoctorName = "BS. Đỗ Quang Huy", PatientName = "Lâm Ánh T.", Rating = 5, Date = "2026-07-07", Comment = "Bác sĩ Huy chữa đau nửa đầu tiền đình rất hiệu quả, tôi ngủ ngon hơn nhiều.", Specialty = "Thần kinh" }
            );

            // Seed Payment Settings
            _ = modelBuilder.Entity<PaymentSetting>().HasData(
                new PaymentSetting { Id = 1, MethodCode = "cash", DisplayName = "Thanh toán tại quầy", IsActive = true, Icon = "bi-cash", UpdatedAt = new DateTime(2026, 1, 1) },
                new PaymentSetting { Id = 2, MethodCode = "bank_transfer", DisplayName = "Chuyển khoản Ngân hàng", IsActive = true, Icon = "bi-bank2", UpdatedAt = new DateTime(2026, 1, 1), AdditionalConfigJson = "{\"bankName\":\"Vietcombank\",\"accountNumber\":\"0123456789\",\"accountName\":\"MEDIBOOKING VN\"}" },
                new PaymentSetting { Id = 3, MethodCode = "momo", DisplayName = "Ví MoMo", IsActive = true, Icon = "bi-qr-code", UpdatedAt = new DateTime(2026, 1, 1) },
                new PaymentSetting { Id = 4, MethodCode = "zalopay", DisplayName = "ZaloPay", IsActive = true, Icon = "bi-qr-code", UpdatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}

