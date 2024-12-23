using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserIDColumnsChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "AppSecurity",
                table: "Users",
                newName: "UserRoleId");

            migrationBuilder.RenameColumn(
                name: "DivId",
                schema: "AppSecurity",
                table: "Users",
                newName: "DivisionId");

            migrationBuilder.RenameColumn(
                name: "CoId",
                schema: "AppSecurity",
                table: "Users",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlement",
                newName: "UserRoleId");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

migrationBuilder.RenameColumn(
                name: "UserRoleId",
                schema: "AppSecurity",
                table: "Users",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "DivisionId",
                schema: "AppSecurity",
                table: "Users",
                newName: "DivId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                schema: "AppSecurity",
                table: "Users",
                newName: "CoId");

            migrationBuilder.RenameColumn(
                name: "UserRoleId",
                schema: "AppSecurity",
                table: "RoleEntitlement",
                newName: "RoleId");



        }
    }
}
