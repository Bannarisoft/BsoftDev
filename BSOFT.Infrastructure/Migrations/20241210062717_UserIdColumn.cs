using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.AddColumn<int>(
        name: "UserId",
        table: "User",
        type: "int",
        schema: "AppSecurity",
        nullable: false,
        defaultValue: 0)
        .Annotation("SqlServer:Identity", "1, 1");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropColumn(
                name: "UserId",
                schema: "AppSecurity",
                table: "User");

        }
    }
}
