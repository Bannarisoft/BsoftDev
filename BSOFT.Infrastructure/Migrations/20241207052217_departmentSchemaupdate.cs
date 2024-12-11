using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class departmentSchemaupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AppData");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "Department",
                newSchema: "AppData");

            migrationBuilder.RenameColumn(
                name: "ModifiedUser",
                schema: "AppData",
                table: "Department",
                newName: "ModifiedIP");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                schema: "AppData",
                table: "Department",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUser",
                schema: "AppData",
                table: "Department",
                newName: "ModifiedByName");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                schema: "AppData",
                table: "Department",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Company",
                schema: "AppData",
                table: "Department",
                newName: "CreatedIP");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppData",
                table: "Department",
                newName: "DeptId");

            migrationBuilder.AddColumn<int>(
                name: "CoId",
                schema: "AppData",
                table: "Department",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "AppData",
                table: "Department",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                schema: "AppData",
                table: "Department",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "AppData",
                table: "Department",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoId",
                schema: "AppData",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "AppData",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                schema: "AppData",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "AppData",
                table: "Department");

            migrationBuilder.RenameTable(
                name: "Department",
                schema: "AppData",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "ModifiedIP",
                table: "Department",
                newName: "ModifiedUser");

            migrationBuilder.RenameColumn(
                name: "ModifiedByName",
                table: "Department",
                newName: "CreatedUser");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Department",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedIP",
                table: "Department",
                newName: "Company");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Department",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "DeptId",
                table: "Department",
                newName: "Id");
        }
    }
}
