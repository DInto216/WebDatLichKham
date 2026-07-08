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
            _ = migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            _ = migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "Appointments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            _ = migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            _ = migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            _ = migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_01",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            _ = migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_02",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            _ = migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_03",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            _ = migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_04",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            _ = migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_05",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });

            _ = migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: "appt_06",
                columns: new[] { "PaidAt", "PaymentAmount", "PaymentMethod", "PaymentStatus" },
                values: new object[] { null, 0m, "cash", "unpaid" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Appointments");

            _ = migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Appointments");

            _ = migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Appointments");

            _ = migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Appointments");
        }
    }
}
