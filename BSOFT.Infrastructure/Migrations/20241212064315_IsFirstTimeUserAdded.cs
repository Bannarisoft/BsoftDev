using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsFirstTimeUserAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameTable(
                name: "User",
                schema: "AppSecurity",
                newName: "Users",
                newSchema: "AppSecurity");

            migrationBuilder.AddColumn<bool>(
                name: "IsFirstTimeUser",
                schema: "AppSecurity",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Division",
                schema: "AppData");

            migrationBuilder.DropTable(
                name: "Entity",
                schema: "AppData");

            migrationBuilder.DropTable(
                name: "Unit",
                schema: "AppData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "AppSecurity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsFirstTimeUser",
                schema: "AppSecurity",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "AppSecurity",
                newName: "User",
                newSchema: "AppSecurity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                schema: "AppSecurity",
                table: "User",
                column: "UserId");
        }
    }
}
