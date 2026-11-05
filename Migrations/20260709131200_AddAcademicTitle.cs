using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDatLichKham.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademicTitle",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_01",
                columns: new[] { "AcademicTitle", "Degree" },
                values: new object[] { "PGS", "TS" });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_02",
                columns: new[] { "AcademicTitle", "Degree" },
                values: new object[] { "", "TS" });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_03",
                columns: new[] { "AcademicTitle", "Degree" },
                values: new object[] { "", "ThS" });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_04",
                columns: new[] { "AcademicTitle", "Degree" },
                values: new object[] { "", "CKI" });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_05",
                columns: new[] { "AcademicTitle", "Degree" },
                values: new object[] { "PGS", "TS" });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_06",
                columns: new[] { "AcademicTitle", "Degree" },
                values: new object[] { "", "TS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicTitle",
                table: "Doctors");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_01",
                column: "Degree",
                value: "PGS.TS");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_02",
                column: "Degree",
                value: "TS.BS");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_03",
                column: "Degree",
                value: "ThS.BS");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_04",
                column: "Degree",
                value: "BS.CKI");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_05",
                column: "Degree",
                value: "PGS.TS");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: "doc_06",
                column: "Degree",
                value: "TS.BS");
        }
    }
}
