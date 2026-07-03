using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDatLichKham.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "Appointments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_01",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_02",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_03",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_04",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_05",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_06",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Appointments");
        }
    }
}
