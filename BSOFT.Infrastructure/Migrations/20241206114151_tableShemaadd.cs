using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tableShemaadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.EnsureSchema(
                name: "AppData");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company",
                newSchema: "AppData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                schema: "AppData",
                table: "Company",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                schema: "AppData",
                table: "Company");

            migrationBuilder.RenameTable(
                name: "Company",
                schema: "AppData",
                newName: "Companies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");
        }
    }
}
