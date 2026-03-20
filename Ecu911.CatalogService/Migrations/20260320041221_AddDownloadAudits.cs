using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddDownloadAudits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DownloadAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    DownloadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DownloadedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownloadAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DownloadAudits_DocumentFiles_DocumentFileId",
                        column: x => x.DocumentFileId,
                        principalTable: "DocumentFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DownloadAudits_DocumentItems_DocumentItemId",
                        column: x => x.DocumentItemId,
                        principalTable: "DocumentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DownloadAudits_DocumentFileId",
                table: "DownloadAudits",
                column: "DocumentFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadAudits_DocumentItemId",
                table: "DownloadAudits",
                column: "DocumentItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DownloadAudits");
        }
    }
}
