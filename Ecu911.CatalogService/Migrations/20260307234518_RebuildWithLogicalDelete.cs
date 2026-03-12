using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class RebuildWithLogicalDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DocumentItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DocumentItems",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DocumentItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DocumentItems");
        }
    }
}
