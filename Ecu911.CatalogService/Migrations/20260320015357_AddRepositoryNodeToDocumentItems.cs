using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryNodeToDocumentItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_DocumentItems_DocumentItemId",
                table: "DocumentFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationalUnits_OrganizationalUnits_ParentId",
                table: "OrganizationalUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_RepositoryNodes_OrganizationalUnits_OrganizationalUnitId",
                table: "RepositoryNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_RepositoryNodes_RepositoryNodes_ParentId",
                table: "RepositoryNodes");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "DocumentItems",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "DocumentItems",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "RepositoryNodeId",
                table: "DocumentItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentItems_RepositoryNodeId",
                table: "DocumentItems",
                column: "RepositoryNodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_DocumentItems_DocumentItemId",
                table: "DocumentFiles",
                column: "DocumentItemId",
                principalTable: "DocumentItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentItems_RepositoryNodes_RepositoryNodeId",
                table: "DocumentItems",
                column: "RepositoryNodeId",
                principalTable: "RepositoryNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationalUnits_OrganizationalUnits_ParentId",
                table: "OrganizationalUnits",
                column: "ParentId",
                principalTable: "OrganizationalUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepositoryNodes_OrganizationalUnits_OrganizationalUnitId",
                table: "RepositoryNodes",
                column: "OrganizationalUnitId",
                principalTable: "OrganizationalUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepositoryNodes_RepositoryNodes_ParentId",
                table: "RepositoryNodes",
                column: "ParentId",
                principalTable: "RepositoryNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_DocumentItems_DocumentItemId",
                table: "DocumentFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentItems_RepositoryNodes_RepositoryNodeId",
                table: "DocumentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationalUnits_OrganizationalUnits_ParentId",
                table: "OrganizationalUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_RepositoryNodes_OrganizationalUnits_OrganizationalUnitId",
                table: "RepositoryNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_RepositoryNodes_RepositoryNodes_ParentId",
                table: "RepositoryNodes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentItems_RepositoryNodeId",
                table: "DocumentItems");

            migrationBuilder.DropColumn(
                name: "RepositoryNodeId",
                table: "DocumentItems");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "DocumentItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "DocumentItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_DocumentItems_DocumentItemId",
                table: "DocumentFiles",
                column: "DocumentItemId",
                principalTable: "DocumentItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationalUnits_OrganizationalUnits_ParentId",
                table: "OrganizationalUnits",
                column: "ParentId",
                principalTable: "OrganizationalUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RepositoryNodes_OrganizationalUnits_OrganizationalUnitId",
                table: "RepositoryNodes",
                column: "OrganizationalUnitId",
                principalTable: "OrganizationalUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RepositoryNodes_RepositoryNodes_ParentId",
                table: "RepositoryNodes",
                column: "ParentId",
                principalTable: "RepositoryNodes",
                principalColumn: "Id");
        }
    }
}
