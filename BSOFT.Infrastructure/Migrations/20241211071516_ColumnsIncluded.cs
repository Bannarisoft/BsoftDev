using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ColumnsIncluded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                 migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                schema: "AppSecurity",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "RoleEntitlement",
                type: "int",
                schema: "AppSecurity",
                nullable: true,
                defaultValue: null); // Changed default value to null for int type

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByName",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                schema: "AppSecurity",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "RoleEntitlement",
                type: "datetime2",
                schema: "AppSecurity",
                nullable: true,
                defaultValue: null); // Changed default value to null for datetime2 type

            migrationBuilder.AddColumn<string>(
                name: "CreatedIP",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                schema: "AppSecurity",
                nullable: false,
                defaultValue: ""); // Kept as non-nullable with empty string default

            migrationBuilder.AddColumn<string>(
                name: "ModifiedIP",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                schema: "AppSecurity",
                nullable: false,
                defaultValue: "");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CreatedByName", table: "RoleEntitlement");
            migrationBuilder.DropColumn(name: "ModifiedBy", table: "RoleEntitlement");
            migrationBuilder.DropColumn(name: "ModifiedByName", table: "RoleEntitlement");
            migrationBuilder.DropColumn(name: "ModifiedAt", table: "RoleEntitlement");
            migrationBuilder.DropColumn(name: "CreatedIP", table: "RoleEntitlement");
            migrationBuilder.DropColumn(name: "ModifiedIP", table: "RoleEntitlement");

        }
    }
}
