using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseIdRemovedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    // Drop the foreign key constraint
        migrationBuilder.DropForeignKey(
        name: "FK_UserRole_Users_UserId",
        schema: "AppSecurity",
        table: "UserRole");

            // Drop the index for the UserId column
    migrationBuilder.DropIndex(
        name: "IX_UserRole_UserId",
        schema: "AppSecurity",
        table: "UserRole");

    // Drop the UserId column
        migrationBuilder.DropColumn(
        name: "UserId",
        schema: "AppSecurity",
        table: "UserRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
