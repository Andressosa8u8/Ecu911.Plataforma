using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDocumentTypeDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentTypeId",
                table: "DocumentItems",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentTypeId",
                table: "DocumentItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
