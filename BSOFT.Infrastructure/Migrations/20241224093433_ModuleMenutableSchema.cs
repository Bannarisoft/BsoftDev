using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModuleMenutableSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             // Move tables to the new schema
    migrationBuilder.Sql("ALTER SCHEMA AppData TRANSFER dbo.Menus");
    migrationBuilder.Sql("ALTER SCHEMA AppData TRANSFER dbo.Modules");


    // Add foreign keys
    migrationBuilder.AddForeignKey(
        name: "FK_RoleEntitlements_Menus_MenuId",
        schema: "AppSecurity",
        table: "RoleEntitlements",
        column: "MenuId",
        principalSchema: "AppData", // New schema for Menus
        principalTable: "Menus",
        principalColumn: "Id",
        onDelete: ReferentialAction.Restrict);

    migrationBuilder.AddForeignKey(
        name: "FK_RoleEntitlements_Modules_ModuleId",
        schema: "AppSecurity",
        table: "RoleEntitlements",
        column: "ModuleId",
        principalSchema: "AppData", // New schema for Modules
        principalTable: "Modules",
        principalColumn: "Id",
        onDelete: ReferentialAction.Restrict);

    migrationBuilder.AddForeignKey(
        name: "FK_RoleEntitlements_UserRole_RoleId",
        schema: "AppSecurity",
        table: "RoleEntitlements",
        column: "RoleId",
        principalSchema: "AppSecurity",
        principalTable: "UserRole",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
