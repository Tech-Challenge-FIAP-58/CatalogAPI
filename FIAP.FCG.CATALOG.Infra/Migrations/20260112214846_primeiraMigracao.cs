using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIAP.FCG.CATALOG.Infra.Migrations
{
    /// <inheritdoc />
    public partial class primeiraMigracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Platform = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    PublisherName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(12,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game");
        }
    }
}
