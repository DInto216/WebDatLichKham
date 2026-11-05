using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDatLichKham.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_01",
                columns: new[] { "Email", "Experience", "Phone" },
                values: new object[] { "", "", "" });

            _ = migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_02",
                columns: new[] { "Email", "Experience", "Phone" },
                values: new object[] { "", "", "" });

            _ = migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_03",
                columns: new[] { "Email", "Experience", "Phone" },
                values: new object[] { "", "", "" });

            _ = migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_04",
                columns: new[] { "Email", "Experience", "Phone" },
                values: new object[] { "", "", "" });

            _ = migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_05",
                columns: new[] { "Email", "Experience", "Phone" },
                values: new object[] { "", "", "" });

            _ = migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_06",
                columns: new[] { "Email", "Experience", "Phone" },
                values: new object[] { "", "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "Email",
                table: "Doctors");

            _ = migrationBuilder.DropColumn(
                name: "Experience",
                table: "Doctors");

            _ = migrationBuilder.DropColumn(
                name: "Phone",
                table: "Doctors");

            _ = migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
