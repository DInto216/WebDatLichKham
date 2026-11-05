using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebDatLichKham.Migrations
{
    /// <inheritdoc />
    public partial class InitialProduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthYear = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoctorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialtyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialtyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Reviews = table.Column<int>(type: "int", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Doctors", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "MedicalHistories",
                columns: table => new
                {
                    MaLichSu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaBenhAn = table.Column<int>(type: "int", nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoctorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisitDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrescriptionItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_MedicalHistories", x => x.MaLichSu);
                });

            _ = migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ingredients = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Medicines", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoctorCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Specialties", x => x.Id);
                });

            _ = migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Avatar", "CreatedAt", "DoctorId", "Name", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { "acc_admin", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Qu?n tr? viên", "admin123", "admin", "admin" },
                    { "acc_doc01", null, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "doc_01", "BS. Nguy?n Van An", "doctor123", "doctor", "doctor01" },
                    { "acc_doc02", null, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "doc_02", "BS. Tr?n Th? Bích", "doctor123", "doctor", "doctor02" },
                    { "acc_doc03", null, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "doc_03", "BS. Lê Minh Tu?n", "doctor123", "doctor", "doctor03" }
                });

            _ = migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "BirthYear", "BookingId", "CreatedAt", "Date", "DoctorId", "DoctorName", "Email", "Gender", "PatientName", "Phone", "Reason", "Specialty", "Status", "Time" },
                values: new object[,]
                {
                    { "appt_01", 1990, "BK-A1B2C3D4", new DateTime(2026, 6, 25, 10, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-26", "doc_01", "BS. Nguy?n Van An", "hoa@email.com", "N?", "Nguy?n Th? Hoa", "0912345678", "Ðau d?u, chóng m?t kéo dài", "N?i t?ng quát", "pending", "08:00" },
                    { "appt_02", 1985, "BK-E5F6G7H8", new DateTime(2026, 6, 24, 14, 30, 0, 0, DateTimeKind.Unspecified), "2026-06-26", "doc_01", "BS. Nguy?n Van An", "minh@email.com", "Nam", "Tr?n Van Minh", "0987654321", "Khám s?c kh?e t?ng quát", "N?i t?ng quát", "approved", "09:00" },
                    { "appt_03", 1995, "BK-I9J0K1L2", new DateTime(2026, 6, 24, 9, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-26", "doc_02", "BS. Tr?n Th? Bích", "lan@email.com", "N?", "Lê Th? Lan", "0909123456", "Bé s?t cao, ho nhi?u", "Nhi khoa", "approved", "08:30" },
                    { "appt_04", 1978, "BK-M3N4O5P6", new DateTime(2026, 6, 23, 16, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-25", "doc_03", "BS. Lê Minh Tu?n", "bao@email.com", "Nam", "Ph?m Qu?c B?o", "0978111222", "Ðau ng?c, khó th?", "Tim m?ch", "completed", "14:00" },
                    { "appt_05", 2000, "BK-Q7R8S9T0", new DateTime(2026, 6, 25, 11, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-27", "doc_04", "BS. Ph?m H?ng Nhung", "yen@email.com", "N?", "Hoàng Th? Y?n", "0961222333", "N?i m?n d?, ng?a da", "Da li?u", "pending", "09:00" },
                    { "appt_06", 1970, "BK-U1V2W3X4", new DateTime(2026, 6, 22, 8, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-25", "doc_05", "BS. Võ Thanh Hùng", "tung@email.com", "Nam", "Ð? Van Tùng", "0945333444", "Viêm h?ng mãn tính", "Tai Mui H?ng", "cancelled", "08:30" }
                });

            _ = migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "Avatar", "Bio", "Degree", "Name", "Price", "Rating", "Reviews", "ScheduleJson", "SpecialtyId", "SpecialtyName" },
                values: new object[,]
                {
                    { "doc_01", "", "Hon 15 nam kinh nghi?m trong linh v?c N?i khoa t?ng quát", "PGS.TS", "BS. Nguy?n Van An", 350000m, 4.7999999999999998, 124, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\",\"16:00\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"sunday\":[]}", "sp_01", "N?i t?ng quát" },
                    { "doc_02", "", "20 nam kinh nghi?m chuyên khoa Nhi, t?ng tu nghi?p t?i Nh?t B?n", "TS.BS", "BS. Tr?n Th? Bích", 400000m, 4.9000000000000004, 210, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"10:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\",\"15:00\"],\"wednesday\":[],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"sunday\":[]}", "sp_02", "Nhi khoa" },
                    { "doc_03", "", "Chuyên gia tim m?ch can thi?p, t?t nghi?p ÐH Y Hà N?i", "ThS.BS", "BS. Lê Minh Tu?n", 500000m, 4.7000000000000002, 98, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"thursday\":[],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"saturday\":[],\"sunday\":[]}", "sp_03", "Tim m?ch" },
                    { "doc_04", "", "Chuyên gia Da li?u th?m m?, nhi?u nam kinh nghi?m", "BS.CKI", "BS. Ph?m H?ng Nhung", 300000m, 4.5999999999999996, 156, "{\"monday\":[\"09:00\",\"09:30\",\"10:00\",\"10:30\",\"14:00\",\"14:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"wednesday\":[\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"friday\":[],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"sunday\":[]}", "sp_04", "Da li?u" },
                    { "doc_05", "", "Phó Giáo su chuyên ngành Tai Mui H?ng", "PGS.TS", "BS. Võ Thanh Hùng", 350000m, 4.7999999999999998, 89, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"tuesday\":[\"14:00\",\"14:30\",\"15:00\",\"15:30\",\"16:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"saturday\":[],\"sunday\":[]}", "sp_05", "Tai Mui H?ng" },
                    { "doc_06", "", "Ti?n si chuyên ngành S?n ph? khoa, BV T? Du", "TS.BS", "BS. Ð?ng Th? Mai", 450000m, 4.9000000000000004, 175, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\"],\"friday\":[\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\"],\"sunday\":[]}", "sp_10", "S?n ph? khoa" }
                });

            _ = migrationBuilder.InsertData(
                table: "MedicalHistories",
                columns: new[] { "MaLichSu", "BookingId", "Diagnosis", "DoctorName", "MaBenhAn", "Notes", "PatientName", "Prescription", "PrescriptionItemsJson", "SoDienThoai", "Specialty", "Symptoms", "VisitDate" },
                values: new object[,]
                {
                    { 901, "BK-A1B2C3D4", "Ðau n?a d?u migraine", "BS. Nguy?n Van An", 501, "Tái khám sau 2 tu?n. Tránh cang th?ng, ng? d? gi?c.", "Nguy?n Th? Hoa", "Paracetamol 500mg x2, Ibuprofen 400mg x1", "[{\"name\":\"Paracetamol 500mg\",\"qty\":20,\"unit\":\"Viên\",\"dosage\":\"Ngày 2 l?n, m?i l?n 1 viên, sau an\"},{\"name\":\"Ibuprofen 400mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 l?n khi dau, sau an no\"}]", "0912345678", "N?i t?ng quát", "Ðau d?u d? d?i vùng trán và thái duong, bu?n nôn, nh?y c?m ánh sáng", "2026-03-10" },
                    { 902, null, "Tái khám - Ðau n?a d?u", "BS. Nguy?n Van An", 502, "Tình tr?ng c?i thi?n t?t", "Nguy?n Th? Hoa", "Paracetamol 500mg x1", "[{\"name\":\"Paracetamol 500mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Khi dau, t?i da 2 viên/ngày\"}]", "0912345678", "N?i t?ng quát", "Tái khám, dau d?u gi?m", "2026-03-24" },
                    { 903, null, "Viêm da ti?p xúc", "BS. Ph?m H?ng Nhung", 503, "Tránh ti?p xúc hóa ch?t", "Nguy?n Th? Hoa", "Kem bôi Hydrocortisone 1%", "[{\"name\":\"Kem bôi Hydrocortisone 1%\",\"qty\":1,\"unit\":\"Tuýp\",\"dosage\":\"Bôi vùng t?n thuong 2 l?n/ngày\"}]", "0912345678", "Da li?u", "Da tay b? d?, ng?a, b? n?i m?n", "2026-05-15" },
                    { 904, null, "Viêm d? dày", "BS. Nguy?n Van An", 504, "Kiêng d? cay nóng, ru?u bia", "Tr?n Van Minh", "Omeprazole 20mg x2, Sucralfate x3", "[{\"name\":\"Omeprazole 20mg\",\"qty\":14,\"unit\":\"Viên\",\"dosage\":\"Ngày 2 l?n tru?c an 30 phút\"},{\"name\":\"Sucralfate 1g\",\"qty\":21,\"unit\":\"Gói\",\"dosage\":\"Ngày 3 l?n tru?c an\"}]", "0987654321", "N?i t?ng quát", "Ðau b?ng vùng thu?ng v?, ? chua", "2026-01-20" },
                    { 905, null, "Tang huy?t áp d? 1", "BS. Lê Minh Tu?n", 505, "Theo dõi huy?t áp hàng ngày", "Tr?n Van Minh", "Amlodipine 5mg x1", "[{\"name\":\"Amlodipine 5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng sau an\"}]", "0987654321", "Tim m?ch", "Chóng m?t, dau d?u khi thay d?i tu th?", "2026-04-05" },
                    { 906, null, "Viêm ph? qu?n c?p (bé)", "BS. Tr?n Th? Bích", 506, "U?ng nhi?u nu?c, ngh? ngoi", "Lê Th? Lan", "Amoxicillin 250mg x3, Thu?c ho B?o Thanh", "[{\"name\":\"Amoxicillin 250mg\",\"qty\":21,\"unit\":\"Viên\",\"dosage\":\"Ngày 3 l?n, m?i l?n 1 viên\"},{\"name\":\"Thu?c ho B?o Thanh\",\"qty\":1,\"unit\":\"Chai\",\"dosage\":\"Ngày 3 l?n, 10ml m?i l?n\"}]", "0909123456", "Nhi khoa", "Bé s?t cao, ho nhi?u, khó th?", "2026-02-14" },
                    { 907, "BK-M3N4O5P6", "R?i lo?n nh?p tim", "BS. Lê Minh Tu?n", 507, "H?n ch? v?n d?ng m?nh. Tái khám sau 3 tu?n.", "Ph?m Qu?c B?o", "Bisoprolol 2.5mg x1, Aspirin 81mg x1", "[{\"name\":\"Bisoprolol 2.5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng\"},{\"name\":\"Aspirin 81mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sau an trua\"}]", "0978111222", "Tim m?ch", "Ðau ng?c, khó th? khi g?ng s?c, h?i h?p", "2026-05-20" },
                    { 908, null, "Tái khám - R?i lo?n nh?p tim", "BS. Lê Minh Tu?n", 508, "Nh?p tim ?n d?nh, ti?p t?c thu?c", "Ph?m Qu?c B?o", "Bisoprolol 2.5mg x1", "[{\"name\":\"Bisoprolol 2.5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng\"}]", "0978111222", "Tim m?ch", "Tái khám, còn h?i h?p nh?", "2026-06-10" },
                    { 909, null, "M? day d? ?ng", "BS. Ph?m H?ng Nhung", 509, "Tránh h?i s?n, ph?n hoa", "Hoàng Th? Y?n", "Loratadine 10mg x1, Kem du?ng ?m", "[{\"name\":\"Loratadine 10mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, t?i\"},{\"name\":\"Kem du?ng ?m Cetaphil\",\"qty\":1,\"unit\":\"Tuýp\",\"dosage\":\"Bôi 2 l?n/ngày sau t?m\"}]", "0961222333", "Da li?u", "N?i m?n d?, ng?a kh?p ngu?i", "2026-04-18" },
                    { 910, null, "Viêm xoang mãn tính", "BS. Võ Thanh Hùng", 510, "R?a mui b?ng nu?c mu?i sinh lý hàng ngày", "Ð? Van Tùng", "Amoxicillin 250mg x3, X?t mui Nasonex", "[{\"name\":\"Amoxicillin 250mg\",\"qty\":21,\"unit\":\"Viên\",\"dosage\":\"Ngày 3 l?n, m?i l?n 1 viên\"},{\"name\":\"X?t mui Nasonex\",\"qty\":1,\"unit\":\"Chai\",\"dosage\":\"X?t 2 nhát m?i bên mui, ngày 1 l?n\"}]", "0945333444", "Tai Mui H?ng", "Ngh?t mui, dau d?u vùng trán, ch?y d?ch mui", "2026-02-28" }
                });

            _ = migrationBuilder.InsertData(
                table: "Medicines",
                columns: new[] { "Id", "ExpiryDate", "Ingredients", "Name", "Unit" },
                values: new object[,]
                {
                    { "med_01", "2027-12-31", "Paracetamol", "Paracetamol 500mg", "Viên" },
                    { "med_02", "2026-05-15", "Amoxicillin", "Amoxicillin 250mg", "Viên" },
                    { "med_03", "2028-01-20", "Ibuprofen", "Ibuprofen 400mg", "Viên" },
                    { "med_04", "2025-10-10", "Vitamin C", "Vitamin C 1000mg", "Viên s?i" },
                    { "med_05", "2026-02-28", "Th?o du?c", "Thu?c ho B?o Thanh", "Chai" }
                });

            _ = migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "Description", "DoctorCount", "Icon", "Name" },
                values: new object[,]
                {
                    { "sp_01", "Khám và di?u tr? các b?nh n?i khoa", 8, "??", "N?i t?ng quát" },
                    { "sp_02", "Cham sóc s?c kh?e tr? em", 5, "??", "Nhi khoa" },
                    { "sp_03", "Ch?n doán và di?u tr? b?nh tim", 4, "??", "Tim m?ch" },
                    { "sp_04", "Ði?u tr? các b?nh v? da", 3, "??", "Da li?u" },
                    { "sp_05", "Khám và di?u tr? TMH", 4, "??", "Tai Mui H?ng" },
                    { "sp_06", "Khám và di?u tr? b?nh v? m?t", 3, "???", "M?t" },
                    { "sp_07", "Cham sóc s?c kh?e rang mi?ng", 6, "??", "Rang Hàm M?t" },
                    { "sp_08", "Ði?u tr? xuong kh?p", 3, "??", "Ch?n thuong ch?nh hình" },
                    { "sp_09", "Ch?n doán b?nh th?n kinh", 2, "??", "Th?n kinh" },
                    { "sp_10", "Cham sóc s?c kh?e ph? n?", 5, "??", "S?n ph? khoa" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "Accounts");

            _ = migrationBuilder.DropTable(
                name: "Appointments");

            _ = migrationBuilder.DropTable(
                name: "Doctors");

            _ = migrationBuilder.DropTable(
                name: "MedicalHistories");

            _ = migrationBuilder.DropTable(
                name: "Medicines");

            _ = migrationBuilder.DropTable(
                name: "Specialties");
        }
    }
}
