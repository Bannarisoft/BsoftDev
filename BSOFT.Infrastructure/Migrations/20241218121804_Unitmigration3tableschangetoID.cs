using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Unitmigration3tableschangetoID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitContactId",
                schema: "AppData",
                table: "UnitContacts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UnitAddressId",
                schema: "AppData",
                table: "UnitAddress",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                schema: "AppData",
                table: "Unit",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppData",
                table: "UnitContacts",
                newName: "UnitContactId");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppData",
                table: "UnitAddress",
                newName: "UnitAddressId");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppData",
                table: "Unit",
                newName: "UnitId");
        }
    }
}
