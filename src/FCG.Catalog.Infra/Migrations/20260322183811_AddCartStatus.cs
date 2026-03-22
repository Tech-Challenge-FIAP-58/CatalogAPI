using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Catalog.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddCartStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Cart",
                type: "INT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[StoredEvents]', N'U') IS NULL
BEGIN
    CREATE TABLE [StoredEvents] (
        [Id] uniqueidentifier NOT NULL,
        [AggregateId] uniqueidentifier NOT NULL,
        [AggregateType] varchar(100) NOT NULL,
        [EventType] varchar(100) NOT NULL,
        [Payload] varchar(100) NOT NULL,
        [OccurredOn] datetime2 NOT NULL,
        CONSTRAINT [PK_StoredEvents] PRIMARY KEY ([Id])
    );
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[StoredEvents]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [StoredEvents];
END
");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cart");
        }
    }
}
