using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Catalog.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddGameLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "INT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameLibraryItem",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Platform = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    PublisherName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "DECIMAL(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLibraryItem", x => new { x.GameLibraryId, x.GameId });
                    table.ForeignKey(
                        name: "FK_GameLibraryItem_GameLibrary_GameLibraryId",
                        column: x => x.GameLibraryId,
                        principalTable: "GameLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameLibraryItem");

            migrationBuilder.DropTable(
                name: "GameLibrary");
        }
    }
}
