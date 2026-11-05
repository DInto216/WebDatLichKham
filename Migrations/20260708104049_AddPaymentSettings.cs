using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebDatLichKham.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "PaymentSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AdditionalConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_PaymentSettings", x => x.Id);
                });

            _ = migrationBuilder.InsertData(
                table: "PaymentSettings",
                columns: new[] { "Id", "AdditionalConfigJson", "DisplayName", "Icon", "IsActive", "MethodCode", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, "Thanh toán t?i qu?y", "bi-cash", true, "cash", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "{\"bankName\":\"Vietcombank\",\"accountNumber\":\"0123456789\",\"accountName\":\"MEDIBOOKING VN\"}", "Chuy?n kho?n Ngân hàng", "bi-bank2", true, "bank_transfer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, "Ví MoMo", "bi-qr-code", true, "momo", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, null, "ZaloPay", "bi-qr-code", true, "zalopay", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "PaymentSettings");
        }
    }
}
