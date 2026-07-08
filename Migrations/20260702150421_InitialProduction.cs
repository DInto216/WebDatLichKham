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
                    { "acc_admin", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Quản trị viên", "admin123", "admin", "admin" },
                    { "acc_doc01", null, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "doc_01", "BS. Nguyễn Văn An", "doctor123", "doctor", "doctor01" },
                    { "acc_doc02", null, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "doc_02", "BS. Trần Thị Bích", "doctor123", "doctor", "doctor02" },
                    { "acc_doc03", null, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "doc_03", "BS. Lê Minh Tuấn", "doctor123", "doctor", "doctor03" }
                });

            _ = migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "BirthYear", "BookingId", "CreatedAt", "Date", "DoctorId", "DoctorName", "Email", "Gender", "PatientName", "Phone", "Reason", "Specialty", "Status", "Time" },
                values: new object[,]
                {
                    { "appt_01", 1990, "BK-A1B2C3D4", new DateTime(2026, 6, 25, 10, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-26", "doc_01", "BS. Nguyễn Văn An", "hoa@email.com", "Nữ", "Nguyễn Thị Hoa", "0912345678", "Đau đầu, chóng mặt kéo dài", "Nội tổng quát", "pending", "08:00" },
                    { "appt_02", 1985, "BK-E5F6G7H8", new DateTime(2026, 6, 24, 14, 30, 0, 0, DateTimeKind.Unspecified), "2026-06-26", "doc_01", "BS. Nguyễn Văn An", "minh@email.com", "Nam", "Trần Văn Minh", "0987654321", "Khám sức khỏe tổng quát", "Nội tổng quát", "approved", "09:00" },
                    { "appt_03", 1995, "BK-I9J0K1L2", new DateTime(2026, 6, 24, 9, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-26", "doc_02", "BS. Trần Thị Bích", "lan@email.com", "Nữ", "Lê Thị Lan", "0909123456", "Bé sốt cao, ho nhiều", "Nhi khoa", "approved", "08:30" },
                    { "appt_04", 1978, "BK-M3N4O5P6", new DateTime(2026, 6, 23, 16, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-25", "doc_03", "BS. Lê Minh Tuấn", "bao@email.com", "Nam", "Phạm Quốc Bảo", "0978111222", "Đau ngực, khó thở", "Tim mạch", "completed", "14:00" },
                    { "appt_05", 2000, "BK-Q7R8S9T0", new DateTime(2026, 6, 25, 11, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-27", "doc_04", "BS. Phạm Hồng Nhung", "yen@email.com", "Nữ", "Hoàng Thị Yến", "0961222333", "Nổi mẩn đỏ, ngứa da", "Da liễu", "pending", "09:00" },
                    { "appt_06", 1970, "BK-U1V2W3X4", new DateTime(2026, 6, 22, 8, 0, 0, 0, DateTimeKind.Unspecified), "2026-06-25", "doc_05", "BS. Võ Thanh Hùng", "tung@email.com", "Nam", "Đỗ Văn Tùng", "0945333444", "Viêm họng mãn tính", "Tai Mũi Họng", "cancelled", "08:30" }
                });

            _ = migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "Avatar", "Bio", "Degree", "Name", "Price", "Rating", "Reviews", "ScheduleJson", "SpecialtyId", "SpecialtyName" },
                values: new object[,]
                {
                    { "doc_01", "", "Hơn 15 năm kinh nghiệm trong lĩnh vực Nội khoa tổng quát", "PGS.TS", "BS. Nguyễn Văn An", 350000m, 4.7999999999999998, 124, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\",\"16:00\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"sunday\":[]}", "sp_01", "Nội tổng quát" },
                    { "doc_02", "", "20 năm kinh nghiệm chuyên khoa Nhi, từng tu nghiệp tại Nhật Bản", "TS.BS", "BS. Trần Thị Bích", 400000m, 4.9000000000000004, 210, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"10:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\",\"15:00\"],\"wednesday\":[],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"sunday\":[]}", "sp_02", "Nhi khoa" },
                    { "doc_03", "", "Chuyên gia tim mạch can thiệp, tốt nghiệp ĐH Y Hà Nội", "ThS.BS", "BS. Lê Minh Tuấn", 500000m, 4.7000000000000002, 98, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"thursday\":[],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"saturday\":[],\"sunday\":[]}", "sp_03", "Tim mạch" },
                    { "doc_04", "", "Chuyên gia Da liễu thẩm mỹ, nhiều năm kinh nghiệm", "BS.CKI", "BS. Phạm Hồng Nhung", 300000m, 4.5999999999999996, 156, "{\"monday\":[\"09:00\",\"09:30\",\"10:00\",\"10:30\",\"14:00\",\"14:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"wednesday\":[\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"friday\":[],\"saturday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"sunday\":[]}", "sp_04", "Da liễu" },
                    { "doc_05", "", "Phó Giáo sư chuyên ngành Tai Mũi Họng", "PGS.TS", "BS. Võ Thanh Hùng", 350000m, 4.7999999999999998, 89, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"tuesday\":[\"14:00\",\"14:30\",\"15:00\",\"15:30\",\"16:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\",\"14:00\",\"14:30\",\"15:00\"],\"friday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\"],\"saturday\":[],\"sunday\":[]}", "sp_05", "Tai Mũi Họng" },
                    { "doc_06", "", "Tiến sĩ chuyên ngành Sản phụ khoa, BV Từ Dũ", "TS.BS", "BS. Đặng Thị Mai", 450000m, 4.9000000000000004, 175, "{\"monday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"14:00\",\"14:30\"],\"tuesday\":[\"08:00\",\"08:30\",\"09:00\"],\"wednesday\":[\"08:00\",\"08:30\",\"09:00\",\"09:30\",\"10:00\",\"14:00\",\"14:30\",\"15:00\"],\"thursday\":[\"08:00\",\"08:30\",\"09:00\"],\"friday\":[\"14:00\",\"14:30\",\"15:00\",\"15:30\"],\"saturday\":[\"08:00\",\"08:30\",\"09:00\"],\"sunday\":[]}", "sp_10", "Sản phụ khoa" }
                });

            _ = migrationBuilder.InsertData(
                table: "MedicalHistories",
                columns: new[] { "MaLichSu", "BookingId", "Diagnosis", "DoctorName", "MaBenhAn", "Notes", "PatientName", "Prescription", "PrescriptionItemsJson", "SoDienThoai", "Specialty", "Symptoms", "VisitDate" },
                values: new object[,]
                {
                    { 901, "BK-A1B2C3D4", "Đau nửa đầu migraine", "BS. Nguyễn Văn An", 501, "Tái khám sau 2 tuần. Tránh căng thẳng, ngủ đủ giấc.", "Nguyễn Thị Hoa", "Paracetamol 500mg x2, Ibuprofen 400mg x1", "[{\"name\":\"Paracetamol 500mg\",\"qty\":20,\"unit\":\"Viên\",\"dosage\":\"Ngày 2 lần, mỗi lần 1 viên, sau ăn\"},{\"name\":\"Ibuprofen 400mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 lần khi đau, sau ăn no\"}]", "0912345678", "Nội tổng quát", "Đau đầu dữ dội vùng trán và thái dương, buồn nôn, nhạy cảm ánh sáng", "2026-03-10" },
                    { 902, null, "Tái khám - Đau nửa đầu", "BS. Nguyễn Văn An", 502, "Tình trạng cải thiện tốt", "Nguyễn Thị Hoa", "Paracetamol 500mg x1", "[{\"name\":\"Paracetamol 500mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Khi đau, tối đa 2 viên/ngày\"}]", "0912345678", "Nội tổng quát", "Tái khám, đau đầu giảm", "2026-03-24" },
                    { 903, null, "Viêm da tiếp xúc", "BS. Phạm Hồng Nhung", 503, "Tránh tiếp xúc hóa chất", "Nguyễn Thị Hoa", "Kem bôi Hydrocortisone 1%", "[{\"name\":\"Kem bôi Hydrocortisone 1%\",\"qty\":1,\"unit\":\"Tuýp\",\"dosage\":\"Bôi vùng tổn thương 2 lần/ngày\"}]", "0912345678", "Da liễu", "Da tay bị đỏ, ngứa, bị nổi mẩn", "2026-05-15" },
                    { 904, null, "Viêm dạ dày", "BS. Nguyễn Văn An", 504, "Kiêng đồ cay nóng, rượu bia", "Trần Văn Minh", "Omeprazole 20mg x2, Sucralfate x3", "[{\"name\":\"Omeprazole 20mg\",\"qty\":14,\"unit\":\"Viên\",\"dosage\":\"Ngày 2 lần trước ăn 30 phút\"},{\"name\":\"Sucralfate 1g\",\"qty\":21,\"unit\":\"Gói\",\"dosage\":\"Ngày 3 lần trước ăn\"}]", "0987654321", "Nội tổng quát", "Đau bụng vùng thượng vị, ợ chua", "2026-01-20" },
                    { 905, null, "Tăng huyết áp độ 1", "BS. Lê Minh Tuấn", 505, "Theo dõi huyết áp hàng ngày", "Trần Văn Minh", "Amlodipine 5mg x1", "[{\"name\":\"Amlodipine 5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng sau ăn\"}]", "0987654321", "Tim mạch", "Chóng mặt, đau đầu khi thay đổi tư thế", "2026-04-05" },
                    { 906, null, "Viêm phế quản cấp (bé)", "BS. Trần Thị Bích", 506, "Uống nhiều nước, nghỉ ngơi", "Lê Thị Lan", "Amoxicillin 250mg x3, Thuốc ho Bảo Thanh", "[{\"name\":\"Amoxicillin 250mg\",\"qty\":21,\"unit\":\"Viên\",\"dosage\":\"Ngày 3 lần, mỗi lần 1 viên\"},{\"name\":\"Thuốc ho Bảo Thanh\",\"qty\":1,\"unit\":\"Chai\",\"dosage\":\"Ngày 3 lần, 10ml mỗi lần\"}]", "0909123456", "Nhi khoa", "Bé sốt cao, ho nhiều, khó thở", "2026-02-14" },
                    { 907, "BK-M3N4O5P6", "Rối loạn nhịp tim", "BS. Lê Minh Tuấn", 507, "Hạn chế vận động mạnh. Tái khám sau 3 tuần.", "Phạm Quốc Bảo", "Bisoprolol 2.5mg x1, Aspirin 81mg x1", "[{\"name\":\"Bisoprolol 2.5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng\"},{\"name\":\"Aspirin 81mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sau ăn trưa\"}]", "0978111222", "Tim mạch", "Đau ngực, khó thở khi gắng sức, hồi hộp", "2026-05-20" },
                    { 908, null, "Tái khám - Rối loạn nhịp tim", "BS. Lê Minh Tuấn", 508, "Nhịp tim ổn định, tiếp tục thuốc", "Phạm Quốc Bảo", "Bisoprolol 2.5mg x1", "[{\"name\":\"Bisoprolol 2.5mg\",\"qty\":30,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, sáng\"}]", "0978111222", "Tim mạch", "Tái khám, còn hồi hộp nhẹ", "2026-06-10" },
                    { 909, null, "Mề đay dị ứng", "BS. Phạm Hồng Nhung", 509, "Tránh hải sản, phấn hoa", "Hoàng Thị Yến", "Loratadine 10mg x1, Kem dưỡng ẩm", "[{\"name\":\"Loratadine 10mg\",\"qty\":10,\"unit\":\"Viên\",\"dosage\":\"Ngày 1 viên, tối\"},{\"name\":\"Kem dưỡng ẩm Cetaphil\",\"qty\":1,\"unit\":\"Tuýp\",\"dosage\":\"Bôi 2 lần/ngày sau tắm\"}]", "0961222333", "Da liễu", "Nổi mẩn đỏ, ngứa khắp người", "2026-04-18" },
                    { 910, null, "Viêm xoang mãn tính", "BS. Võ Thanh Hùng", 510, "Rửa mũi bằng nước muối sinh lý hàng ngày", "Đỗ Văn Tùng", "Amoxicillin 250mg x3, Xịt mũi Nasonex", "[{\"name\":\"Amoxicillin 250mg\",\"qty\":21,\"unit\":\"Viên\",\"dosage\":\"Ngày 3 lần, mỗi lần 1 viên\"},{\"name\":\"Xịt mũi Nasonex\",\"qty\":1,\"unit\":\"Chai\",\"dosage\":\"Xịt 2 nhát mỗi bên mũi, ngày 1 lần\"}]", "0945333444", "Tai Mũi Họng", "Nghẹt mũi, đau đầu vùng trán, chảy dịch mũi", "2026-02-28" }
                });

            _ = migrationBuilder.InsertData(
                table: "Medicines",
                columns: new[] { "Id", "ExpiryDate", "Ingredients", "Name", "Unit" },
                values: new object[,]
                {
                    { "med_01", "2027-12-31", "Paracetamol", "Paracetamol 500mg", "Viên" },
                    { "med_02", "2026-05-15", "Amoxicillin", "Amoxicillin 250mg", "Viên" },
                    { "med_03", "2028-01-20", "Ibuprofen", "Ibuprofen 400mg", "Viên" },
                    { "med_04", "2025-10-10", "Vitamin C", "Vitamin C 1000mg", "Viên sủi" },
                    { "med_05", "2026-02-28", "Thảo dược", "Thuốc ho Bảo Thanh", "Chai" }
                });

            _ = migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "Description", "DoctorCount", "Icon", "Name" },
                values: new object[,]
                {
                    { "sp_01", "Khám và điều trị các bệnh nội khoa", 8, "🫀", "Nội tổng quát" },
                    { "sp_02", "Chăm sóc sức khỏe trẻ em", 5, "👶", "Nhi khoa" },
                    { "sp_03", "Chẩn đoán và điều trị bệnh tim", 4, "❤️", "Tim mạch" },
                    { "sp_04", "Điều trị các bệnh về da", 3, "🧴", "Da liễu" },
                    { "sp_05", "Khám và điều trị TMH", 4, "👂", "Tai Mũi Họng" },
                    { "sp_06", "Khám và điều trị bệnh về mắt", 3, "👁️", "Mắt" },
                    { "sp_07", "Chăm sóc sức khỏe răng miệng", 6, "🦷", "Răng Hàm Mặt" },
                    { "sp_08", "Điều trị xương khớp", 3, "🦴", "Chấn thương chỉnh hình" },
                    { "sp_09", "Chẩn đoán bệnh thần kinh", 2, "🧠", "Thần kinh" },
                    { "sp_10", "Chăm sóc sức khỏe phụ nữ", 5, "🤰", "Sản phụ khoa" }
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
