using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullColAllow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.AlterColumn<string>(
                name: "CreatedIP",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                schema: "AppSecurity",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

                migrationBuilder.AlterColumn<string>(
                name: "ModifiedIP",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                schema: "AppSecurity",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
