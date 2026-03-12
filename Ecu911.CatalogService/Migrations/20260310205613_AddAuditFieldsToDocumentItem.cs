using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToDocumentItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "DocumentItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "DocumentItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DocumentItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "DocumentItems",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DocumentItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "DocumentItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DocumentItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DocumentItems");
        }
    }
}
