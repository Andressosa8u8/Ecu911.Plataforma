using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class MakeDocumentTypeRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentItems_DocumentTypes_DocumentTypeId",
                table: "DocumentItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentTypeId",
                table: "DocumentItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentItems_DocumentTypes_DocumentTypeId",
                table: "DocumentItems",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentItems_DocumentTypes_DocumentTypeId",
                table: "DocumentItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentTypeId",
                table: "DocumentItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentItems_DocumentTypes_DocumentTypeId",
                table: "DocumentItems",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id");
        }
    }
}
