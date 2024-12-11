using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Unitschemamigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Units",
                table: "Units");

            migrationBuilder.EnsureSchema(
                name: "AppData");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "Unit",
                newSchema: "AppData");

            migrationBuilder.RenameColumn(
                name: "updated_user",
                schema: "AppData",
                table: "Unit",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "updated_date",
                schema: "AppData",
                table: "Unit",
                newName: "ModifiedByName");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "AppData",
                table: "Unit",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "MobileNo",
                schema: "AppData",
                table: "Unit",
                newName: "ModifiedIP");

            migrationBuilder.RenameColumn(
                name: "EmailId",
                schema: "AppData",
                table: "Unit",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "Division",
                schema: "AppData",
                table: "Unit",
                newName: "DivId");

            migrationBuilder.RenameColumn(
                name: "Created_user",
                schema: "AppData",
                table: "Unit",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "Created_date",
                schema: "AppData",
                table: "Unit",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Company",
                schema: "AppData",
                table: "Unit",
                newName: "CoId");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppData",
                table: "Unit",
                newName: "UnitId");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedIP",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Unit",
                schema: "AppData",
                table: "Unit",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Unit",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "CreatedIP",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "Mobile",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.RenameTable(
                name: "Unit",
                schema: "AppData",
                newName: "Units");

            migrationBuilder.RenameColumn(
                name: "ModifiedIP",
                table: "Units",
                newName: "MobileNo");

            migrationBuilder.RenameColumn(
                name: "ModifiedByName",
                table: "Units",
                newName: "updated_date");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "Units",
                newName: "updated_user");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Units",
                newName: "EmailId");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Units",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "DivId",
                table: "Units",
                newName: "Division");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Units",
                newName: "Created_user");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Units",
                newName: "Created_date");

            migrationBuilder.RenameColumn(
                name: "CoId",
                table: "Units",
                newName: "Company");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "Units",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Units",
                table: "Units",
                column: "Id");
        }
    }
}
