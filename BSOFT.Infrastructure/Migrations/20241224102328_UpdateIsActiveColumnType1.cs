using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIsActiveColumnType1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "AppData",
                table: "Country",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "byte");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "AppData",
                table: "Country",
                type: "byte",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
