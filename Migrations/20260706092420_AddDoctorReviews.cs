using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebDatLichKham.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Specialties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Specialties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Ingredients",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "ExpiryDate",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "VisitDate",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Symptoms",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "PrescriptionItemsJson",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Prescription",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "PatientName",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DoctorName",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "SpecialtyName",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "ScheduleJson",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Degree",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DoctorName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.CreateTable(
                name: "DoctorReviews",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoctorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_DoctorReviews", x => x.Id);
                });

            _ = migrationBuilder.InsertData(
                table: "DoctorReviews",
                columns: new[] { "Id", "Comment", "Date", "DoctorId", "DoctorName", "PatientName", "Rating", "Specialty" },
                values: new object[,]
                {
                    { "rev_01", "Bác sĩ An tư vấn rất nhiệt tình, chu đáo. Quy trình khám diễn ra nhanh chóng, tôi rất hài lòng.", "2026-06-15", "doc_01", "BS. Nguyễn Văn An", "Nguyễn Tấn Đ.", 5, "Nội tổng quát" },
                    { "rev_02", "Phòng khám sạch sẽ, bác sĩ Bích giải thích kỹ càng về bệnh án và thuốc. Tuyệt vời!", "2026-06-20", "doc_02", "BS. Trần Thị Bích", "Lê Minh H.", 5, "Nhi khoa" },
                    { "rev_03", "Chuyên môn cao, dịch vụ tốt. Tuy nhiên lúc chờ lấy thuốc hơi lâu một chút.", "2026-06-28", "doc_03", "BS. Lê Minh Tuấn", "Trần Quyết T.", 4, "Tim mạch" },
                    { "rev_04", "Bác sĩ Nhung rất nhẹ nhàng, mát tay. Các nốt mẩn đỏ của tôi đã hết hẳn sau 1 tuần dùng thuốc.", "2026-07-01", "doc_04", "BS. Phạm Hồng Nhung", "Phạm Thị M.", 5, "Da liễu" },
                    { "rev_05", "Rất chuyên nghiệp, bác sĩ Hùng thao tác cực kỳ cẩn thận.", "2026-07-02", "doc_05", "BS. Võ Thanh Hùng", "Vũ Đức K.", 5, "Tai Mũi Họng" },
                    { "rev_06", "Hệ thống đặt lịch rất tiện, không phải chờ đợi. Bác sĩ siêu có tâm.", "2026-07-03", "doc_01", "BS. Nguyễn Văn An", "Đinh Tuấn N.", 5, "Nội tổng quát" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "DoctorReviews");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Specialties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Specialties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Ingredients",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "ExpiryDate",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "VisitDate",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Symptoms",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "PrescriptionItemsJson",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Prescription",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "PatientName",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "DoctorName",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "SpecialtyName",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "ScheduleJson",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Degree",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "DoctorName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
