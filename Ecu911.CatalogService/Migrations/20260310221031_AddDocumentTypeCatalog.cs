using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTypeCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentTypeId",
                table: "DocumentItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentItems_DocumentTypeId",
                table: "DocumentItems",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentItems_DocumentTypes_DocumentTypeId",
                table: "DocumentItems",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentItems_DocumentTypes_DocumentTypeId",
                table: "DocumentItems");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentItems_DocumentTypeId",
                table: "DocumentItems");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "DocumentItems");
        }
    }
}
