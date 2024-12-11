using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCoIdToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                schema: "AppData",
                table: "Company",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "Modified_Time",
                schema: "AppData",
                table: "Company",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "Entity",
                schema: "AppData",
                table: "Company",
                newName: "EntityId");

            migrationBuilder.RenameColumn(
                name: "Created_Time",
                schema: "AppData",
                table: "Company",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppData",
                table: "Company",
                newName: "CoId");

            migrationBuilder.AlterColumn<int>(
                name: "YearofEstablishment",
                schema: "AppData",
                table: "Company",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CreatedIP",
                schema: "AppData",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedIP",
                schema: "AppData",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedIP",
                schema: "AppData",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ModifiedIP",
                schema: "AppData",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "Phone",
                schema: "AppData",
                table: "Company",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "AppData",
                table: "Company",
                newName: "Modified_Time");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                schema: "AppData",
                table: "Company",
                newName: "Entity");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "AppData",
                table: "Company",
                newName: "Created_Time");

            migrationBuilder.RenameColumn(
                name: "CoId",
                schema: "AppData",
                table: "Company",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "YearofEstablishment",
                schema: "AppData",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
