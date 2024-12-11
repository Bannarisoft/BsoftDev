using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                // Recreate the UserId column with the IDENTITY property
        migrationBuilder.AddColumn<int>(
        name: "UserId",
        table: "Users",
        type: "int",
        nullable: false,
        defaultValue: 0)
        .Annotation("SqlServer:Identity", "1, 1");

        migrationBuilder.RenameTable(
        name: "Users",
        newName: "User");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
