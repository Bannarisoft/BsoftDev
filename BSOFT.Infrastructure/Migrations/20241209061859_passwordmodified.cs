using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class passwordmodified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.RenameColumn(
         name: "UserPassword",
        schema: "AppSecurity",
        table: "User",
        newName: "PasswordHash");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        
        // Rename Id back to TempId
        migrationBuilder.RenameColumn(
        name: "PasswordHash",
        schema: "AppSecurity",
        table: "User",
        newName: "UserPassword");

        }
    }
}
