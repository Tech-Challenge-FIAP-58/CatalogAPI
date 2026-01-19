using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIAP.FCG.CATALOG.Infra.Migrations
{
    /// <inheritdoc />
    public partial class M3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    UserId = table.Column<int>(type: "INT", nullable: false),
                    GameId = table.Column<int>(type: "INT", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(12,2)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    CardName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    CardNumber = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    ExpirationDate = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Cvv = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
